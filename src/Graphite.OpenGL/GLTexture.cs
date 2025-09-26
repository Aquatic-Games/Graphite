using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLTexture : Texture
{
    private readonly GL _gl;
    
    public readonly uint Texture;

    public GLTexture(GL gl, uint texture, TextureInfo info) : base(info)
    {
        _gl = gl;
        Texture = texture;
    }
    
    public override void Dispose()
    {
        _gl.DeleteTexture(Texture);
    }
}