using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLPipeline : Pipeline
{
    private readonly GL _gl;
    
    public readonly uint VertexArray;

    public readonly uint ShaderProgram;

    public GLPipeline(GL gl, ref readonly GraphicsPipelineInfo info)
    {
        _gl = gl;

        VertexArray = _gl.GenVertexArray();
        _gl.BindVertexArray(VertexArray);

        for (int i = 0; i < info.InputLayout.Length; i++)
        {
            ref readonly InputElementDescription element = ref info.InputLayout[i];

            uint location = element.Location;
            uint offset = element.Offset;
            
            _gl.EnableVertexAttribArray(location);
            _gl.VertexAttribBinding(location, element.Slot);

            switch (element.Format)
            {
                case Format.R32_Float:
                    _gl.VertexAttribFormat(location, 1, VertexAttribType.Float, false, offset);
                    break;
                case Format.R32G32_Float:
                    _gl.VertexAttribFormat(location, 2, VertexAttribType.Float, false, offset);
                    break;
                case Format.R32G32B32_Float:
                    _gl.VertexAttribFormat(location, 3, VertexAttribType.Float, false, offset);
                    break;
                case Format.R32G32B32A32_Float:
                    _gl.VertexAttribFormat(location, 4, VertexAttribType.Float, false, offset);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        GLShaderModule vertexShader = (GLShaderModule) info.VertexShader;
        GLShaderModule pixelShader = (GLShaderModule) info.PixelShader;

        ShaderProgram = _gl.CreateProgram();
        _gl.AttachShader(ShaderProgram, vertexShader.Shader);
        _gl.AttachShader(ShaderProgram, pixelShader.Shader);
        
        _gl.LinkProgram(ShaderProgram);

        if (_gl.GetProgram(ShaderProgram, ProgramPropertyARB.LinkStatus) != (int) GLEnum.True)
            throw new Exception($"Failed to link program: {_gl.GetProgramInfoLog(ShaderProgram)}");
        
        _gl.DetachShader(ShaderProgram, pixelShader.Shader);
        _gl.DetachShader(ShaderProgram, vertexShader.Shader);
    }

    public override void Dispose()
    {
        _gl.DeleteProgram(ShaderProgram);
        _gl.DeleteVertexArray(VertexArray);
    }
}