using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Instance : Instance
{
    private readonly bool _debug;
    private readonly IDXGIFactory1* _factory;

    public override string BackendName => D3D11Backend.Name;

    public override Backend Backend => D3D11Backend.Backend;
    
    public D3D11Instance(ref readonly InstanceInfo info)
    {
        if (IsDXVK)
            ResolveLibrary += OnResolveLibrary;
        else if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "D3D11 is not supprted on non-Windows platforms unless the GRAPHITE_USE_DXVK environment variable is set.");
        }

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
            string name;
            
            // Work around a DXVK issue where GetDesc causes strange issues.
            // Might just be an issue with my machine, needs testing.
            if (IsDXVK)
            {
                name = $"Adapter {i + 1}";
            }
            else
            {
                DXGI_ADAPTER_DESC1 desc;
                adapter->GetDesc1(&desc).Check("Get adapter description");

                name = new string(&desc.Description.e0);
            }
            
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
    
    private static IntPtr OnResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        string newLibName = libraryName switch
        {
            "d3d11" => "libdxvk_d3d11",
            "dxgi" => "libdxvk_dxgi",
            "d3dcompiler_47" => "libvkd3d-utils",
            _ => libraryName
        };

        return NativeLibrary.Load(newLibName, assembly, searchPath);
    }
    
    internal static readonly bool IsDXVK;
    
    static D3D11Instance()
    {
        if (Environment.GetEnvironmentVariable("GRAPHITE_USE_DXVK") == "1")
            IsDXVK = true;
    }
}