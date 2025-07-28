namespace Graphite;

public ref struct GraphicsPipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<ColorTargetInfo> ColorTargets;

    public GraphicsPipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, in ReadOnlySpan<ColorTargetInfo> colorTargets)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorTargets = colorTargets;
    }
}