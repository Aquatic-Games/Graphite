namespace Graphite;

public struct BlendStateDescription
{
    public bool EnableBlending;

    public BlendFactor SrcColorFactor;

    public BlendFactor DestColorFactor;

    public BlendOp ColorBlendOp;

    public BlendFactor SrcAlphaFactor;

    public BlendFactor DestAlphaFactor;

    public BlendOp AlphaBlendOp;
    
    // TODO: ColorWriteMask

    public BlendStateDescription(bool enableBlending, BlendFactor srcColorFactor, BlendFactor destColorFactor,
        BlendOp colorBlendOp, BlendFactor srcAlphaFactor = BlendFactor.One,
        BlendFactor destAlphaFactor = BlendFactor.One, BlendOp alphaBlendOp = BlendOp.Add)
    {
        EnableBlending = enableBlending;
        SrcColorFactor = srcColorFactor;
        DestColorFactor = destColorFactor;
        ColorBlendOp = colorBlendOp;
        SrcAlphaFactor = srcAlphaFactor;
        DestAlphaFactor = destAlphaFactor;
        AlphaBlendOp = alphaBlendOp;
    }

    public static BlendStateDescription NoBlend =>
        new BlendStateDescription(false, BlendFactor.One, BlendFactor.One, BlendOp.Add);

    public static BlendStateDescription NonPremultipliedAlpha =>
        new BlendStateDescription(true, BlendFactor.SrcAlpha, BlendFactor.OneMinusSrcAlpha, BlendOp.Add);
}