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

    public override void CopyBufferToTexture(Buffer src, uint srcOffset, Texture dest, Region3D? region = null)
    {
        /*D3D11Buffer d3dSrc = (D3D11Buffer) src;
        D3D11Texture d3dDest = (D3D11Texture) dest;

        uint x = 0;
        uint y = 0;
        uint z = 0;

        uint rowPitch = d3dDest.Info.Format.Bpp() / 8;

        if (region is { } reg)
        {
            x = (uint) reg.X;
            y = (uint) reg.Y;
            z = (uint) reg.Z;
            rowPitch *= reg.Width;
        }
        else
            rowPitch *= d3dDest.Info.Size.Width;

        D3D11_BOX box = new D3D11_BOX((int) srcOffset, 0, 0, (int) (srcOffset + rowPitch), 1, 1);

        _context->CopySubresourceRegion((ID3D11Resource*) d3dDest.Texture, 0, x, y, z, (ID3D11Resource*) d3dSrc.Buffer,
            0, &box);*/

        // TODO: This doesn't work in D3D11. Perhaps staging textures should be implemented but I'm not a big fan of
        // that idea. Right now I don't really know the best way to implement this, but I don't want to remove this
        // functionality either.
        
        throw new NotImplementedException(
            "Copying between a buffer and texture is not supported in D3D11, and there is not yet a workaround in the D3D11 backend to make this work.");
    }

    public override void GenerateMipmaps(Texture texture)
    {
        D3D11Texture d3dTexture = (D3D11Texture) texture;
        _context->GenerateMips(d3dTexture.ResourceView);
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
        // TODO: Optimize this by only setting these values if the pipeline changes. Otherwise, return.
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;

        _context->VSSetShader(d3dPipeline.VertexShader, null, 0);
        _context->PSSetShader(d3dPipeline.PixelShader, null, 0);

        if (d3dPipeline.InputLayout != null)
            _context->IASetInputLayout(d3dPipeline.InputLayout);

        _context->OMSetBlendState(d3dPipeline.BlendState, null, uint.MaxValue);

        // TODO: Primitive topology
        _context->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    }
    
    public override void SetDescriptorSet(uint slot, Pipeline pipeline, DescriptorSet set)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;

        // TODO: I wonder if a lot of this can be cached?
        // TODO: Try and reduce code duplication somehow. I'd hate to have to duplicate this for 5 different shaders....
        
        PushDescriptorsToShader(slot, d3dPipeline, d3dSet.Layout, d3dSet.Descriptors);
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

    public override void PushDescriptors(uint slot, Pipeline pipeline, params ReadOnlySpan<Descriptor> descriptors)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        PushDescriptorsToShader(slot, d3dPipeline, d3dPipeline.DescriptorLayouts[slot], in descriptors);
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

    private void PushDescriptorsToShader(uint slot, D3D11Pipeline pipeline, D3D11DescriptorLayout layout, in ReadOnlySpan<Descriptor> descriptors)
    {
        foreach (Descriptor descriptor in descriptors)
        {
            ShaderStage stages = layout.Layout[descriptor.Binding].Stages;
            DescriptorType type = layout.Layout[descriptor.Binding].Type;

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

                    if ((stages & ShaderStage.Vertex) != 0)
                    {
                        Debug.Assert(pipeline.VertexDescriptors != null);
                        uint remappedSlot = pipeline.VertexDescriptors[slot][descriptor.Binding];
                        _context->VSSetConstantBuffers1(remappedSlot, 1, &buf, &offset, &range);
                    }

                    if ((stages & ShaderStage.Pixel) != 0)
                    {
                        Debug.Assert(pipeline.PixelDescriptors != null);
                        uint remappedSlot = pipeline.PixelDescriptors[slot][descriptor.Binding];
                        _context->PSSetConstantBuffers1(remappedSlot, 1, &buf, &offset, &range);
                    }

                    break;
                }
                
                case DescriptorType.Texture:
                {
                    Debug.Assert(descriptor.Texture != null);
                    Debug.Assert(descriptor.Sampler != null);

                    D3D11Texture texture = (D3D11Texture) descriptor.Texture;
                    D3D11Sampler sampler = (D3D11Sampler) descriptor.Sampler;

                    ID3D11ShaderResourceView* srv = texture.ResourceView;
                    ID3D11SamplerState* ss = sampler.Sampler;
                    
                    if ((stages & ShaderStage.Vertex) != 0)
                    {
                        Debug.Assert(pipeline.VertexDescriptors != null);
                        uint remappedSlot = pipeline.VertexDescriptors[slot][descriptor.Binding];
                        _context->VSSetShaderResources(remappedSlot, 1, &srv);
                        _context->VSSetSamplers(remappedSlot, 1, &ss);
                    }

                    if ((stages & ShaderStage.Pixel) != 0)
                    {
                        Debug.Assert(pipeline.PixelDescriptors != null);
                        uint remappedSlot = pipeline.PixelDescriptors[slot][descriptor.Binding];
                        _context->PSSetShaderResources(remappedSlot, 1, &srv);
                        _context->PSSetSamplers(remappedSlot, 1, &ss);
                    }
                    
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}