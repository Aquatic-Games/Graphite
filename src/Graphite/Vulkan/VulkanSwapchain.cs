using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly KhrSwapchain _swapchainExt;
    
    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;

        if (!_vk.TryGetDeviceExtension(device.Instance, device.Device, out _swapchainExt))
            throw new Exception("Failed to get KhrSwapchain extension.");

        VulkanSurface vkSurface = (VulkanSurface) info.Surface;
        SurfaceCapabilitiesKHR capabilities;
        vkSurface.SurfaceExt
            .GetPhysicalDeviceSurfaceCapabilities(device.PhysicalDevice, vkSurface.Surface, &capabilities)
            .Check("Get surface capabilities");

        Extent2D extent = new Extent2D(info.Size.Width, info.Size.Height);
        
        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = vkSurface.Surface,
            
        }
    }
    
    public override void Dispose()
    {
        
    }
}