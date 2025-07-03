namespace Graphite;

/// <summary>
/// Contains the underlying window manager handles for use when creating a <see cref="Surface"/>.
/// </summary>
public struct SurfaceInfo
{
    /// <summary>
    /// The window manager type.
    /// </summary>
    public SurfaceType Type;

    /// <summary>
    /// A pointer to the display.
    /// </summary>
    public nint Display;

    /// <summary>
    /// A pointer to the window/surface.
    /// </summary>
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