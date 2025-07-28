global using VkBuffer = Silk.NET.Vulkan.Buffer;
using Graphite.VulkanMemoryAllocator;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed class VulkanBuffer : Buffer
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    public readonly VkBuffer Buffer;

    public VulkanBuffer(Vk vk, VkDevice device, ref readonly BufferInfo info)
    {
        _vk = vk;
        _device = device;

        BufferUsageFlags usage = BufferUsageFlags.TransferDstBit;

        if ((info.Usage & BufferUsage.VertexBuffer) != 0)
            usage |= BufferUsageFlags.VertexBufferBit;
        if ((info.Usage & BufferUsage.IndexBuffer) != 0)
            usage |= BufferUsageFlags.IndexBufferBit;
        if ((info.Usage & BufferUsage.ConstantBuffer) != 0)
            usage |= BufferUsageFlags.UniformBufferBit;
        if ((info.Usage & BufferUsage.StructuredBuffer) != 0)
            usage |= BufferUsageFlags.StorageBufferBit;
        if ((info.Usage & BufferUsage.TransferBuffer) != 0)
            usage |= BufferUsageFlags.TransferSrcBit;

        AllocationCreateInfo allocInfo = new()
        {
            usage = VmaMemoryUsage.VMA_MEMORY_USAGE_AUTO,
            flags = AllocationCreateFlags.
        };

        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Usage = usage,
            Size = info.SizeInBytes
        };
    }
    
    public override void Dispose()
    {
        
    }
}