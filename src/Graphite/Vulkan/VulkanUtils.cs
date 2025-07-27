global using VkFormat = Silk.NET.Vulkan.Format;
using Graphite.Exceptions;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal static class VulkanUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new OperationFailedException($"Vulkan operation '{operation}' failed: {result}");
    }

    public static VkFormat ToVk(this Format format)
    {
        return format switch
        {
            Format.Unknown => VkFormat.Undefined,
            Format.B5G6R5_UNorm => VkFormat.B5G6R5UnormPack16,
            Format.B5G5R5A1_UNorm => VkFormat.B5G5R5A1UnormPack16,
            Format.A8_UNorm => VkFormat.A8UnormKhr,
            Format.R8_UNorm => VkFormat.R8Unorm,
            Format.R8_UInt => VkFormat.R8Uint,
            Format.R8_SNorm => VkFormat.R8SNorm,
            Format.R8_SInt => VkFormat.R8Sint,
            Format.R8G8_UNorm => VkFormat.R8G8Unorm,
            Format.R8G8_UInt => VkFormat.R8G8Uint,
            Format.R8G8_SNorm => VkFormat.R8G8SNorm,
            Format.R8G8_SInt => VkFormat.R8G8Sint,
            Format.R8G8B8A8_UNorm => VkFormat.R8G8B8A8Unorm,
            Format.R8G8B8A8_UNorm_SRGB => VkFormat.R8G8B8A8Srgb,
            Format.R8G8B8A8_UInt => VkFormat.R8G8B8A8Uint,
            Format.R8G8B8A8_SNorm => VkFormat.R8G8B8A8SNorm,
            Format.R8G8B8A8_SInt => VkFormat.R8G8B8A8Sint,
            Format.B8G8R8A8_UNorm => VkFormat.B8G8R8A8Unorm,
            Format.B8G8R8A8_UNorm_SRGB => VkFormat.B8G8R8A8Srgb,
            Format.R10G10B10A2_UNorm => VkFormat.A2R10G10B10UnormPack32,
            Format.R10G10B10A2_UInt => VkFormat.A2R10G10B10UintPack32,
            Format.R16_Float => VkFormat.R16Sfloat,
            Format.R16_UNorm => VkFormat.R16Unorm,
            Format.R16_UInt => VkFormat.R16Uint,
            Format.R16_SNorm => VkFormat.R16SNorm,
            Format.R16_SInt => VkFormat.R16Sint,
            Format.R16G16_Float => VkFormat.R16G16Sfloat,
            Format.R16G16_UNorm => VkFormat.R16G16Unorm,
            Format.R16G16_UInt => VkFormat.R16G16Uint,
            Format.R16G16_SNorm => VkFormat.R16G16SNorm,
            Format.R16G16_SInt => VkFormat.R16G16Sint,
            Format.R16G16B16A16_Float => VkFormat.R16G16B16A16Sfloat,
            Format.R16G16B16A16_UNorm => VkFormat.R16G16B16A16Unorm,
            Format.R16G16B16A16_UInt => VkFormat.R16G16B16A16Uint,
            Format.R16G16B16A16_SNorm => VkFormat.R16G16B16A16SNorm,
            Format.R16G16B16A16_SInt => VkFormat.R16G16B16A16Sint,
            Format.R32_Float => VkFormat.R32Sfloat,
            Format.R32_UInt => VkFormat.R32Uint,
            Format.R32_SInt => VkFormat.R32Sint,
            Format.R32G32_Float => VkFormat.R32G32Sfloat,
            Format.R32G32_UInt => VkFormat.R32G32Uint,
            Format.R32G32_SInt => VkFormat.R32G32Sint,
            Format.R32G32B32_Float => VkFormat.R32G32B32Sfloat,
            Format.R32G32B32_UInt => VkFormat.R32G32B32Uint,
            Format.R32G32B32_SInt => VkFormat.R32G32B32Sint,
            Format.R32G32B32A32_Float => VkFormat.R32G32B32A32Sfloat,
            Format.R32G32B32A32_UInt => VkFormat.R32G32B32A32Uint,
            Format.R32G32B32A32_SInt => VkFormat.R32G32B32A32Sint,
            Format.D16_UNorm => VkFormat.D16Unorm,
            Format.D24_UNorm_S8_UInt => VkFormat.D24UnormS8Uint,
            Format.D32_Float => VkFormat.D32Sfloat,
            Format.BC1_UNorm => VkFormat.BC1RgbaUnormBlock,
            Format.BC1_UNorm_SRGB => VkFormat.BC1RgbaSrgbBlock,
            Format.BC2_UNorm => VkFormat.BC2UnormBlock,
            Format.BC2_UNorm_SRGB => VkFormat.BC2SrgbBlock,
            Format.BC3_UNorm => VkFormat.BC3UnormBlock,
            Format.BC3_UNorm_SRGB => VkFormat.BC3SrgbBlock,
            Format.BC4_UNorm => VkFormat.BC4UnormBlock,
            Format.BC4_SNorm => VkFormat.BC4SNormBlock,
            Format.BC5_UNorm => VkFormat.BC5UnormBlock,
            Format.BC5_SNorm => VkFormat.BC5SNormBlock,
            Format.BC6H_UF16 => VkFormat.BC6HUfloatBlock,
            Format.BC6H_SF16 => VkFormat.BC6HSfloatBlock,
            Format.BC7_UNorm => VkFormat.BC7UnormBlock,
            Format.BC7_UNorm_SRGB => VkFormat.BC7SrgbBlock,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static PresentModeKHR ToVk(this PresentMode mode)
    {
        return mode switch
        {
            PresentMode.Fifo => PresentModeKHR.FifoKhr,
            PresentMode.Immediate => PresentModeKHR.ImmediateKhr,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public static AttachmentLoadOp ToVk(this LoadOp op)
    {
        return op switch
        {
            LoadOp.Load => AttachmentLoadOp.Load,
            LoadOp.Clear => AttachmentLoadOp.Clear,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static AttachmentStoreOp ToVk(this StoreOp op)
    {
        return op switch
        {
            StoreOp.Store => AttachmentStoreOp.Store,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
}