namespace Graphite;

/// <summary>
/// Describes how a <see cref="Buffer"/> should be created.
/// </summary>
public struct BufferInfo
{
    /// <summary>
    /// How the buffer will be used.
    /// </summary>
    public BufferUsage Usage;

    /// <summary>
    /// The size, in bytes, of the buffer.
    /// </summary>
    public uint SizeInBytes;

    /// <summary>
    /// Create a new <see cref="BufferInfo"/>.
    /// </summary>
    /// <param name="usage">How the buffer will be used.</param>
    /// <param name="sizeInBytes">The size, in bytes, of the buffer.</param>
    public BufferInfo(BufferUsage usage, uint sizeInBytes)
    {
        Usage = usage;
        SizeInBytes = sizeInBytes;
    }
}