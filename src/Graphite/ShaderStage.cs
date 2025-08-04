namespace Graphite;

[Flags]
public enum ShaderStage
{
    /// <summary>
    /// No shader stage.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Vertex shader.
    /// </summary>
    Vertex = 1 << 0,
    
    /// <summary>
    /// Pixel shader.
    /// </summary>
    Pixel = 1 << 1,
    
    /// <summary>
    /// Vertex and Pixel shaders.
    /// </summary>
    VertexPixel = Vertex | Pixel
}