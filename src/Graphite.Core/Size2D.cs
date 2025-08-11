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
    
    /// <summary>
    /// Cast a <see cref="Size2D"/> to a <see cref="Size3D"/>. The <see cref="Size3D.Depth"/> field will be set to 1.
    /// </summary>
    public static explicit operator Size3D(Size2D size)
        => new Size3D(size);

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