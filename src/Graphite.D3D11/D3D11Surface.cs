namespace Graphite.D3D11;

internal sealed class D3D11Surface : Surface
{
    public readonly nint HWND;

    public D3D11Surface(ref readonly SurfaceInfo info)
    {
        HWND = info.Window;
    }
    
    public override void Dispose()
    {
        // Nothing to dispose
    }
}