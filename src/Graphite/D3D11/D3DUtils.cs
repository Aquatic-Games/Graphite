using System.Diagnostics.CodeAnalysis;
using Graphite.Exceptions;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI_FORMAT;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class D3DUtils
{
    public static void Check(this HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new OperationFailedException($"D3D11 operation '{operation}' failed with HRESULT: {result.Value}");
    }

    public static DXGI_FORMAT ToD3D(this Format format)
    {
        return format switch
        {
            Format.Unknown => DXGI_FORMAT_UNKNOWN,
            Format.B5G6R5_UNorm => DXGI_FORMAT_B5G6R5_UNORM,
            Format.B5G5R5A1_UNorm => DXGI_FORMAT_B5G5R5A1_UNORM,
            Format.A8_UNorm => DXGI_FORMAT_A8_UNORM,
            Format.R8_UNorm => DXGI_FORMAT_R8_UNORM,
            Format.R8_UInt => DXGI_FORMAT_R8_UINT,
            Format.R8_SNorm => DXGI_FORMAT_R8_SNORM,
            Format.R8_SInt => DXGI_FORMAT_R8_SINT,
            Format.R8G8_UNorm => DXGI_FORMAT_R8G8_UNORM,
            Format.R8G8_UInt => DXGI_FORMAT_R8G8_UINT,
            Format.R8G8_SNorm => DXGI_FORMAT_R8G8_SNORM,
            Format.R8G8_SInt => DXGI_FORMAT_R8G8_SINT,
            Format.R8G8B8A8_UNorm => DXGI_FORMAT_R8G8B8A8_UNORM,
            Format.R8G8B8A8_UNorm_SRGB => DXGI_FORMAT_R8G8B8A8_UNORM_SRGB,
            Format.R8G8B8A8_UInt => DXGI_FORMAT_R8G8B8A8_UINT,
            Format.R8G8B8A8_SNorm => DXGI_FORMAT_R8G8B8A8_SNORM,
            Format.R8G8B8A8_SInt => DXGI_FORMAT_R8G8B8A8_SINT,
            Format.B8G8R8A8_UNorm => DXGI_FORMAT_B8G8R8A8_UNORM,
            Format.B8G8R8A8_UNorm_SRGB => DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
            Format.R10G10B10A2_UNorm => DXGI_FORMAT_R10G10B10A2_UNORM,
            Format.R10G10B10A2_UInt => DXGI_FORMAT_R10G10B10A2_UINT,
            Format.R16_Float => DXGI_FORMAT_R16_FLOAT,
            Format.R16_UNorm => DXGI_FORMAT_R16_UNORM,
            Format.R16_UInt => DXGI_FORMAT_R16_UINT,
            Format.R16_SNorm => DXGI_FORMAT_R16_SNORM,
            Format.R16_SInt => DXGI_FORMAT_R16_SINT,
            Format.R16G16_Float => DXGI_FORMAT_R16G16_FLOAT,
            Format.R16G16_UNorm => DXGI_FORMAT_R16G16_UNORM,
            Format.R16G16_UInt => DXGI_FORMAT_R16G16_UINT,
            Format.R16G16_SNorm => DXGI_FORMAT_R16G16_SNORM,
            Format.R16G16_SInt => DXGI_FORMAT_R16G16_SINT,
            Format.R16G16B16A16_Float => DXGI_FORMAT_R16G16B16A16_FLOAT,
            Format.R16G16B16A16_UNorm => DXGI_FORMAT_R16G16B16A16_UNORM,
            Format.R16G16B16A16_UInt => DXGI_FORMAT_R16G16B16A16_UINT,
            Format.R16G16B16A16_SNorm => DXGI_FORMAT_R16G16B16A16_SNORM,
            Format.R16G16B16A16_SInt => DXGI_FORMAT_R16G16B16A16_SINT,
            Format.R32_Float => DXGI_FORMAT_R32_FLOAT,
            Format.R32_UInt => DXGI_FORMAT_R32_UINT,
            Format.R32_SInt => DXGI_FORMAT_R32_SINT,
            Format.R32G32_Float => DXGI_FORMAT_R32G32_FLOAT,
            Format.R32G32_UInt => DXGI_FORMAT_R32G32_UINT,
            Format.R32G32_SInt => DXGI_FORMAT_R32G32_SINT,
            Format.R32G32B32_Float => DXGI_FORMAT_R32G32B32_FLOAT,
            Format.R32G32B32_UInt => DXGI_FORMAT_R32G32B32_UINT,
            Format.R32G32B32_SInt => DXGI_FORMAT_R32G32B32_SINT,
            Format.R32G32B32A32_Float => DXGI_FORMAT_R32G32B32A32_FLOAT,
            Format.R32G32B32A32_UInt => DXGI_FORMAT_R32G32B32A32_UINT,
            Format.R32G32B32A32_SInt => DXGI_FORMAT_R32G32B32A32_SINT,
            Format.D16_UNorm => DXGI_FORMAT_D16_UNORM,
            Format.D24_UNorm_S8_UInt => DXGI_FORMAT_D24_UNORM_S8_UINT,
            Format.D32_Float => DXGI_FORMAT_D32_FLOAT,
            Format.BC1_UNorm => DXGI_FORMAT_BC1_UNORM,
            Format.BC1_UNorm_SRGB => DXGI_FORMAT_BC1_UNORM_SRGB,
            Format.BC2_UNorm => DXGI_FORMAT_BC2_UNORM,
            Format.BC2_UNorm_SRGB => DXGI_FORMAT_BC2_UNORM_SRGB,
            Format.BC3_UNorm => DXGI_FORMAT_BC3_UNORM,
            Format.BC3_UNorm_SRGB => DXGI_FORMAT_BC3_UNORM_SRGB,
            Format.BC4_UNorm => DXGI_FORMAT_BC4_UNORM,
            Format.BC4_SNorm => DXGI_FORMAT_BC4_SNORM,
            Format.BC5_UNorm => DXGI_FORMAT_BC5_UNORM,
            Format.BC5_SNorm => DXGI_FORMAT_BC5_SNORM,
            Format.BC6H_UF16 => DXGI_FORMAT_BC6H_UF16,
            Format.BC6H_SF16 => DXGI_FORMAT_BC6H_SF16,
            Format.BC7_UNorm => DXGI_FORMAT_BC7_UNORM,
            Format.BC7_UNorm_SRGB => DXGI_FORMAT_BC7_UNORM_SRGB,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}