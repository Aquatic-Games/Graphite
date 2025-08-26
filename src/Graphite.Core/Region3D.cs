namespace Graphite.Core;

public struct Region3D : IEquatable<Region3D>
{
    public Offset3D Offset;

    public Size3D Size;

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

    public int Z
    {
        get => Offset.Z;
        set => Offset.Z = value;
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

    public uint Depth
    {
        get => Size.Depth;
        set => Size.Depth = value;
    }

    public Region3D(Offset3D offset, Size3D size)
    {
        Offset = offset;
        Size = size;
    }

    public Region3D(int x, int y, int z, uint width, uint height, uint depth)
    {
        Offset = new Offset3D(x, y, z);
        Size = new Size3D(width, height, depth);
    }

    public bool Equals(Region3D other)
    {
        return Offset.Equals(other.Offset) && Size.Equals(other.Size);
    }

    public override bool Equals(object? obj)
    {
        return obj is Region3D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Size);
    }

    public static bool operator ==(Region3D left, Region3D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Region3D left, Region3D right)
    {
        return !left.Equals(right);
    }
}