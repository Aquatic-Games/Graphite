namespace Graphite;

public static class GraphiteUtils
{
    public static uint CalculateMipLevels(uint width, uint height)
    {
        return (uint) (double.Floor(double.Log2(double.Max(width, height))) + 1);
    }
}