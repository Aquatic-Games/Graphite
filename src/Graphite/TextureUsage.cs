namespace Graphite;

[Flags]
public enum TextureUsage
{
    None = 0,
    
    ShaderResource = 1 << 0,
    
    GenerateMips = 1 << 1
}