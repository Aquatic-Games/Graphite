namespace Graphite;

public struct Descriptor
{
    public uint Binding;

    public DescriptorType Type;
    
    public Buffer? Buffer;

    public Texture? Texture;

    public uint BufferOffset;

    public uint BufferRange;

    public Descriptor(uint binding, DescriptorType type, Buffer? buffer = null, Texture? texture = null,
        uint bufferOffset = 0, uint bufferRange = uint.MaxValue)
    {
        Binding = binding;
        Type = type;
        Buffer = buffer;
        Texture = texture;
        BufferOffset = bufferOffset;
        BufferRange = bufferRange;
    }
}