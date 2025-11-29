namespace Graphite;

/// <summary>
/// A surface that can be rendered to.
/// </summary>
public abstract class Surface : IDisposable
{
    /// <summary>
    /// Returns if this surface is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this surface.
    /// </summary>
    public abstract void Dispose();
}