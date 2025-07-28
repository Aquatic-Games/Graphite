namespace Graphite;

public abstract class Pipeline : IDisposable
{
    /// <summary>
    /// Dispose of this <see cref="Pipeline"/>.
    /// </summary>
    public abstract void Dispose();
}