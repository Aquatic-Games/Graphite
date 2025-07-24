using Graphite.Core;

namespace Graphite;

public abstract class Texture : IDisposable
{
    public readonly Size2D Size;

    protected Texture(Size2D size)
    {
        Size = size;
    }
    
    public abstract void Dispose();
}