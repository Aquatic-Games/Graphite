namespace Graphite;

/// <summary>
/// A region of GPU memory that can be used for various purposes.
/// </summary>
public abstract class Buffer : IDisposable
{
    public readonly BufferInfo Info;

    protected Buffer(BufferInfo info)
    {
        Info = info;
    }
    
    /// <summary>
    /// Dispose of this <see cref="Buffer"/>.
    /// </summary>
    public abstract void Dispose();
}