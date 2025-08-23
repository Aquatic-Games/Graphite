using Graphite.Core;
using Graphite.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static Graphite.VulkanMemoryAllocator.VmaMemoryUsage;

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

    public ImageLayout CurrentLayout;

    public VulkanTexture(Vk vk, VkDevice device, Allocator* allocator, ref readonly TextureInfo info) : base(info)
    {
        _vk = vk;
        _device = device;
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

        Extent3D extent = new Extent3D(info.Size.Width, info.Size.Height, info.Size.Depth);
        CurrentLayout = ImageLayout.Undefined;

        ImageCreateInfo imageInfo = new()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = type,
            Format = info.Format.ToVk(),
            Extent = extent,
            MipLevels = info.MipLevels,
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
                LevelCount = info.MipLevels,
                BaseMipLevel = 0
            }
        };
        
        GraphiteLog.Log("Creating image view.");
        _vk.CreateImageView(_device, &viewInfo, null, out View).Check("Create image view");
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

    public void Transition(CommandBuffer cb, ImageLayout @new)
    {
        ImageMemoryBarrier memoryBarrier = new()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = Image,
            OldLayout = CurrentLayout,
            NewLayout = @new,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                LayerCount = 1,
                BaseArrayLayer = 0,
                LevelCount = 1,
                BaseMipLevel = 0
            }
        };

        _vk.CmdPipelineBarrier(cb, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &memoryBarrier);

        CurrentLayout = @new;
    }
}