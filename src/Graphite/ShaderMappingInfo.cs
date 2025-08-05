namespace Graphite;

/// <summary>
/// Defines how the shader may be mapped to other backends.
/// </summary>
public ref struct ShaderMappingInfo
{
    public ReadOnlySpan<VertexInputMapping> VertexInput;
    
    public ReadOnlySpan<DescriptorMapping> Descriptors;

    public ShaderMappingInfo(ReadOnlySpan<VertexInputMapping> vertexInput, ReadOnlySpan<DescriptorMapping> descriptors)
    {
        VertexInput = vertexInput;
        Descriptors = descriptors;
    }
}