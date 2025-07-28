global using VkBuffer = Silk.NET.Vulkan.Buffer;
using Graphite.Core;
using Graphite.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static Graphite.VulkanMemoryAllocator.AllocationCreateFlags;
using static Graphite.VulkanMemoryAllocator.VmaMemoryUsage;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanBuffer : Buffer
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly Allocator* _allocator;

    public readonly Allocation* Allocation;
    public readonly VkBuffer Buffer;

    public VulkanBuffer(Vk vk, VkDevice device, Allocator* allocator, ref readonly BufferInfo info)
    {
        _vk = vk;
        _device = device;
        _allocator = allocator;
        
        AllocationCreateInfo allocInfo = new()
        {
            usage = VMA_MEMORY_USAGE_AUTO
        };

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
        {
            usage |= BufferUsageFlags.TransferSrcBit;
            allocInfo.flags |= (uint) VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT;
        }

        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Usage = usage,
            Size = info.SizeInBytes
        };
        
        GraphiteLog.Log("Creating buffer.");
        Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out Allocation, null).Check("Create buffer");
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying buffer.");
        Vma.DestroyBuffer(_allocator, Buffer, Allocation);
    }
}