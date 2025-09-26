using System.Text;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLShaderModule : ShaderModule
{
    private readonly GL _gl;
    
    public readonly uint Shader;
    
    public GLShaderModule(GL gl, ShaderStage stage, byte[] glsl)
    {
        _gl = gl;
        string sglsl = Encoding.UTF8.GetString(glsl);

        ShaderType type = stage switch
        {
            ShaderStage.Vertex => ShaderType.VertexShader,
            ShaderStage.Pixel => ShaderType.FragmentShader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        Shader = _gl.CreateShader(type);
        _gl.ShaderSource(Shader, sglsl);
        _gl.CompileShader(Shader);

        if (_gl.GetShader(Shader, ShaderParameterName.CompileStatus) != (int) GLEnum.True)
            throw new Exception($"Failed to compile {stage} shader: {_gl.GetShaderInfoLog(Shader)}");
    }

    public override void Dispose()
    {
        _gl.DeleteShader(Shader);
    }
}