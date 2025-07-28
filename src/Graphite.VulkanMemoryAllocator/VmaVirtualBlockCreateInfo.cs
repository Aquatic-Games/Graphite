using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaVirtualBlockCreateInfo
    {
        [NativeTypeName("VkDeviceSize")]
        public nuint size;

        [NativeTypeName("VmaVirtualBlockCreateFlags")]
        public uint flags;

        [NativeTypeName("const VkAllocationCallbacks * _Nullable")]
        public AllocationCallbacks* pAllocationCallbacks;
    }
}
