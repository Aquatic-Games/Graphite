using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.Windows.Windows;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Swapchain : Swapchain
{
    private readonly IDXGISwapChain* _swapchain;
    private readonly Format _format;

    private Size2D _size;
    private D3D11Texture _swapchainTexture;

    public override Size2D Size => _size;

    public override Format Format => _format;

    public D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device1* device, ref readonly SwapchainInfo info)
    {
        _size = info.Size;
        _format = info.Format;
        
        DXGI_SWAP_CHAIN_DESC swapchainDesc = new()
        {
            OutputWindow = (HWND) ((D3D11Surface) info.Surface).HWND,
            Windowed = true,
            BufferCount = info.NumBuffers,
            BufferDesc = new DXGI_MODE_DESC { Width = _size.Width, Height = _size.Height, Format = _format.ToD3D() },
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
            SwapEffect = DXGI_SWAP_EFFECT_DISCARD
        };
        
        GraphiteLog.Log("Creating swapchain.");
        fixed (IDXGISwapChain** swapchain = &_swapchain)
            factory->CreateSwapChain((IUnknown*) device, &swapchainDesc, swapchain).Check("Create swapchain");

        ID3D11Texture2D* texture;
        _swapchain->GetBuffer(0, __uuidof<ID3D11Texture2D>(), (void**) &texture).Check("Get swapchain texture");

        _swapchainTexture = new D3D11Texture(device, texture, _size);
    }
    
    public override Texture GetNextTexture()
    {
        return _swapchainTexture;
    }
    
    public override void Present()
    {
        _swapchain->Present(1, 0);
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Releasing swapchain.");
        _swapchain->Release();
    }
}