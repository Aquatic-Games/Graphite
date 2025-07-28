using Graphite.Core;

namespace Graphite;

public struct ColorAttachmentInfo
{
    public Texture Texture;

    public ColorF ClearColor;

    public LoadOp LoadOp;

    public StoreOp StoreOp;

    public ColorAttachmentInfo(Texture texture, ColorF clearColor, LoadOp loadOp = LoadOp.Clear, StoreOp storeOp = StoreOp.Store)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
        StoreOp = storeOp;
    }

    public ColorAttachmentInfo(Texture texture, LoadOp loadOp = LoadOp.Load, StoreOp storeOp = StoreOp.Store)
    {
        Texture = texture;
        ClearColor = new ColorF();
        LoadOp = loadOp;
        StoreOp = storeOp;
    }
}