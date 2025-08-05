namespace Graphite;

public abstract class CommandList : IDisposable
{
    /// <summary>
    /// Begin the command list. Commands can be issued after this point.
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// End the command list. No commands can be issued after this point, and it is ready for execution.
    /// </summary>
    public abstract void End();

    /// <summary>
    /// Copy a source <see cref="Buffer"/> to a destination <see cref="Buffer"/>.
    /// </summary>
    /// <param name="src">The source <see cref="Buffer"/>.</param>
    /// <param name="srcOffset">The offset into the source buffer, in bytes.</param>
    /// <param name="dest">The destination <see cref="Buffer"/>.</param>
    /// <param name="destOffset">The offset into the destination buffer, in bytes.</param>
    /// <param name="copySize">The size in bytes of the copy. If 0, the whole source buffer will be copied to the destination.</param>
    /// <remarks>This is a transfer operation, and cannot occur inside a render pass.</remarks>
    public abstract void CopyBufferToBuffer(Buffer src, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0);

    /// <summary>
    /// Begin a render pass with the given color attachments.
    /// </summary>
    /// <param name="colorAttachments">The color attachments that will be used in this render pass.</param>
    public abstract void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments);

    /// <summary>
    /// End the currently active render pass.
    /// </summary>
    public abstract void EndRenderPass();

    /// <summary>
    /// Set the graphics <see cref="Pipeline"/> that will be used on next draw.
    /// </summary>
    /// <param name="pipeline">The <see cref="Pipeline"/> to use.</param>
    public abstract void SetGraphicsPipeline(Pipeline pipeline);

    /// <summary>
    /// Set the graphics <see cref="DescriptorSet"/> that will be used on next draw.
    /// </summary>
    /// <param name="slot">The descriptor slot that it will be bound to.</param>
    /// <param name="pipeline">The graphics <see cref="Pipeline"/> to bind this descriptor set to.</param>
    /// <param name="set">The <see cref="DescriptorSet"/> to bind.</param>
    public abstract void SetDescriptorSet(uint slot, Pipeline pipeline, DescriptorSet set);

    /// <summary>
    /// Set the vertex <see cref="Buffer"/> that will be used on next draw.
    /// </summary>
    /// <param name="slot">The vertex buffer slot.</param>
    /// <param name="buffer">The <see cref="Buffer"/> to use.</param>
    /// <param name="stride">The size, in bytes, of a single vertex.</param>
    /// <param name="offset">The offset, in bytes, into the buffer.</param>
    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0);

    /// <summary>
    /// Set the index <see cref="Buffer"/> that will be used on next draw.
    /// </summary>
    /// <param name="buffer">The <see cref="Buffer"/> to use.</param>
    /// <param name="format">The <see cref="Format"/> of the buffer. Valid values are: <see cref="Format.R16_UInt"/> and <see cref="Format.R32_UInt"/>.</param>
    /// <param name="offset">The offset, in bytes, into the buffer.</param>
    public abstract void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0);
    
    /// <summary>
    /// Draw primitives.
    /// </summary>
    /// <param name="numVertices">The number of vertices.</param>
    /// <param name="firstVertex">The location of the first vertex.</param>
    public abstract void Draw(uint numVertices, uint firstVertex = 0);

    /// <summary>
    /// Draw indexed primitives.
    /// </summary>
    /// <param name="numIndices">The number of indices.</param>
    /// <param name="firstIndex">The location of the first index.</param>
    /// <param name="baseVertex">The value added to each index.</param>
    public abstract void DrawIndexed(uint numIndices, uint firstIndex = 0, int baseVertex = 0);
    
    /// <summary>
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}