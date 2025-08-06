using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
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
                    &featureLevel, 1, D3D11_SDK_VERSION, (ID3D11Device**) device, null, (ID3D11DeviceContext**) context)
                .Check("Create D3D11 device");
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(_device);
    }
    
    public override ShaderModule CreateShaderModule(byte[] code, string entryPoint, ShaderMappingInfo mapping = default)
    {
        throw new NotImplementedException();
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* data)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params ReadOnlySpan<Descriptor> descriptors)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override IntPtr MapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Releasing context.");
        _context->Release();
        GraphiteLog.Log("Releasing device.");
        _device->Release();
    }
}