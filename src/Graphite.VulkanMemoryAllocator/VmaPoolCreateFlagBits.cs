using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    [NativeTypeName("unsigned int")]
    public enum VmaPoolCreateFlagBits : uint
    {
        VMA_POOL_CREATE_IGNORE_BUFFER_IMAGE_GRANULARITY_BIT = 0x00000002,
        VMA_POOL_CREATE_LINEAR_ALGORITHM_BIT = 0x00000004,
        VMA_POOL_CREATE_ALGORITHM_MASK = VMA_POOL_CREATE_LINEAR_ALGORITHM_BIT,
        VMA_POOL_CREATE_FLAG_BITS_MAX_ENUM = 0x7FFFFFFF,
    }
}
