using Silk.NET.SDL;

namespace Graphite.Vulkan.Tests;

public unsafe class Program
{
    public static void Main(string[] args)
    {
        Instance.LogMessage += (severity, message, line, path) => Console.WriteLine($"[{severity}] {message}");

        Sdl sdl = Sdl.GetApi();

        if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

        const int width = 1280;
        const int height = 720;

        Window* window =
            sdl.CreateWindow("Graphite.Vulkan.Tests", Sdl.WindowposCentered, Sdl.WindowposCentered, width, height, 0);
        if (window == null)
            throw new Exception($"Failed to create window: {sdl.GetErrorS()}");

        InstanceInfo instanceInfo = new InstanceInfo("Graphite.Vulkan.Tests");
        Instance instance = new VulkanInstance(in instanceInfo);

        Adapter[] adapters = instance.EnumerateAdapters();
        foreach (Adapter adapter in adapters)
            Console.WriteLine(adapter);
        
        SysWMInfo wmInfo = new();
        sdl.GetWindowWMInfo(window, &wmInfo);

        SurfaceInfo surfaceInfo = wmInfo.Subsystem switch
        {
            SysWMType.Windows => new SurfaceInfo(SurfaceType.Win32, wmInfo.Info.Win.HInstance, wmInfo.Info.Win.Hwnd),
            SysWMType.Wayland => new SurfaceInfo(SurfaceType.Wayland, (nint) wmInfo.Info.Wayland.Display, (nint) wmInfo.Info.Wayland.Surface),
            SysWMType.X11 => new SurfaceInfo(SurfaceType.Xlib, (nint) wmInfo.Info.X11.Display, (nint) wmInfo.Info.X11.Window),
            _ => throw new PlatformNotSupportedException()
        };

        Surface surface = instance.CreateSurface(in surfaceInfo);
        Device device = instance.CreateDevice(surface);
        
        device.Dispose();
        surface.Dispose();
        instance.Dispose();
        sdl.DestroyWindow(window);
        sdl.Quit();
        sdl.Dispose();
    }
}