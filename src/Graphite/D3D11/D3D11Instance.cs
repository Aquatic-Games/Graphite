using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Instance : Instance
{
    private readonly IDXGIFactory1* _factory;

    public override Backend Backend => Backend.D3D11;
    
    public D3D11Instance(ref readonly InstanceInfo info)
    {
        GraphiteLog.Log("Creating DXGI factory.");
        fixed (IDXGIFactory1** factory = &_factory)
            CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory).Check("Create DXGI factory");
    }

    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        _factory->Release();
    }
}