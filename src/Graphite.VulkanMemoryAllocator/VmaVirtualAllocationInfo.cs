using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaVirtualAllocationInfo
    {
        [NativeTypeName("VkDeviceSize")]
        public nuint offset;

        [NativeTypeName("VkDeviceSize")]
        public nuint size;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;
    }
}
