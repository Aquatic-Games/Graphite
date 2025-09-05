namespace Graphite;

public struct Descriptor
{
    public uint Binding;

    public DescriptorType Type;
    
    public Buffer? Buffer;

    public Texture? Texture;

    public Sampler? Sampler;

    public uint BufferOffset;

    public uint BufferRange;

    public Descriptor(uint binding, DescriptorType type, Buffer? buffer = null, Texture? texture = null,
        Sampler? sampler = null, uint bufferOffset = 0, uint bufferRange = uint.MaxValue)
    {
        Binding = binding;
        Type = type;
        Buffer = buffer;
        Texture = texture;
        Sampler = sampler;
        BufferOffset = bufferOffset;
        BufferRange = bufferRange;
    }
}