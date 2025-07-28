using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    [NativeTypeName("unsigned int")]
    public enum VmaVirtualBlockCreateFlagBits : uint
    {
        VMA_VIRTUAL_BLOCK_CREATE_LINEAR_ALGORITHM_BIT = 0x00000001,
        VMA_VIRTUAL_BLOCK_CREATE_ALGORITHM_MASK = VMA_VIRTUAL_BLOCK_CREATE_LINEAR_ALGORITHM_BIT,
        VMA_VIRTUAL_BLOCK_CREATE_FLAG_BITS_MAX_ENUM = 0x7FFFFFFF,
    }
}
