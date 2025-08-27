using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D_SRV_DIMENSION;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_RESOURCE_MISC_FLAG;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Texture : Texture
{
    public readonly ID3D11DeviceChild* Texture;

    public readonly ID3D11ShaderResourceView* ResourceView;

    public readonly ID3D11RenderTargetView* RenderTarget;

    public D3D11Texture(ID3D11Device1* device, ID3D11DeviceContext1* context, ref readonly TextureInfo info,
        void* pData) : base(info)
    {
        D3D11_BIND_FLAG bindFlags = 0;
        D3D11_RESOURCE_MISC_FLAG miscFlags = 0;
        
        if ((info.Usage & TextureUsage.ShaderResource) != 0)
            bindFlags |= D3D11_BIND_SHADER_RESOURCE;
        if ((info.Usage & TextureUsage.GenerateMips) != 0)
        {
            bindFlags |= D3D11_BIND_RENDER_TARGET;
            miscFlags |= D3D11_RESOURCE_MISC_GENERATE_MIPS;
        }

        DXGI_FORMAT format = info.Format.ToD3D();

        D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc = new()
        {
            Format = format
        };

        switch (info.Type)
        {
            case TextureType.Texture2D:
            {
                D3D11_TEXTURE2D_DESC textureDesc = new()
                {
                    Format = format,
                    Width = info.Size.Width,
                    Height = info.Size.Height,
                    MipLevels = info.MipLevels,
                    ArraySize = info.ArraySize,
                    BindFlags = (uint) bindFlags,
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
                    MiscFlags = (uint) miscFlags
                };

                GraphiteLog.Log("Creating Texture2D.");
                fixed (ID3D11DeviceChild** pTexture = &Texture)
                    device->CreateTexture2D(&textureDesc, null, (ID3D11Texture2D**) pTexture).Check("Create Texture2D");
                
                viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
                viewDesc.Texture2D = new D3D11_TEX2D_SRV
                {
                    MipLevels = uint.MaxValue,
                    MostDetailedMip = 0
                };
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        if ((info.Usage & TextureUsage.ShaderResource) != 0)
        {
            GraphiteLog.Log("Creating Shader Resource View.");
            fixed (ID3D11ShaderResourceView** resourceView = &ResourceView)
            {
                device->CreateShaderResourceView((ID3D11Resource*) Texture, &viewDesc, resourceView)
                    .Check("Create shader resource view");
            }
        }

        if (pData == null)
            return;

        uint rowPitch = info.Size.Width * info.Format.Bpp() / 8;
        context->UpdateSubresource((ID3D11Resource*) Texture, 0, null, pData, rowPitch, 0);
    }
    
    public D3D11Texture(ID3D11Device1* device, ID3D11Texture2D* swapchainTexture, Format format, Size2D size)
        : base(TextureInfo.Texture2D(format, size, 1, TextureUsage.None))
    {
        Texture = (ID3D11DeviceChild*) swapchainTexture;
        
        GraphiteLog.Log("Creating swapchain render target.");
        fixed (ID3D11RenderTargetView** renderTarget = &RenderTarget)
        {
            device->CreateRenderTargetView((ID3D11Resource*) Texture, null, renderTarget).Check("Create swapchain render target");
        }
    }

    public override void Dispose()
    {
        if (RenderTarget != null)
            RenderTarget->Release();

        if (ResourceView != null)
            ResourceView->Release();

        Texture->Release();
    }
}