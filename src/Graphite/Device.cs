namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Dispose();
}