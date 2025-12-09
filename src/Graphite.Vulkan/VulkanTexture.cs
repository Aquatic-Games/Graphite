using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly bool _isSwapchainOwned;

    public readonly Image Image;
    public readonly ImageView ImageView;

    public ImageLayout CurrentLayout;

    public VulkanTexture(Vk vk, VkDevice device, Image image, VkFormat format)
    {
        _vk = vk;
        _device = device;
        _isSwapchainOwned = true;
        Image = image;
        CurrentLayout = ImageLayout.Undefined;

        ImageViewCreateInfo imageViewInfo = new()
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
            SubresourceRange = new ImageSubresourceRange
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseArrayLayer = 0,
                LayerCount = 1,
                BaseMipLevel = 0,
                LevelCount = 1
            }
        };
        
        Instance.Log("Creating swapchain image view.");
        _vk.CreateImageView(_device, &imageViewInfo, null, out ImageView).Check("Create image view");
    }

    public void Transition(CommandBuffer cb, ImageLayout old, ImageLayout @new, AccessFlags srcAccessMask = AccessFlags.ColorAttachmentReadBit, AccessFlags dstAccessMask = AccessFlags.ColorAttachmentReadBit)
    {
        ImageMemoryBarrier memoryBarrier = new()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = Image,
            OldLayout = old,
            NewLayout = @new,
            SrcAccessMask = srcAccessMask,
            DstAccessMask = dstAccessMask,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseArrayLayer = 0,
                LayerCount = 1,
                BaseMipLevel = 0,
                LevelCount = 1
            }
        };

        _vk.CmdPipelineBarrier(cb, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &memoryBarrier);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.DestroyImageView(_device, ImageView, null);

        if (_isSwapchainOwned)
            return;
        
        _vk.DestroyImage(_device, Image, null);
    }
}