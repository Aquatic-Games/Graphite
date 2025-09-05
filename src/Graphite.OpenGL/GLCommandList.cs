using Graphite.Core;
using Graphite.OpenGL.Instructions;

namespace Graphite.OpenGL;

internal sealed class GLCommandList : CommandList
{
    public readonly List<IInstruction> Instructions;

    public GLCommandList()
    {
        Instructions = [];
    }

    public override void Begin()
    {
        Instructions.Clear();
    }
    
    public override void End() { }
    
    public override void CopyBufferToBuffer(Buffer src, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void CopyBufferToTexture(Buffer src, uint srcOffset, Texture dest, Region3D? region = null)
    {
        throw new NotImplementedException();
    }
    
    public override void GenerateMipmaps(Texture texture)
    {
        throw new NotImplementedException();
    }
    
    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        Instructions.Add(new BeginRenderPassInstruction
        {
            ColorAttachments = Array.ConvertAll(colorAttachments.ToArray(), input => (GLTexture) input.Texture),
            ClearColor = colorAttachments[0].ClearColor
        });
    }
    
    public override void EndRenderPass() { }
    
    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        throw new NotImplementedException();
    }
    
    public override void SetDescriptorSet(uint slot, Pipeline pipeline, DescriptorSet set)
    {
        throw new NotImplementedException();
    }
    
    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void PushDescriptors(uint slot, Pipeline pipeline, params ReadOnlySpan<Descriptor> descriptors)
    {
        throw new NotImplementedException();
    }
    
    public override void Draw(uint numVertices, uint firstVertex = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void DrawIndexed(uint numIndices, uint firstIndex = 0, int baseVertex = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose() { }
}