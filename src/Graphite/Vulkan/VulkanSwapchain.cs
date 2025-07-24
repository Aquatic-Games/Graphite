using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly KhrSwapchain _swapchainExt;

    public SwapchainKHR Swapchain;
    
    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;

        if (!_vk.TryGetDeviceExtension(device.Instance, device.Device, out _swapchainExt))
            throw new Exception("Failed to get KhrSwapchain extension.");

        VulkanSurface vkSurface = (VulkanSurface) info.Surface;
        SurfaceCapabilitiesKHR capabilities;
        vkSurface.SurfaceExt
            .GetPhysicalDeviceSurfaceCapabilities(device.PhysicalDevice, vkSurface.Surface, &capabilities)
            .Check("Get surface capabilities");

        uint numBuffers = info.NumBuffers;
        GraphiteLog.Log($"Requested image count: {numBuffers}");
        if (numBuffers < capabilities.MinImageCount)
            numBuffers = capabilities.MinImageCount;
        if (numBuffers > capabilities.MaxImageCount && capabilities.MaxImageCount != 0)
            numBuffers = capabilities.MaxImageCount;
        GraphiteLog.Log($"Actual image count: {numBuffers}");
        
        Extent2D extent = new Extent2D(info.Size.Width, info.Size.Height);
        GraphiteLog.Log($"Requested extent: {extent.Width}x{extent.Height}");
        extent.Width = uint.Clamp(extent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
        extent.Height = uint.Clamp(extent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);
        GraphiteLog.Log($"Actual extent: {extent.Width}x{extent.Height}");

        VkFormat format = info.Format.ToVk();
        GraphiteLog.Log($"Format: {format}");
        ColorSpaceKHR colorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr;
        GraphiteLog.Log($"Color space: {colorSpace}");

        PresentModeKHR presentMode = info.PresentMode.ToVk();
        GraphiteLog.Log($"Present mode: {presentMode}");

        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = vkSurface.Surface,

            MinImageCount = numBuffers,
            ImageExtent = extent,
            ImageFormat = format,
            ImageColorSpace = colorSpace,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,

            PresentMode = presentMode,

            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            Clipped = false
        };

        if (device.Queues.GraphicsIndex == device.Queues.PresentIndex)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
            throw new NotImplementedException("Graphics and Present queues are not the same which is not yet supported.");

        GraphiteLog.Log("Creating swapchain.");
        _swapchainExt.CreateSwapchain(device.Device, &swapchainInfo, null, out Swapchain).Check("Create Swapchain");
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying swapchain.");
        _swapchainExt.DestroySwapchain(_device.Device, Swapchain, null);
        _swapchainExt.Dispose();
    }
}