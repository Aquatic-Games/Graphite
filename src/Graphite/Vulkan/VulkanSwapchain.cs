using System.Diagnostics;
using System.Runtime.CompilerServices;
using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly KhrSwapchain _swapchainExt;
    
    private readonly Fence _getNextTextureFence;

    private SwapchainKHR _swapchain;
    private VulkanTexture[] _swapchainTextures;
    private uint _currentImage;
    private bool _hasGotNextTextureThisFrame;
    
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
        _swapchainExt.CreateSwapchain(device.Device, &swapchainInfo, null, out _swapchain).Check("Create Swapchain");

        FenceCreateInfo fenceInfo = new()
        {
            SType = StructureType.FenceCreateInfo
        };
        GraphiteLog.Log("Creating fence.");
        _vk.CreateFence(_device.Device, &fenceInfo, null, out _getNextTextureFence).Check("Create fence");

        uint numImages;
        _swapchainExt.GetSwapchainImages(_device.Device, _swapchain, &numImages, null);
        Image* images = stackalloc Image[(int) numImages];
        _swapchainExt.GetSwapchainImages(_device.Device, _swapchain, &numImages, images);
        
        GraphiteLog.Log("Creating swapchain textures.");
        _swapchainTextures = new VulkanTexture[numImages];
        for (uint i = 0; i < numImages; i++)
            _swapchainTextures[i] = new VulkanTexture(_vk, images[i], _device.Device, extent, format);
    }

    public override Texture GetNextTexture()
    {
        Debug.Assert(_hasGotNextTextureThisFrame == false);
        
        _swapchainExt
            .AcquireNextImage(_device.Device, _swapchain, ulong.MaxValue, new Semaphore(), _getNextTextureFence,
                ref _currentImage).Check("Acquire next image");

        _vk.WaitForFences(_device.Device, 1, in _getNextTextureFence, true, ulong.MaxValue).Check("Wait for fence");
        _vk.ResetFences(_device.Device, 1, in _getNextTextureFence).Check("Reset fence");

        _hasGotNextTextureThisFrame = true;

        return _swapchainTextures[_currentImage];
    }

    public override void Present()
    {
        // If Present() is called without GetNextTexture being called first, it will call it to mirror the behaviour of D3D11.
        if (!_hasGotNextTextureThisFrame)
            GetNextTexture();
        
        // Generally this won't ever run. It will, however, if Present() is called and no render pass has ever touched
        // the swapchain textures. This prevents "unexpected" crashes and mirrors the behaviour of D3D11.
        if (_swapchainTextures[_currentImage].CurrentLayout != ImageLayout.PresentSrcKhr)
        {
            CommandBuffer cb = _device.BeginCommands();
            _swapchainTextures[_currentImage].Transition(cb, ImageLayout.PresentSrcKhr);
            _device.EndCommands();
        }
        
        PresentInfoKHR presentInfo = new()
        {
            SType = StructureType.PresentInfoKhr,
            SwapchainCount = 1,
            PSwapchains = (SwapchainKHR*) Unsafe.AsPointer(ref _swapchain),
            PImageIndices = (uint*) Unsafe.AsPointer(ref _currentImage)
        };
        
        _swapchainExt.QueuePresent(_device.Queues.Present, &presentInfo).Check("Present");

        _hasGotNextTextureThisFrame = false;
    }

    public override void Dispose()
    {
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        
        GraphiteLog.Log("Destroying fence");
        _vk.DestroyFence(_device.Device, _getNextTextureFence, null);
        
        GraphiteLog.Log("Destroying swapchain.");
        _swapchainExt.DestroySwapchain(_device.Device, _swapchain, null);
        _swapchainExt.Dispose();
    }
}