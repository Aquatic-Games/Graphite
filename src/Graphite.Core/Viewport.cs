namespace Graphite.Core;

public struct Viewport
{
    public float X;

    public float Y;

    public float Width;

    public float Height;

    public float MinDepth;

    public float MaxDepth;

    public Viewport(float x, float y, float width, float height, float minDepth = 0, float maxDepth = 1)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
    }
}