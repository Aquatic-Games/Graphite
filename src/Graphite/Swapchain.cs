namespace Graphite;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Get the next texture from the swap chain.
    /// </summary>
    /// <returns>The texture that can be rendered to.</returns>
    public abstract Texture GetNextTexture();
    
    /// <summary>
    /// Present to the surface.
    /// </summary>
    public abstract void Present();
    
    /// <summary>
    /// Dispose of this <see cref="Swapchain"/>.
    /// </summary>
    public abstract void Dispose();
}