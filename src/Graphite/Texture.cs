namespace Graphite;

public abstract class Texture : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Dispose();
}