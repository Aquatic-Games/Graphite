using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public partial struct VmaAllocationInfo2
    {
        public VmaAllocationInfo allocationInfo;

        [NativeTypeName("VkDeviceSize")]
        public nuint blockSize;

        [NativeTypeName("VkBool32")]
        public uint dedicatedMemory;
    }
}
