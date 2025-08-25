global using VkBuffer = Silk.NET.Vulkan.Buffer;
using System.Runtime.CompilerServices;
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

    public readonly bool IsMappable;

    public VulkanBuffer(Vk vk, VulkanDevice device, Allocator* allocator, ref readonly BufferInfo info, void* data) : base((BufferInfo) info)
    {
        _vk = vk;
        _device = device.Device;
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
            IsMappable = true;
        }

        if ((info.Usage & BufferUsage.MapWrite) != 0)
        {
            allocInfo.flags |= (uint) VMA_ALLOCATION_CREATE_HOST_ACCESS_RANDOM_BIT;
            IsMappable = true;
        }

        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Usage = usage,
            Size = info.SizeInBytes
        };
        
        GraphiteLog.Log("Creating buffer.");
        Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out Allocation, null).Check("Create buffer");

        if (data == null)
            return;

        if (IsMappable)
        {
            void* mappedData;
            GraphiteLog.Log("Mapping buffer and copying data.");
            Vma.MapMemory(_allocator, Allocation, &mappedData).Check("Map buffer");
            Unsafe.CopyBlock(mappedData, data, info.SizeInBytes);
            Vma.UnmapMemory(_allocator, Allocation);
            return;
        }
        
        // Create a new transfer buffer and copy the data.
        // This is quite inefficient as a transfer buffer is created for each buffer we are creating.
        // TODO: VulkanDevice.GetFreeTransferBuffer(uint size) with a transfer buffer pool?
        
        AllocationCreateInfo transferAllocInfo = new()
        {
            usage = VMA_MEMORY_USAGE_AUTO,
            flags = (uint) VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT | (uint) VMA_ALLOCATION_CREATE_MAPPED_BIT
        };

        BufferCreateInfo transferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Usage = BufferUsageFlags.TransferSrcBit,
            Size = info.SizeInBytes
        };
        
        GraphiteLog.Log("Creating transfer buffer.");
        // please help I ran out of names
        VmaAllocationInfo tInfo;
        Vma.CreateBuffer(_allocator, &transferInfo, &transferAllocInfo, out VkBuffer transferBuffer,
            out Allocation* transferAlloc, &tInfo).Check("Create transfer buffer");
        
        Unsafe.CopyBlock(tInfo.pMappedData, data, info.SizeInBytes);

        CommandBuffer cb = device.BeginCommands();
        
        BufferCopy copy = new()
        {
            Size = info.SizeInBytes
        };
        
        GraphiteLog.Log($"Copying {copy.Size} bytes from transfer buffer to buffer.");
        _vk.CmdCopyBuffer(cb, transferBuffer, Buffer, 1, &copy);
        
        device.EndCommands();
        
        GraphiteLog.Log("Destroying transfer buffer.");
        Vma.DestroyBuffer(_allocator, transferBuffer, transferAlloc);
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying buffer.");
        Vma.DestroyBuffer(_allocator, Buffer, Allocation);
    }
}