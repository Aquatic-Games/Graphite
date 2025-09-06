namespace Graphite.OpenGL.Instructions;

internal struct SetIndexBufferInstruction : IInstruction
{
    public GLBuffer Buffer;

    public Format Format;

    public uint Offset;

    public SetIndexBufferInstruction(GLBuffer buffer, Format format, uint offset)
    {
        Buffer = buffer;
        Format = format;
        Offset = offset;
    }
}