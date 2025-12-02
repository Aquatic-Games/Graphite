namespace Graphite;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Get if this swapchain is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this swapchain.
    /// </summary>
    public abstract void Dispose();
}