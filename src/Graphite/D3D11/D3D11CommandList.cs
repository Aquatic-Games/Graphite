using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11CommandList : CommandList
{
    private readonly ID3D11DeviceContext1* _context;

    public ID3D11CommandList* CommandList;

    public D3D11CommandList(ID3D11Device1* device)
    {
        GraphiteLog.Log("Creating deferred context.");
        fixed (ID3D11DeviceContext1** context = &_context)
            device->CreateDeferredContext1(0, context).Check("Create deferred context");
    }
    
    public override void Begin()
    {
        if (CommandList == null)
            return;

        CommandList->Release();
        CommandList = null;
    }
    
    public override void End()
    {
        fixed (ID3D11CommandList** commandList = &CommandList)
            _context->FinishCommandList(false, commandList).Check("Finish command list");
    }
    
    public override void CopyBufferToBuffer(Buffer src, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        throw new NotImplementedException();
    }
    
    public override void EndRenderPass()
    {
        throw new NotImplementedException();
    }
    
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
    
    public override void Draw(uint numVertices, uint firstVertex = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void DrawIndexed(uint numIndices, uint firstIndex = 0, int baseVertex = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();
        
        GraphiteLog.Log("Releasing deferred context.");
        _context->Release();
    }
}