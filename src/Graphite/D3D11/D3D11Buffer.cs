using System.Diagnostics.CodeAnalysis;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG;
using static TerraFX.Interop.DirectX.D3D11_MAP;
using static TerraFX.Interop.DirectX.D3D11_USAGE;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Buffer : Buffer
{
    public readonly ID3D11Buffer* Buffer;

    public readonly D3D11_MAP MapType;
    
    public D3D11Buffer(ID3D11Device1* device, ref readonly BufferInfo info, void* data) : base(info)
    {
        D3D11_BIND_FLAG flags = 0;
        D3D11_USAGE usage = D3D11_USAGE_DEFAULT;
        D3D11_CPU_ACCESS_FLAG cpuFlags = 0;

        if ((info.Usage & BufferUsage.VertexBuffer) != 0)
            flags |= D3D11_BIND_VERTEX_BUFFER;
        if ((info.Usage & BufferUsage.IndexBuffer) != 0)
            flags |= D3D11_BIND_INDEX_BUFFER;
        if ((info.Usage & BufferUsage.ConstantBuffer) != 0)
            flags |= D3D11_BIND_CONSTANT_BUFFER;
        if ((info.Usage & BufferUsage.StructuredBuffer) != 0)
            throw new NotImplementedException();
        if ((info.Usage & BufferUsage.TransferBuffer) != 0)
        {
            usage = D3D11_USAGE_STAGING;
            cpuFlags |= D3D11_CPU_ACCESS_WRITE;
            MapType = D3D11_MAP_WRITE;
        }

        if ((info.Usage & BufferUsage.MapWrite) != 0)
        {
            usage = D3D11_USAGE_DYNAMIC;
            cpuFlags |= D3D11_CPU_ACCESS_WRITE;
            // If the user provides the MapWrite flag with a transfer buffer, don't set it to discard as this will cause
            // an error.
            if ((info.Usage & BufferUsage.TransferBuffer) == 0)
                MapType = D3D11_MAP_WRITE_DISCARD;
        }

        D3D11_BUFFER_DESC bufferDesc = new()
        {
            BindFlags = (uint) flags,
            Usage = usage,
            ByteWidth = info.SizeInBytes,
            CPUAccessFlags = (uint) cpuFlags,
        };

        D3D11_SUBRESOURCE_DATA resourceData = new()
        {
            pSysMem = data
        };

        GraphiteLog.Log("Creating buffer.");
        fixed (ID3D11Buffer** buffer = &Buffer)
            device->CreateBuffer(&bufferDesc, data == null ? null : &resourceData, buffer).Check("Create buffer");
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Releasing buffer.");
        Buffer->Release();
    }
}