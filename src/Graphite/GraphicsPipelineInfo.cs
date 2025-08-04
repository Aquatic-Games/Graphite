namespace Graphite;

public ref struct GraphicsPipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<ColorTargetInfo> ColorTargets;

    public ReadOnlySpan<InputElementDescription> InputLayout;

    public ReadOnlySpan<DescriptorLayout> Descriptors;

    public GraphicsPipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader,
        ReadOnlySpan<ColorTargetInfo> colorTargets, ReadOnlySpan<InputElementDescription> inputLayout,
        ReadOnlySpan<DescriptorLayout> descriptors)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorTargets = colorTargets;
        InputLayout = inputLayout;
        Descriptors = descriptors;
    }
}