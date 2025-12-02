using Graphite.Core;
using Graphite.Exceptions;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;
    
    private Size2D _size;
    private PresentMode _presentMode;
    private readonly Format _format;
    private readonly uint _imageCount;

    private readonly KhrSwapchain _khrSwapchain;
    private SwapchainKHR _swapchain;

    public override Size2D Size => _size;

    public override Format Format => _format;

    public override PresentMode PresentMode => _presentMode;
    
    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;
        _surface = (VulkanSurface) info.Surface;
        _size = info.Size;
        _format = info.Format;
        _imageCount = info.NumBuffers;
        _presentMode = info.PresentMode;

        if (!_vk.TryGetDeviceExtension(_device.Instance, _device.Device, out _khrSwapchain))
            throw new UnsupportedFeatureException($"Failed to get {KhrSwapchain.ExtensionName} instance extension.");

        CreateSwapchain();
    }

    private void CreateSwapchain()
    {
        SurfaceCapabilitiesKHR surfaceCapabilities;
        _surface.KhrSurface.GetPhysicalDeviceSurfaceCapabilities(_device.PhysicalDevice, _surface.Surface,
            &surfaceCapabilities);
        
        Instance.Log($"Requesting swapchain size: {_size}");

        Extent2D minExtent = surfaceCapabilities.MinImageExtent;
        _size.Width = uint.Max(_size.Width, minExtent.Width);
        _size.Height = uint.Max(_size.Height, minExtent.Height);

        Extent2D maxExtent = surfaceCapabilities.MaxImageExtent;
        _size.Width = uint.Min(_size.Width, maxExtent.Width);
        _size.Height = uint.Min(_size.Height, maxExtent.Height);
        
        Instance.Log($"Got swapchain size: {_size}");

        VkFormat format = _format.ToVk();
        ColorSpaceKHR colorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr;
        Instance.Log($"Format: {format}, ColorSpace: {colorSpace}");

        uint imageCount = _imageCount;
        Instance.Log($"Requesting image count: {imageCount}");
        imageCount = uint.Clamp(imageCount, surfaceCapabilities.MinImageCount, surfaceCapabilities.MaxImageCount);
        Instance.Log($"Got image count: {imageCount}");

        uint numPresentModes;
        _surface.KhrSurface.GetPhysicalDeviceSurfacePresentModes(_device.PhysicalDevice, _surface.Surface,
            &numPresentModes, null);
        Span<PresentModeKHR> presentModes = stackalloc PresentModeKHR[(int) numPresentModes];
        fixed (PresentModeKHR* pPresentModes = presentModes)
        {
            _surface.KhrSurface.GetPhysicalDeviceSurfacePresentModes(_device.PhysicalDevice, _surface.Surface,
                &numPresentModes, pPresentModes);
        }

        Instance.Log($"Requesting present mode: {_presentMode}");
        PresentModeKHR presentMode = _presentMode switch
        {
            PresentMode.Immediate => TryPresentModes(presentModes, [PresentModeKHR.ImmediateKhr]),
            PresentMode.Mailbox => TryPresentModes(presentModes, [PresentModeKHR.MailboxKhr]),
            PresentMode.Fifo => TryPresentModes(presentModes, [PresentModeKHR.FifoKhr]),
            PresentMode.FifoRelaxed => TryPresentModes(presentModes, [PresentModeKHR.FifoRelaxedKhr]),
            PresentMode.VSyncOn => TryPresentModes(presentModes, [PresentModeKHR.MailboxKhr, PresentModeKHR.FifoKhr]),
            PresentMode.VSyncOff => TryPresentModes(presentModes, [PresentModeKHR.ImmediateKhr, PresentModeKHR.FifoKhr]),
            _ => throw new ArgumentOutOfRangeException()
        };
        Instance.Log($"Got present mode: {presentMode}");
        
        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface.Surface,
            
            ImageExtent = new Extent2D(_size.Width, _size.Height),
            ImageFormat = format,
            ImageColorSpace = colorSpace,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            MinImageCount = imageCount,
            
            PresentMode = presentMode,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            Clipped = false,
            ImageSharingMode = SharingMode.Exclusive,
            
            OldSwapchain = _swapchain 
        };

        if (_device.Queues.PresentIndex != _device.Queues.GraphicsIndex)
        {
            throw new NotImplementedException(
                "Currently, Graphite requires the Graphics and Present queues to use the same queue index.");
        }
        
        Instance.Log("Creating swapchain.");
        _khrSwapchain.CreateSwapchain(_device.Device, &swapchainInfo, null, out _swapchain).Check("Create swapchain");
    }

    private static PresentModeKHR TryPresentModes(ReadOnlySpan<PresentModeKHR> availableModes, ReadOnlySpan<PresentModeKHR> acceptablePresentModes)
    {
        foreach (PresentModeKHR presentMode in acceptablePresentModes)
        {
            foreach (PresentModeKHR availableMode in availableModes)
            {
                if (presentMode == availableMode)
                    return presentMode;
            }
        }

        throw new UnsupportedFeatureException("Requested present mode or substitutes not supported.");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _khrSwapchain.DestroySwapchain(_device.Device, _swapchain, null);
        _khrSwapchain.Dispose();
    }
}