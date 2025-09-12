using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed unsafe class GLBuffer : Buffer
{
    private readonly GL _gl;

    public readonly uint Buffer;
    
    public GLBuffer(GL gl, ref readonly BufferInfo info, void* pData) : base(info)
    {
        _gl = gl;

        Buffer = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, Buffer);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, info.SizeInBytes, pData, BufferUsageARB.StaticDraw);
    }

    public override void Dispose()
    {
        _gl.DeleteBuffer(Buffer);
    }
}