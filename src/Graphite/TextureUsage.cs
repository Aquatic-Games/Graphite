namespace Graphite;

[Flags]
public enum TextureUsage
{
    None = 0,
    
    ShaderResource = 1 << 0,
    
    ColorTarget = 1 << 1,
    
    DepthStencilTarget = 1 << 2,
    
    GenerateMips = 1 << 16,
}