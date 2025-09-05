using Graphite.Core;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLSwapchain : Swapchain
{
    private readonly GL _gl;
    private readonly GLContext _context;

    private uint _texture;
    
    public override Size2D Size { get; }
    
    public override Format Format { get; }

    public GLSwapchain(GL gl, GLContext context, ref readonly SwapchainInfo info)
    {
        _gl = gl;
        _context = context;

        Size = info.Size;
        Format = info.Format;

        (_, SizedInternalFormat iFormat, _) = GLUtils.ToGL(info.Format);

        _texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.TexStorage2D(TextureTarget.Texture2D, 1, iFormat, info.Size.Width, info.Size.Height);
    }
    
    public override Texture GetNextTexture()
    {
        throw new NotImplementedException();
    }
    
    public override void Present()
    {
        _context.PresentFunc(1);
    }
    
    public override void Dispose() { }
}