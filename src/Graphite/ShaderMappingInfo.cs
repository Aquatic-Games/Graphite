namespace Graphite;

/// <summary>
/// Defines how the shader may be mapped to other backends.
/// </summary>
public struct ShaderMappingInfo
{
    public VertexInputMapping[]? VertexInput;
    
    public DescriptorMapping[]? Descriptors;

    public ShaderMappingInfo(VertexInputMapping[]? vertexInput = null, DescriptorMapping[]? descriptors = null)
    {
        VertexInput = vertexInput;
        Descriptors = descriptors;
    }
}