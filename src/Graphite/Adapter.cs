namespace Graphite;

public readonly record struct Adapter
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
}