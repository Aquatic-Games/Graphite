namespace Graphite;

public struct Descriptor
{
    public uint Binding;

    public DescriptorType Type;
    
    public Buffer? Buffer;

    public uint BufferOffset;

    public uint BufferRange;

    public Descriptor(uint binding, DescriptorType type, Buffer? buffer = null, uint bufferOffset = 0,
        uint bufferRange = uint.MaxValue)
    {
        Binding = binding;
        Type = type;
        Buffer = buffer;
        BufferOffset = bufferOffset;
        BufferRange = bufferRange;
    }
}