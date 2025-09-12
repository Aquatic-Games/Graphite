namespace Graphite.OpenGL.Instructions;

internal struct SetVertexBufferInstruction : IInstruction
{
    public uint Slot;
    
    public GLBuffer Buffer;

    public uint Stride;

    public uint Offset;

    public SetVertexBufferInstruction(uint slot, GLBuffer buffer, uint stride, uint offset)
    {
        Slot = slot;
        Buffer = buffer;
        Stride = stride;
        Offset = offset;
    }
}