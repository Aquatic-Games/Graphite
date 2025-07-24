using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    public readonly Image Image;
    public readonly ImageView View;

    public readonly bool IsSwapchainTexture;

    public ImageLayout CurrentLayout;
    
    public VulkanTexture(Vk vk, Image image, VkDevice device, Extent2D extent, VkFormat format)
        : base(new Size2D(extent.Width, extent.Height))
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
            Format = format,
            ViewType = ImageViewType.Type2D,
            Components = new ComponentMapping
            {
                R = ComponentSwizzle.Identity,
                G = ComponentSwizzle.Identity,
                B = ComponentSwizzle.Identity,
                A = ComponentSwizzle.Identity
            },
            SubresourceRange = new ImageSubresourceRange()
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
        _vk.DestroyImage(_device, Image, null);
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