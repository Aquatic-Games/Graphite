namespace Graphite;

public readonly struct Adapter : IEquatable<Adapter>
{
    public readonly nint Handle;

    public readonly uint Index;

    public readonly string Name;

    public Adapter(IntPtr handle, uint index, string name)
    {
        Handle = handle;
        Index = index;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public bool Equals(Adapter other)
    {
        return Handle == other.Handle;
    }

    public override bool Equals(object? obj)
    {
        return obj is Adapter other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(Adapter left, Adapter right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Adapter left, Adapter right)
    {
        return !left.Equals(right);
    }
}