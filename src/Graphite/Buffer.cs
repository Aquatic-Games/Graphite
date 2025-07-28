namespace Graphite;

public abstract class Buffer : IDisposable
{
    /// <summary>
    /// Dispose of this <see cref="Buffer"/>.
    /// </summary>
    public abstract void Dispose();
}