using Graphite.Core;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLSwapchain : Swapchain
{
    private readonly GL _gl;
    private readonly GLContext _context;
    private readonly uint _vao;
    private readonly uint _program;

    private GLTexture _texture;
    
    public override Size2D Size { get; }
    
    public override Format Format { get; }

    public GLSwapchain(GL gl, GLContext context, ref readonly SwapchainInfo info)
    {
        _gl = gl;
        _context = context;

        Size = info.Size;
        Format = info.Format;

        (_, SizedInternalFormat iFormat, _) = info.Format.ToGL();

        uint texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, texture);
        _gl.TexStorage2D(TextureTarget.Texture2D, 1, iFormat, info.Size.Width, info.Size.Height);

        _texture = new GLTexture(_gl, texture, TextureInfo.Texture2D(info.Format, info.Size, 1, TextureUsage.None));

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        
        uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, VertexShader);
        _gl.CompileShader(vertexShader);
        _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to compile vertex shader: {_gl.GetShaderInfoLog(vertexShader)}");

        uint fragmentShader = _gl.CreateShader(GLEnum.FragmentShader);
        _gl.ShaderSource(fragmentShader, FragmentShader);
        _gl.CompileShader(fragmentShader);
        _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to compile fragment shader: {_gl.GetShaderInfoLog(fragmentShader)}");

        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);
        
        _gl.LinkProgram(_program);
        _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to link program: {_gl.GetProgramInfoLog(_program)}");
        
        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }
    
    public override Texture GetNextTexture()
    {
        return _texture;
    }
    
    public override void Present()
    {
        _gl.BindVertexArray(_vao);
        
        _gl.UseProgram(_program);
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture.Texture);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        
        _context.PresentFunc(1);
    }

    public override void Dispose()
    {
        _texture.Dispose();
    }

    private const string VertexShader = """
                                        #version 330 core

                                        out vec2 frag_TexCoord;

                                        // XY = Position, ZW = TexCoord
                                        const vec4 vertices[4] = vec4[]
                                        (
                                            vec4(-1.0, -1.0,   0.0, 0.0),
                                            vec4(-1.0,  1.0,   0.0, 1.0),
                                            vec4( 1.0,  1.0,   1.0, 1.0),
                                            vec4( 1.0, -1.0,   1.0, 0.0)
                                        );

                                        const int indices[6] = int[]
                                        (
                                            0, 1, 3,
                                            1, 2, 3
                                        );

                                        void main()
                                        {
                                            vec4 vertex = vertices[indices[gl_VertexID]];
                                            gl_Position = vec4(vertex.xy, 0.0, 1.0);
                                            frag_TexCoord = vertex.zw;
                                        }
                                        """;

    private const string FragmentShader = """
                                          #version 330 core

                                          in vec2 frag_TexCoord;

                                          out vec4 out_Color;

                                          uniform sampler2D uTexture;

                                          void main()
                                          {
                                              out_Color = texture(uTexture, frag_TexCoord);
                                              //out_Color = vec4(frag_TexCoord, 0.0, 1.0);
                                          }
                                          """;
}