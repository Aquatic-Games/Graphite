namespace Graphite;

public struct BufferInfo
{
    public BufferUsage Usage;

    public uint SizeInBytes;

    public BufferInfo(BufferUsage usage, uint sizeInBytes)
    {
        Usage = usage;
        SizeInBytes = sizeInBytes;
    }
}