using Graphite.Core;

namespace Graphite;

public abstract class Texture : IDisposable
{
    public readonly TextureInfo Info;

    protected Texture(TextureInfo info)
    {
        Info = info;
    }
    
    public abstract void Dispose();
}