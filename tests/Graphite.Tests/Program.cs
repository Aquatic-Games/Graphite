using Graphite;
using Graphite.Core;
using SDL3;

GraphiteLog.LogMessage += (severity, type, message, _, _) =>
{
    if (severity == GraphiteLog.Severity.Error)
        throw new Exception(message);

    Console.WriteLine($"{severity} - {type}: {message}");
};

if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
    throw new Exception($"Failed to initialize SDL: {SDL.GetError()}");

const uint width = 1280;
const uint height = 720;

IntPtr window = SDL.CreateWindow("Graphite.Tests", (int) width, (int) height, 0);
if (window == IntPtr.Zero)
    throw new Exception($"Failed to create window: {SDL.GetError()}");

Instance instance = Instance.Create(new InstanceInfo("Graphite.Tests", true));
Console.WriteLine($"Adapters: {string.Join(", ", instance.EnumerateAdapters())}");

uint properties = SDL.GetWindowProperties(window);
SurfaceInfo surfaceInfo;

if (OperatingSystem.IsWindows())
{
    nint hinstance = SDL.GetPointerProperty(properties, SDL.Props.WindowWin32InstancePointer, 0);
    nint hwnd = SDL.GetPointerProperty(properties, SDL.Props.WindowWin32HWNDPointer, 0);
    surfaceInfo = SurfaceInfo.Windows(hinstance, hwnd);
}
else if (OperatingSystem.IsLinux())
{
    if (SDL.GetCurrentVideoDriver() == "wayland")
    {
        nint display = SDL.GetPointerProperty(properties, SDL.Props.WindowWaylandDisplayPointer, 0);
        nint wsurface = SDL.GetPointerProperty(properties, SDL.Props.WindowWaylandSurfacePointer, 0);
        surfaceInfo = SurfaceInfo.Wayland(display, wsurface);
    }
    else if (SDL.GetCurrentVideoDriver() == "x11")
    {
        nint display = SDL.GetPointerProperty(properties, SDL.Props.WindowX11DisplayPointer, 0);
        long xwindow = SDL.GetNumberProperty(properties, SDL.Props.WindowX11WindowNumber, 0);
        surfaceInfo = SurfaceInfo.Xlib(display, (nint) xwindow);
    }
    else
        throw new PlatformNotSupportedException();
}
else
    throw new PlatformNotSupportedException();

Surface surface = instance.CreateSurface(in surfaceInfo);

bool alive = true;
while (alive)
{
    while (SDL.PollEvent(out SDL.Event winEvent))
    {
        switch ((SDL.EventType) winEvent.Type)
        {
            case SDL.EventType.WindowCloseRequested:
                alive = false;
                break;
        }
    }
}

surface.Dispose();
instance.Dispose();
SDL.DestroyWindow(window);
SDL.Quit();