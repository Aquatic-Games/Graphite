namespace Graphite;

/// <summary>
/// Contains various API backends that Graphite recognizes. This enum may be used to handle implementation differences.
/// </summary>
public enum Backend
{
    /// <summary>
    /// Other/Unknown. Usually this will be a custom backend, either not provided by Graphite, or one that is not
    /// publicly available.
    /// </summary>
    Other,
    
    /// <summary>
    /// Vulkan 1.3
    /// </summary>
    Vulkan,
    
    /// <summary>
    /// DirectX 12 Ultimate
    /// </summary>
    D3D12,
    
    /// <summary>
    /// DirectX 11
    /// </summary>
    D3D11,
    
    /// <summary>
    /// OpenGL 4.3
    /// </summary>
    OpenGL
}