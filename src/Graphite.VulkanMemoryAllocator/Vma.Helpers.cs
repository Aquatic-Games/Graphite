using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Graphite.VulkanMemoryAllocator;

public static unsafe partial class Vma
{
    public static Result CreateAllocator(AllocatorCreateInfo* pCreateInfo, out Allocator* allocator)
    {
        fixed (Allocator** pAllocator = &allocator)
            return CreateAllocator(pCreateInfo, pAllocator);
    }

    public static Result CreateBuffer(Allocator* allocator, BufferCreateInfo* pBufferCreateInfo,
        AllocationCreateInfo* pAllocationCreateInfo, out Buffer buffer, out Allocation* allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        fixed (Buffer* pBuffer = &buffer)
        fixed (Allocation** pAllocation = &allocation)
        {
            return CreateBuffer(allocator, pBufferCreateInfo, pAllocationCreateInfo, pBuffer, pAllocation,
                pAllocationInfo);
        }
    }

    public static Result CreateImage(Allocator* allocator, ImageCreateInfo* pImageCreateInfo,
        AllocationCreateInfo* pAllocationCreateInfo, out Image image, out Allocation* allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        fixed (Image* pImage = &image)
        fixed (Allocation** pAllocation = &allocation)
        {
            return CreateImage(allocator, pImageCreateInfo, pAllocationCreateInfo, pImage, pAllocation,
                pAllocationInfo);
        }
    }
}