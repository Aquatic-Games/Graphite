using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_FILTER;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Sampler : Sampler
{
    public readonly ID3D11SamplerState* Sampler;
    
    public D3D11Sampler(ID3D11Device1* device, ref readonly SamplerInfo info)
    {
        D3D11_FILTER filter;

        if (info.MaxAnisotropy > 0)
            filter = D3D11_FILTER_ANISOTROPIC;
        else
        {
            filter = (info.MinFilter, info.MagFilter, info.MipFilter) switch
            {
                (Filter.Linear, Filter.Linear, Filter.Linear) => D3D11_FILTER_MIN_MAG_MIP_LINEAR,
                (Filter.Linear, Filter.Linear, Filter.Point) => D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT,
                (Filter.Linear, Filter.Point, Filter.Linear) => D3D11_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR,
                (Filter.Linear, Filter.Point, Filter.Point) => D3D11_FILTER_MIN_LINEAR_MAG_MIP_POINT,
                (Filter.Point, Filter.Linear, Filter.Linear) => D3D11_FILTER_MIN_POINT_MAG_MIP_LINEAR,
                (Filter.Point, Filter.Linear, Filter.Point) => D3D11_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT,
                (Filter.Point, Filter.Point, Filter.Linear) => D3D11_FILTER_MIN_MAG_POINT_MIP_LINEAR,
                (Filter.Point, Filter.Point, Filter.Point) => D3D11_FILTER_MIN_MAG_MIP_POINT,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        D3D11_SAMPLER_DESC samplerDesc = new()
        {
            Filter = filter,
            AddressU = info.AddressU.ToD3D(),
            AddressV = info.AddressV.ToD3D(),
            AddressW = info.AddressW.ToD3D(),
            MaxAnisotropy = info.MaxAnisotropy,
            MinLOD = info.MinLod,
            MaxLOD = info.MaxLod
        };
        
        GraphiteLog.Log("Creating sampler state.");
        fixed (ID3D11SamplerState** sampler = &Sampler)
            device->CreateSamplerState(&samplerDesc, sampler).Check("Create sampler");
    }
    
    public override void Dispose()
    {
        Sampler->Release();
    }
}