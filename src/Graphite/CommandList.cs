using Graphite.Vulkan;

namespace Graphite;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments);

    public abstract void EndRenderPass();

    public abstract void SetGraphicsPipeline(Pipeline pipeline);

    public abstract void Draw(uint numVertices);
    
    /// <summary>
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}