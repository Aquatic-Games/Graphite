using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Texture : Texture
{
    public readonly ID3D11DeviceChild* Texture;

    public readonly ID3D11RenderTargetView* RenderTarget;
    
    public D3D11Texture(ID3D11Device1* device, ID3D11Texture2D* swapchainTexture, Format format, Size2D size)
        : base(TextureInfo.Texture2D(format, size, 1, TextureUsage.None))
    {
        Texture = (ID3D11DeviceChild*) swapchainTexture;
        
        GraphiteLog.Log("Creating swapchain render target.");
        fixed (ID3D11RenderTargetView** renderTarget = &RenderTarget)
        {
            device->CreateRenderTargetView((ID3D11Resource*) Texture, null, renderTarget)
                .Check("Create swapchain render target");
        }
    }

    public override void Dispose()
    {
        if (RenderTarget != null)
            RenderTarget->Release();

        Texture->Release();
    }
}