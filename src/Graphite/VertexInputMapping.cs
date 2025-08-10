namespace Graphite;

public struct VertexInputMapping
{
    public Semantic Semantic;
    
    public uint Index;

    public VertexInputMapping(Semantic semantic, uint index)
    {
        Semantic = semantic;
        Index = index;
    }
}