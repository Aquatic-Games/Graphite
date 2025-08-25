using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D_PRIMITIVE_TOPOLOGY;
using ColorF = Graphite.Core.ColorF;

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
        D3D11Buffer d3dSrc = (D3D11Buffer) src;
        D3D11Buffer d3dDest = (D3D11Buffer) dest;

        D3D11_BOX copyBox = new()
        {
            left = srcOffset,
            right = copySize == 0 ? d3dSrc.Info.SizeInBytes : (srcOffset + copySize),
            bottom = 1,
            back = 1
        };

        _context->CopySubresourceRegion((ID3D11Resource*) d3dDest.Buffer, 0, destOffset, 0, 0,
            (ID3D11Resource*) d3dSrc.Buffer, 0, &copyBox);
    }

    public override void CopyBufferToTexture(Buffer src, uint srcOffset, Texture dest, Size3D size,
        Offset3D offset = default)
    {
        throw new NotImplementedException();
    }

    public override void GenerateMipmaps(Texture texture)
    {
        throw new NotImplementedException();
    }

    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        ID3D11RenderTargetView** targets = stackalloc ID3D11RenderTargetView*[colorAttachments.Length];

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            D3D11Texture texture = (D3D11Texture) attachment.Texture;
            ColorF clearColor = attachment.ClearColor;
            
            targets[i] = texture.RenderTarget;

            if (attachment.LoadOp == LoadOp.Clear)
                _context->ClearRenderTargetView(texture.RenderTarget, (float*) &clearColor);
        }

        _context->OMSetRenderTargets((uint) colorAttachments.Length, targets, null);

        Size2D size = (Size2D) ((D3D11Texture) colorAttachments[0].Texture).Info.Size;

        D3D11_VIEWPORT viewport = new()
        {
            TopLeftX = 0,
            TopLeftY = 0,
            Width = size.Width,
            Height = size.Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        _context->RSSetViewports(1, &viewport);
    }

    public override void EndRenderPass()
    {
        // Do nothing
    }
    
    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;

        _context->VSSetShader(d3dPipeline.VertexShader, null, 0);
        _context->PSSetShader(d3dPipeline.PixelShader, null, 0);

        if (d3dPipeline.InputLayout != null)
            _context->IASetInputLayout(d3dPipeline.InputLayout);

        // TODO: Primitive topology
        _context->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    }
    
    public override void SetDescriptorSet(uint slot, Pipeline pipeline, DescriptorSet set)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;

        // TODO: I wonder if a lot of this can be cached?
        // TODO: Try and reduce code duplication somehow. I'd hate to have to duplicate this for 5 different shaders....
        
        foreach (Descriptor descriptor in d3dSet.Descriptors)
        {
            ShaderStage stages = d3dSet.Layout.Layout[descriptor.Binding].Stages;
            DescriptorType type = d3dSet.Layout.Layout[descriptor.Binding].Type;

            if ((stages & ShaderStage.Vertex) != 0)
            {
                Debug.Assert(d3dPipeline.VertexDescriptors != null);
                uint remappedSlot = d3dPipeline.VertexDescriptors[slot][descriptor.Binding];

                switch (type)
                {
                    case DescriptorType.ConstantBuffer:
                    {
                        Debug.Assert(descriptor.Buffer != null);
                        D3D11Buffer buffer = (D3D11Buffer) descriptor.Buffer;
                        uint offset = descriptor.BufferOffset;
                        uint range = descriptor.BufferRange == uint.MaxValue
                            ? buffer.Info.SizeInBytes : descriptor.BufferRange;
                        ID3D11Buffer* buf = buffer.Buffer;
                        _context->VSSetConstantBuffers1(remappedSlot, 1, &buf, &offset, &range);
                        
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if ((stages & ShaderStage.Pixel) != 0)
            {
                Debug.Assert(d3dPipeline.PixelDescriptors != null);
                uint remappedSlot = d3dPipeline.PixelDescriptors[slot][descriptor.Binding];
                
                switch (type)
                {
                    case DescriptorType.ConstantBuffer:
                    {
                        Debug.Assert(descriptor.Buffer != null);
                        D3D11Buffer buffer = (D3D11Buffer) descriptor.Buffer;
                        uint offset = descriptor.BufferOffset;
                        uint range = descriptor.BufferRange == uint.MaxValue
                            ? buffer.Info.SizeInBytes : descriptor.BufferRange;
                        ID3D11Buffer* buf = buffer.Buffer;
                        _context->PSSetConstantBuffers1(remappedSlot, 1, &buf, &offset, &range);
                        
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        ID3D11Buffer* buf = d3dBuffer.Buffer;
        _context->IASetVertexBuffers(slot, 1, &buf, &stride, &offset);
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        _context->IASetIndexBuffer(d3dBuffer.Buffer, format.ToD3D(), offset);
    }
    
    public override void Draw(uint numVertices, uint firstVertex = 0)
    {
        _context->Draw(numVertices, firstVertex);
    }
    
    public override void DrawIndexed(uint numIndices, uint firstIndex = 0, int baseVertex = 0)
    {
        _context->DrawIndexed(numIndices, firstIndex, baseVertex);
    }
    
    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();
        
        GraphiteLog.Log("Releasing deferred context.");
        _context->Release();
    }
}