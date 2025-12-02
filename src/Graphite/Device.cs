namespace Graphite;

public abstract class Device : IDisposable
{
    /// <summary>
    /// Get if this device is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Create a <see cref="Swapchain"/> from the given info.
    /// </summary>
    /// <param name="info">The <see cref="SwapchainInfo"/> to use when creating the swapchain.</param>
    /// <returns>The created <see cref="Swapchain"/>.</returns>
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    /// <summary>
    /// Dispose of this device.
    /// </summary>
    public abstract void Dispose();
}