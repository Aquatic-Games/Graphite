using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader* VertexShader;
    public readonly ID3D11PixelShader* PixelShader;
    
    public D3D11Pipeline(ID3D11Device1* device, ref readonly GraphicsPipelineInfo info)
    {
        D3D11ShaderModule vertexShader = (D3D11ShaderModule) info.VertexShader;
        D3D11ShaderModule pixelShader = (D3D11ShaderModule) info.PixelShader;
        
        // pressed the wrong key and I laughed so I am not changing it
        fixed (ID3D11VertexShader** pBertexShader = &VertexShader)
        {
            device->CreateVertexShader(vertexShader.Data, vertexShader.DataLength, null, pBertexShader)
                .Check("Create vertex shader");
        }
        
        // Create the 🅱️ixel shader
        fixed (ID3D11PixelShader** pBixelShader = &PixelShader)
        {
            device->CreatePixelShader(pixelShader.Data, pixelShader.DataLength, null, pBixelShader)
                .Check("Create pixel shader");
        }
    }
    
    public override void Dispose()
    {
        PixelShader->Release();
        VertexShader->Release();
    }
}