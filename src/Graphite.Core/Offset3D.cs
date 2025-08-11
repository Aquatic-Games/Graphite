namespace Graphite.Core;

public struct Offset3D : IEquatable<Offset3D>
{
    public int X;

    public int Y;

    public int Z;

    public Offset3D(int x, int y, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(Offset3D other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is Offset3D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(Offset3D left, Offset3D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Offset3D left, Offset3D right)
    {
        return !left.Equals(right);
    }
}