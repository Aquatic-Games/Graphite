using System.Diagnostics;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLInstance : Instance
{
    private readonly GLContext _context;
    private readonly GL _gl;

    public override string BackendName => OpenGLBackend.Name;
    
    public override Backend Backend => OpenGLBackend.Backend;

    public GLInstance(ref readonly InstanceInfo info)
    {
        Debug.Assert(OpenGLBackend.Context != null, "OpenGLBackend.Context was null. This value must be set to use the OpenGL backend.");
        _context = OpenGLBackend.Context;
        
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
        return new GLDevice(_gl, _context);
    }
    
    public override void Dispose()
    {
        _gl.Dispose();
    }
}