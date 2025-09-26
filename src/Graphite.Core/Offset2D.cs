namespace Graphite.Core;

public struct Offset2D : IEquatable<Offset2D>
{
    public int X;

    public int Y;

    public Offset2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Offset2D other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Offset2D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Offset2D left, Offset2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Offset2D left, Offset2D right)
    {
        return !left.Equals(right);
    }
}