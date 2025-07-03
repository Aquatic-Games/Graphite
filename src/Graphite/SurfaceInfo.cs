namespace Graphite;

public struct SurfaceInfo
{
    public SurfaceType Type;

    public nint Display;

    public nint Window;

    public SurfaceInfo(SurfaceType type, IntPtr display, IntPtr window)
    {
        Type = type;
        Display = display;
        Window = window;
    }

    public static SurfaceInfo Windows(nint hinstance, nint hwnd)
        => new SurfaceInfo(SurfaceType.Win32, hinstance, hwnd);

    public static SurfaceInfo Xlib(nint dpy, nint window)
        => new SurfaceInfo(SurfaceType.Xlib, dpy, window);

    public static SurfaceInfo Xcb(nint connection, nint window)
        => new SurfaceInfo(SurfaceType.Xcb, connection, window);

    public static SurfaceInfo Wayland(nint display, nint surface)
        => new SurfaceInfo(SurfaceType.Wayland, display, surface);
}