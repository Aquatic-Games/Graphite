using Graphite.Core;

namespace Graphite;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Get if this swapchain is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The size, in pixels, of the swapchain.
    /// </summary>
    public abstract Size2D Size { get; }
    
    /// <summary>
    /// The <see cref="Graphite.Format"/> of the swapchain.
    /// </summary>
    public abstract Format Format { get; }
    
    /// <summary>
    /// The swapchain's <see cref="Graphite.PresentMode"/>
    /// </summary>
    public abstract PresentMode PresentMode { get; }

    /// <summary>
    /// Gets the next <see cref="Texture"/> in the swap chain to render to.
    /// </summary>
    /// <returns>The next texture.</returns>
    public abstract Texture GetNextTexture();

    /// <summary>
    /// Present to the swapchain's surface.
    /// </summary>
    public abstract void Present();

    /// <summary>
    /// Dispose of this swapchain.
    /// </summary>
    public abstract void Dispose();
}