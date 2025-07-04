using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSurface : Surface
{
    private readonly VkInstance _instance;
    
    private readonly KhrWin32Surface? _win32Surface;
    private readonly KhrXlibSurface? _xlibSurface;
    private readonly KhrXcbSurface? _xcbSurface;
    private readonly KhrWaylandSurface? _waylandSurface;

    public readonly KhrSurface SurfaceExt;
    public readonly SurfaceKHR Surface;
    
    public VulkanSurface(Vk vk, VkInstance instance, ref readonly SurfaceInfo info)
    {
        _instance = instance;

        if (!vk.TryGetInstanceExtension(_instance, out SurfaceExt))
            throw new Exception("Failed to get surface extension.");

        switch (info.Type)
        {
            case SurfaceType.Win32:
            {
                if (!vk.TryGetInstanceExtension(_instance, out _win32Surface))
                    throw new Exception("Failed to get Win32 surface extension.");
                
                Win32SurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.Win32SurfaceCreateInfoKhr,
                    Hinstance = info.Display,
                    Hwnd = info.Window
                };

                GraphiteLog.Log("Creating Win32 surface.");
                _win32Surface!.CreateWin32Surface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create Win32 surface");
                    
                break;
            }
            
            case SurfaceType.Xlib:
            {
                if (!vk.TryGetInstanceExtension(_instance, out _xlibSurface))
                    throw new Exception("Failed to get XLib surface extension.");
                
                XlibSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XlibSurfaceCreateInfoKhr,
                    Dpy = (nint*) info.Display,
                    Window = info.Window
                };

                GraphiteLog.Log("Creating XLib surface.");
                _xlibSurface!.CreateXlibSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create XLib surface");
                    
                break;
            }
            
            case SurfaceType.Xcb:
            {
                if (!vk.TryGetInstanceExtension(_instance, out _xcbSurface))
                    throw new Exception("Failed to get XCB surface extension.");
                
                XcbSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XcbSurfaceCreateInfoKhr,
                    Connection = (nint*) info.Display,
                    Window = info.Window
                };

                GraphiteLog.Log("Creating XCB surface.");
                _xcbSurface!.CreateXcbSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create XCB surface");
                    
                break;
            }
            
            case SurfaceType.Wayland:
            {
                if (!vk.TryGetInstanceExtension(_instance, out _waylandSurface))
                    throw new Exception("Failed to get Wayland surface extension.");
                
                WaylandSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.WaylandSurfaceCreateInfoKhr,
                    Display = (nint*) info.Display,
                    Surface = (nint*) info.Window
                };

                GraphiteLog.Log("Creating Wayland surface.");
                _waylandSurface!.CreateWaylandSurface(_instance, &surfaceInfo, null, out Surface)
                    .Check("Create Wayland surface");
                    
                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying surface.");
        SurfaceExt.DestroySurface(_instance, Surface, null);
        
        _waylandSurface?.Dispose();
        _xcbSurface?.Dispose();
        _xlibSurface?.Dispose();
        _win32Surface?.Dispose();
        
        SurfaceExt.Dispose();
    }
}