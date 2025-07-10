namespace Graphite.Core;

public struct Size2D : IEquatable<Size2D>
{
    public uint Width;

    public uint Height;

    public Size2D(uint width, uint height)
    {
        Width = width;
        Height = height;
    }

    public bool Equals(Size2D other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        return obj is Size2D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(Size2D left, Size2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size2D left, Size2D right)
    {
        return !left.Equals(right);
    }
}