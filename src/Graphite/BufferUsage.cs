namespace Graphite;

/// <summary>
/// Contains flags describing how a <see cref="Buffer"/> may be used on the GPU.
/// </summary>
[Flags]
public enum BufferUsage
{
    /// <summary>
    /// This buffer will not be used. Its pure purpose is to waste GPU memory and make the other buffers jealous.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// This buffer will be used as a Vertex buffer.
    /// </summary>
    VertexBuffer = 1 << 0,
    
    /// <summary>
    /// This buffer will be used as an Index buffer.
    /// </summary>
    IndexBuffer = 1 << 1,
    
    /// <summary>
    /// This buffer will be used as a constant/uniform buffer.
    /// </summary>
    ConstantBuffer = 1 << 2,
    
    /// <summary>
    /// This buffer will be used as a structured/storage buffer.
    /// </summary>
    StructuredBuffer = 1 << 3,
    
    /// <summary>
    /// This buffer will be used as a transfer source buffer.
    /// </summary>
    /// <remarks>You do <b>NOT</b> need to provide any Map* flags, as Transfer buffers are inherently mappable.</remarks>
    TransferBuffer = 1 << 4,
    
    /// <summary>
    /// This buffer can be mapped into CPU-addressable memory for writing.
    /// </summary>
    MapWrite = 1 << 8
}