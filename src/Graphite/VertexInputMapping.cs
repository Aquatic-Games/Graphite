namespace Graphite;

public struct VertexInputMapping
{
    public uint Index;

    public uint Semantic;

    public VertexInputMapping(uint index, uint semantic)
    {
        Index = index;
        Semantic = semantic;
    }
}