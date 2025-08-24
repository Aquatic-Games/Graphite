using System.Diagnostics;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLInstance : Instance
{
    private readonly GLContext _context;
    private readonly GL _gl;
    
    public override Backend Backend => Backend.OpenGL;

    public GLInstance(ref readonly InstanceInfo info)
    {
        Debug.Assert(info.GLContext != null);
        _context = info.GLContext;
        
        _gl = GL.GetApi(_context.GetProcAddressFunc);
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        string name = _gl.GetStringS(StringName.Renderer);
        return [new Adapter(0, 0, name)];
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new GLSurface();
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