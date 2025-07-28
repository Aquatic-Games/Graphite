using Graphite.Vulkan;

namespace Graphite;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void CopyBufferToBuffer(Buffer src, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0);

    public abstract void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments);

    public abstract void EndRenderPass();

    public abstract void SetGraphicsPipeline(Pipeline pipeline);

    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0);

    public abstract void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0);
    
    public abstract void Draw(uint numVertices);

    public abstract void DrawIndexed(uint numIndices);
    
    /// <summary>
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}