namespace Graphite.OpenGL.Instructions;

internal struct DrawIndexedInstruction : IInstruction
{
    public uint NumIndices;

    public uint FirstIndex;

    public int BaseVertex;

    public DrawIndexedInstruction(uint numIndices, uint firstIndex, int baseVertex)
    {
        NumIndices = numIndices;
        FirstIndex = firstIndex;
        BaseVertex = baseVertex;
    }
}