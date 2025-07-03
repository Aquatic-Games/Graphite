namespace Graphite;

/// <summary>
/// Contains info about a physical graphics card or device present on the system.
/// </summary>
public readonly struct Adapter : IEquatable<Adapter>
{
    /// <summary>
    /// A pointer to the underlying device.
    /// </summary>
    public readonly nint Handle;

    /// <summary>
    /// The adapter's index.
    /// </summary>
    public readonly uint Index;

    /// <summary>
    /// The adapter name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Create a new <see cref="Adapter"/>.
    /// </summary>
    /// <param name="handle">A pointer to the underlying device.</param>
    /// <param name="index">The adapter's index.</param>
    /// <param name="name">The adapter name.</param>
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