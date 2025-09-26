using Graphite.Core;
using Graphite.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static Graphite.VulkanMemoryAllocator.VmaMemoryUsage;
using Offset3D = Graphite.Core.Offset3D;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly Allocator* _allocator;
    
    public readonly Image Image;
    public readonly ImageView View;
    public readonly Allocation* Allocation;

    public readonly bool IsSwapchainTexture;
    public readonly bool IsSampled;
    public readonly uint MipLevels;

    public ImageLayout CurrentLayout;

    public VulkanTexture(Vk vk, VulkanDevice device, Allocator* allocator, ref readonly TextureInfo info, void* pData) : base(info)
    {
        _vk = vk;
        _device = device.Device;
        _allocator = allocator;
        
        (ImageType type, ImageViewType viewType) = info.Type switch
        {
            TextureType.Texture2D => (ImageType.Type2D, ImageViewType.Type2D),
            _ => throw new ArgumentOutOfRangeException()
        };

        ImageUsageFlags usage = ImageUsageFlags.TransferDstBit;

        if ((info.Usage & TextureUsage.ShaderResource) != 0)
        {
            usage |= ImageUsageFlags.SampledBit;
            IsSampled = true;
        }

        if ((info.Usage & TextureUsage.ColorTarget) != 0)
            usage |= ImageUsageFlags.ColorAttachmentBit;
        if ((info.Usage & TextureUsage.DepthStencilTarget) != 0)
            usage |= ImageUsageFlags.DepthStencilAttachmentBit;

        if ((info.Usage & TextureUsage.GenerateMips) != 0)
            usage |= ImageUsageFlags.TransferSrcBit;

        Extent3D extent = new Extent3D(info.Size.Width, info.Size.Height, info.Size.Depth);
        CurrentLayout = ImageLayout.Undefined;
        MipLevels = info.MipLevels == 0
            ? GraphiteUtils.CalculateMipLevels(info.Size.Width, info.Size.Height)
            : info.MipLevels;

        ImageCreateInfo imageInfo = new()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = type,
            Format = info.Format.ToVk(),
            Extent = extent,
            MipLevels = MipLevels,
            ArrayLayers = info.ArraySize,
            Samples = SampleCountFlags.Count1Bit,
            Usage = usage,
            InitialLayout = CurrentLayout
        };

        AllocationCreateInfo allocInfo = new()
        {
            usage = VMA_MEMORY_USAGE_AUTO
        };
        
        GraphiteLog.Log("Creating image.");
        Vma.CreateImage(_allocator, &imageInfo, &allocInfo, out Image, out Allocation, null).Check("Create image");

        ImageViewCreateInfo viewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = Image,
            Format = imageInfo.Format,
            ViewType = viewType,
            Components = new ComponentMapping
            {
                R = ComponentSwizzle.Identity,
                G = ComponentSwizzle.Identity,
                B = ComponentSwizzle.Identity,
                A = ComponentSwizzle.Identity
            },
            SubresourceRange = new ImageSubresourceRange
            {
                AspectMask = ImageAspectFlags.ColorBit,
                LayerCount = 1,
                BaseArrayLayer = 0,
                LevelCount = MipLevels,
                BaseMipLevel = 0
            }
        };
        
        GraphiteLog.Log("Creating image view.");
        _vk.CreateImageView(_device, &viewInfo, null, out View).Check("Create image view");

        if (pData == null)
        {
            CommandBuffer buffer = device.BeginCommands();
            Transition(buffer, ImageLayout.Undefined, ImageLayout.ShaderReadOnlyOptimal, 0, AccessFlags.ShaderReadBit,
                PipelineStageFlags.AllGraphicsBit, PipelineStageFlags.AllGraphicsBit, mipLevels: MipLevels);
            device.EndCommands();

            return;
        }

        device.UpdateTexture(this, new Region3D(new Offset3D(), info.Size), pData);
    }
    
    public VulkanTexture(Vk vk, Image image, VkDevice device, Extent2D extent, Format format)
        : base(TextureInfo.Texture2D(format, new Size2D(extent.Width, extent.Height), 1, TextureUsage.None))
    {
        _vk = vk;
        _device = device;
        Image = image;
        IsSwapchainTexture = true;
        CurrentLayout = ImageLayout.Undefined;

        ImageViewCreateInfo viewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = Image,
            Format = format.ToVk(),
            ViewType = ImageViewType.Type2D,
            Components = new ComponentMapping
            {
                R = ComponentSwizzle.Identity,
                G = ComponentSwizzle.Identity,
                B = ComponentSwizzle.Identity,
                A = ComponentSwizzle.Identity
            },
            SubresourceRange = new ImageSubresourceRange
            {
                AspectMask = ImageAspectFlags.ColorBit,
                LayerCount = 1,
                BaseArrayLayer = 0,
                LevelCount = 1,
                BaseMipLevel = 0
            }
        };
        
        GraphiteLog.Log("Creating image view.");
        _vk.CreateImageView(_device, &viewInfo, null, out View).Check("Create image view");
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying image view.");
        _vk.DestroyImageView(_device, View, null);

        if (IsSwapchainTexture)
            return;
        
        GraphiteLog.Log("Destroying image.");
        Vma.DestroyImage(_allocator, Image, Allocation);
    }

    public void Transition(CommandBuffer cb, ImageLayout old, ImageLayout @new, AccessFlags srcAccess, AccessFlags dstAccess,
        PipelineStageFlags srcFlags, PipelineStageFlags dstFlags, ImageAspectFlags aspect = ImageAspectFlags.ColorBit,
        uint baseMipLevel = 0, uint mipLevels = 1, uint baseArrayLayer = 0, uint arrayLayers = 1)
    {
        ImageMemoryBarrier memoryBarrier = new()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = Image,
            OldLayout = old,
            NewLayout = @new,
            SrcAccessMask = srcAccess,
            DstAccessMask = dstAccess,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = aspect,
                LayerCount = arrayLayers,
                BaseArrayLayer = baseArrayLayer,
                LevelCount = mipLevels,
                BaseMipLevel = baseMipLevel
            }
        };

        _vk.CmdPipelineBarrier(cb, srcFlags, dstFlags, 0, 0, null, 0, null, 1, &memoryBarrier);

        CurrentLayout = @new;
    }

    public void Transition(CommandBuffer cb, ImageLayout @new)
        => Transition(cb, CurrentLayout, @new, AccessFlags.ColorAttachmentReadBit, AccessFlags.ColorAttachmentReadBit,
            PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.ColorAttachmentOutputBit);
}