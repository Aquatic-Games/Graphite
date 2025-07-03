namespace Graphite;

/// <summary>
/// The surface of a window that can be rendered to with a <see cref="Swapchain"/>.
/// </summary>
public abstract class Surface : IDisposable
{
    /// <summary>
    /// Dispose of this <see cref="Surface"/>.
    /// </summary>
    public abstract void Dispose();
}