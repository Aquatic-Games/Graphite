using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.DirectX.D3D11_MAP;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.DirectX;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Device : Device
{
    private readonly IDXGIFactory1* _factory;
    private readonly ID3D11Device1* _device;
    private readonly ID3D11DeviceContext1* _context;
    
    public override Backend Backend => Backend.D3D11;
    
    public D3D11Device(IDXGIFactory1* factory, IDXGIAdapter1* adapter, bool debug)
    {
        _factory = factory;

        D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        if (debug)
            flags |= D3D11_CREATE_DEVICE_DEBUG;

        D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL_11_1;
        
        GraphiteLog.Log("Creating D3D11 device.");
        fixed (ID3D11Device1** device = &_device)
        fixed (ID3D11DeviceContext1** context = &_context)
            D3D11CreateDevice((IDXGIAdapter*) adapter, D3D_DRIVER_TYPE_UNKNOWN, HMODULE.NULL, (uint) flags,
                &featureLevel, 1, D3D11_SDK_VERSION, (ID3D11Device**) device, null, (ID3D11DeviceContext**) context).Check("Create D3D11 device");
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new D3D11Swapchain(_factory, _device, in info);
    }
    
    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(_device);
    }
    
    public override ShaderModule CreateShaderModule(byte[] code, string entryPoint, ShaderMappingInfo mapping = default)
    {
        return new D3D11ShaderModule(code, in mapping);
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new D3D11Pipeline(_device, in info);
    }
    
    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* data)
    {
        return new D3D11Buffer(_device, in info, data);
    }

    public override Texture CreateTexture(in TextureInfo info, void* pData)
    {
        return new D3D11Texture(_device, _context, in info, pData);
    }

    public override DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings)
    {
        return new D3D11DescriptorLayout(bindings);
    }
    
    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params ReadOnlySpan<Descriptor> descriptors)
    {
        return new D3D11DescriptorSet(layout, descriptors);
    }

    public override Sampler CreateSampler(in SamplerInfo info)
    {
        return new D3D11Sampler(_device, in info);
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        D3D11CommandList d3dCommandList = (D3D11CommandList) cl;
        _context->ExecuteCommandList(d3dCommandList.CommandList, false);
    }

    public override void UpdateBuffer(Buffer buffer, uint offset, uint size, void* pData)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, in Region3D region, void* pData)
    {
        throw new NotImplementedException();
    }

    public override nint MapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        
        D3D11_MAPPED_SUBRESOURCE mapped;
        // TODO: Store the MapWrite/MapRead flags in the buffer so they can be used appropriately here.
        _context->Map((ID3D11Resource*) d3dBuffer.Buffer, 0, d3dBuffer.MapType, 0, &mapped).Check("Map buffer");

        return (nint) mapped.pData;
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        _context->Unmap((ID3D11Resource*) d3dBuffer.Buffer, 0);
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Releasing context.");
        _context->Release();
        GraphiteLog.Log("Releasing device.");
        _device->Release();
    }
}