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
    private readonly VulkanSurface _surface;
    private readonly KhrSwapchain _swapchainExt;
    
    private readonly Fence _getNextTextureFence;

    private Size2D _swapchainSize;
    private Format _format;
    private PresentMode _presentMode;
    private readonly uint _numBuffers;

    private SwapchainKHR _swapchain;
    private VulkanTexture[] _swapchainTextures;
    private uint _currentImage;
    private bool _hasGotNextTextureThisFrame;

    public override Size2D Size => _swapchainSize;

    public override Format Format => _format;
    
    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;
        _surface = (VulkanSurface) info.Surface;
        _swapchainSize = info.Size;
        _numBuffers = info.NumBuffers;
        _format = info.Format;
        _presentMode = info.PresentMode;

        if (!_vk.TryGetDeviceExtension(device.Instance, device.Device, out _swapchainExt))
            throw new Exception("Failed to get KhrSwapchain extension.");
        
        CreateSwapchain();

        FenceCreateInfo fenceInfo = new()
        {
            SType = StructureType.FenceCreateInfo
        };
        GraphiteLog.Log("Creating fence.");
        _vk.CreateFence(_device.Device, &fenceInfo, null, out _getNextTextureFence).Check("Create fence");
    }

    public override Texture GetNextTexture()
    {
        Debug.Assert(_hasGotNextTextureThisFrame == false);

        Result result = _swapchainExt.AcquireNextImage(_device.Device, _swapchain, ulong.MaxValue, new Semaphore(),
            _getNextTextureFence, ref _currentImage);

        if (result == Result.ErrorOutOfDateKhr)
            RecreateSwapchain();
        else if (result != Result.SuboptimalKhr)
            result.Check("Acquire next image");

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
        
        Result result = _swapchainExt.QueuePresent(_device.Queues.Present, &presentInfo);

        if (result is Result.SuboptimalKhr or Result.ErrorOutOfDateKhr)
            RecreateSwapchain();
        else
            result.Check("Acquire next image");

        _hasGotNextTextureThisFrame = false;
    }

    private void CreateSwapchain()
    {
        SurfaceCapabilitiesKHR capabilities;
        _surface.SurfaceExt
            .GetPhysicalDeviceSurfaceCapabilities(_device.PhysicalDevice, _surface.Surface, &capabilities).Check("Get surface capabilities");

        uint numBuffers = _numBuffers;
        GraphiteLog.Log($"Requested image count: {numBuffers}");
        if (numBuffers < capabilities.MinImageCount)
            numBuffers = capabilities.MinImageCount;
        if (numBuffers > capabilities.MaxImageCount && capabilities.MaxImageCount != 0)
            numBuffers = capabilities.MaxImageCount;
        GraphiteLog.Log($"Actual image count: {numBuffers}");
        
        Extent2D extent = new Extent2D(_swapchainSize.Width, _swapchainSize.Height);
        GraphiteLog.Log($"Requested extent: {extent.Width}x{extent.Height}");
        extent.Width = uint.Clamp(extent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
        extent.Height = uint.Clamp(extent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);
        GraphiteLog.Log($"Actual extent: {extent.Width}x{extent.Height}");
        _swapchainSize = new Size2D(extent.Width, extent.Height);

        VkFormat format = _format.ToVk();
        GraphiteLog.Log($"Format: {format}");
        ColorSpaceKHR colorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr;
        GraphiteLog.Log($"Color space: {colorSpace}");

        PresentModeKHR presentMode = _presentMode.ToVk();
        GraphiteLog.Log($"Present mode: {presentMode}");
        
        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface.Surface,

            MinImageCount = numBuffers,
            ImageExtent = extent,
            ImageFormat = format,
            ImageColorSpace = colorSpace,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,

            PresentMode = presentMode,

            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            Clipped = false,
            
            OldSwapchain = _swapchain
        };

        if (_device.Queues.GraphicsIndex == _device.Queues.PresentIndex)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
            throw new NotImplementedException("Graphics and Present queues are not the same which is not yet supported.");

        GraphiteLog.Log("Creating swapchain.");
        _swapchainExt.CreateSwapchain(_device.Device, &swapchainInfo, null, out _swapchain).Check("Create Swapchain");
        
        uint numImages;
        _swapchainExt.GetSwapchainImages(_device.Device, _swapchain, &numImages, null);
        Image* images = stackalloc Image[(int) numImages];
        _swapchainExt.GetSwapchainImages(_device.Device, _swapchain, &numImages, images);
        
        GraphiteLog.Log("Creating swapchain textures.");
        _swapchainTextures = new VulkanTexture[numImages];
        for (uint i = 0; i < numImages; i++)
            _swapchainTextures[i] = new VulkanTexture(_vk, images[i], _device.Device, extent, _format);
    }

    private void RecreateSwapchain()
    {
        // In order to pass a valid swapchain into OldSwapchain, we must only destroy the old swapchain after the new
        // one has been created - however since CreateSwapchain overwrites _swapchain, we must store it in a temporary
        // variable here.
        SwapchainKHR swapchain = _swapchain;
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        CreateSwapchain();
        GraphiteLog.Log("Destroying old swapchain.");
        _swapchainExt.DestroySwapchain(_device.Device, swapchain, null);
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Destroying fence");
        _vk.DestroyFence(_device.Device, _getNextTextureFence, null);
        
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        
        GraphiteLog.Log("Destroying swapchain.");
        _swapchainExt.DestroySwapchain(_device.Device, _swapchain, null);
        _swapchainExt.Dispose();
    }
}