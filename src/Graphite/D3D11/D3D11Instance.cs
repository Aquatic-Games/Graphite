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
    private readonly bool _debug;
    private readonly IDXGIFactory1* _factory;

    public override Backend Backend => Backend.D3D11;
    
    public D3D11Instance(ref readonly InstanceInfo info)
    {
        _debug = info.Debug;
        
        GraphiteLog.Log("Creating DXGI factory.");
        fixed (IDXGIFactory1** factory = &_factory)
            CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory).Check("Create DXGI factory");
    }

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = [];
        
        IDXGIAdapter1* adapter;
        for (uint i = 0; _factory->EnumAdapters1(i, &adapter).SUCCEEDED; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            adapter->GetDesc1(&desc).Check("Get adapter description");

            string name = new string(&desc.Description.e0);
            
            adapters.Add(new Adapter((nint) adapter, i, name));
        }

        return adapters.ToArray();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new D3D11Surface(in info);
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        IDXGIAdapter1* dxgiAdapter;

        if (adapter is { } adp)
            dxgiAdapter = (IDXGIAdapter1*) adp.Handle;
        else
        {
            Adapter[] adapters = EnumerateAdapters();
            dxgiAdapter = (IDXGIAdapter1*) adapters[0].Handle;
        }

        return new D3D11Device(_factory, dxgiAdapter, _debug);
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Releasing factory.");
        _factory->Release();
    }
}