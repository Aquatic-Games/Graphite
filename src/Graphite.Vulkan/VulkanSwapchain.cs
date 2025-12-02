using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed class VulkanSwapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;

    private SwapchainKHR _swapchain;
    
    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;

        VulkanSurface surface = (VulkanSurface) info.Surface;
        
        
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}