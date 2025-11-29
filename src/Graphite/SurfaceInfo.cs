namespace Graphite;

public record struct SurfaceInfo
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
}