namespace Graphite;

[Flags]
public enum BufferUsage
{
    /// <summary>
    /// This buffer will not be used.
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
    TransferBuffer = 1 << 4
}