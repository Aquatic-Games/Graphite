using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public partial struct VmaDefragmentationStats
    {
        [NativeTypeName("VkDeviceSize")]
        public nuint bytesMoved;

        [NativeTypeName("VkDeviceSize")]
        public nuint bytesFreed;

        [NativeTypeName("uint32_t")]
        public uint allocationsMoved;

        [NativeTypeName("uint32_t")]
        public uint deviceMemoryBlocksFreed;
    }
}
