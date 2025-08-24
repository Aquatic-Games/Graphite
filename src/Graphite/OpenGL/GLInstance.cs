using System.Diagnostics;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLInstance : Instance
{
    private readonly GL _gl;
    
    public override Backend Backend => Backend.OpenGL;

    public GLInstance(ref readonly InstanceInfo info)
    {
        Debug.Assert(info.GLGetProcAddressFunc != null);
        _gl = GL.GetApi(info.GLGetProcAddressFunc);
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        _gl.Dispose();
    }
}