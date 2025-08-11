namespace Graphite.Core;

public struct Size3D : IEquatable<Size3D>
{
    public uint Width;

    public uint Height;

    public uint Depth;

    public Size3D(uint width, uint height, uint depth = 1)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }

    public Size3D(Size2D size, uint depth = 1)
    {
        Width = size.Width;
        Height = size.Height;
        Depth = depth;
    }

    /// <summary>
    /// Cast a <see cref="Size3D"/> to a <see cref="Size2D"/>. The <see cref="Depth"/> field will be dropped.
    /// </summary>
    public static explicit operator Size2D(Size3D size)
        => new Size2D(size.Width, size.Height);

    public bool Equals(Size3D other)
    {
        return Width == other.Width && Height == other.Height && Depth == other.Depth;
    }

    public override bool Equals(object? obj)
    {
        return obj is Size3D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height, Depth);
    }

    public static bool operator ==(Size3D left, Size3D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size3D left, Size3D right)
    {
        return !left.Equals(right);
    }
}