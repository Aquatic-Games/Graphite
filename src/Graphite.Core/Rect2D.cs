namespace Graphite.Core;

public struct Rect2D : IEquatable<Rect2D>
{
    public Offset2D Offset;

    public Size2D Size;

    public int X
    {
        get => Offset.X;
        set => Offset.X = value;
    }

    public int Y
    {
        get => Offset.Y;
        set => Offset.Y = value;
    }
    
    public uint Width
    {
        get => Size.Width;
        set => Size.Width = value;
    }

    public uint Height
    {
        get => Size.Height;
        set => Size.Height = value;
    }

    public Rect2D(Offset2D offset, Size2D size)
    {
        Offset = offset;
        Size = size;
    }

    public Rect2D(int x, int y, uint width, uint height)
    {
        Offset = new Offset2D(x, y);
        Size = new Size2D(width, height);
    }

    public bool Equals(Rect2D other)
    {
        return Offset.Equals(other.Offset) && Size.Equals(other.Size);
    }

    public override bool Equals(object? obj)
    {
        return obj is Rect2D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Size);
    }

    public static bool operator ==(Rect2D left, Rect2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rect2D left, Rect2D right)
    {
        return !left.Equals(right);
    }
}