namespace Graphite.OpenGL.Instructions;

public struct DrawInstruction : IInstruction
{
    public uint NumVertices;

    public uint FirstVertex;
}