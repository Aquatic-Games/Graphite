using Graphite.Core;

namespace Graphite;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// The size, in pixels, of the swapchain.
    /// </summary>
    /// <remarks>This may not necessarily be the same size as the <see cref="Surface"/> attached to it.</remarks>
    public abstract Size2D Size { get; }
    
    /// <summary>
    /// The <see cref="Graphite.Format"/> of the swapchain.
    /// </summary>
    public abstract Format Format { get; }
    
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