using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public static unsafe partial class Vma
{
    public static Result CreateAllocator(AllocatorCreateInfo* pCreateInfo, out Allocator* allocator)
    {
        fixed (Allocator** pAllocator = &allocator)
            return CreateAllocator(pCreateInfo, pAllocator);
    }
}