using Graphite.Exceptions;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSurface : Surface
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkInstance _instance;

    private readonly KhrWin32Surface? _win32Surface;
    private readonly KhrWaylandSurface? _waylandSurface;
    private readonly KhrXlibSurface? _xlibSurface;
    private readonly KhrXcbSurface? _xcbSurface;

    public readonly KhrSurface KhrSurface;
    public readonly SurfaceKHR Surface;

    public VulkanSurface(Vk vk, VkInstance instance, ref readonly SurfaceInfo info)
    {
        _vk = vk;
        _instance = instance;

        if (!_vk.TryGetInstanceExtension(_instance, out KhrSurface))
            throw new GraphicsOperationException($"Failed to get {KhrSurface.ExtensionName} instance extension.");

        switch (info.Type)
        {
            case SurfaceType.Win32:
            {
                if (!_vk.TryGetInstanceExtension(_instance, out _win32Surface))
                    throw new GraphicsOperationException($"Failed to get {KhrWin32Surface.ExtensionName} instance extension.");

                Win32SurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.Win32SurfaceCreateInfoKhr,
                    Hinstance = info.Display,
                    Hwnd = info.Window
                };
                
                Instance.Log("Creating Win32 surface.");
                _win32Surface!.CreateWin32Surface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create Win32 surface");
                break;
            }
            case SurfaceType.Wayland:
            {
                if (!_vk.TryGetInstanceExtension(_instance, out _waylandSurface))
                    throw new GraphicsOperationException($"Failed to get {KhrWaylandSurface.ExtensionName} instance extension.");

                WaylandSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.WaylandSurfaceCreateInfoKhr,
                    Display = (IntPtr*) info.Display,
                    Surface = (IntPtr*) info.Window
                };
                
                Instance.Log("Creating Wayland surface.");
                _waylandSurface!.CreateWaylandSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create Wayland surface");
                break;
            }
            case SurfaceType.Xlib:
            {
                if (!_vk.TryGetInstanceExtension(_instance, out _xlibSurface))
                    throw new GraphicsOperationException($"Failed to get {KhrXlibSurface.ExtensionName} instance extension.");

                XlibSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XlibSurfaceCreateInfoKhr,
                    Dpy = (IntPtr*) info.Display,
                    Window = info.Window
                };
                
                Instance.Log("Creating Xlib surface.");
                _xlibSurface!.CreateXlibSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create Xlib surface");
                break;
            }
            case SurfaceType.Xcb:
            {
                if (!_vk.TryGetInstanceExtension(_instance, out _xcbSurface))
                    throw new GraphicsOperationException($"Failed to get {KhrXcbSurface.ExtensionName} instance extension.");

                XcbSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XcbSurfaceCreateInfoKhr,
                    Connection = (IntPtr*) info.Display,
                    Window = info.Window
                };
                
                Instance.Log("Creating XCB surface.");
                _xcbSurface!.CreateXcbSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create XCB surface");
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        KhrSurface.DestroySurface(_instance, Surface, null);
        KhrSurface.Dispose();
        
        _xcbSurface?.Dispose();
        _xlibSurface?.Dispose();
        _waylandSurface?.Dispose();
        _win32Surface?.Dispose();
    }
}