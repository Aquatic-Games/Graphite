using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public unsafe partial struct VmaVirtualAllocationCreateInfo
{
    [NativeTypeName("VkDeviceSize")]
    public nuint size;

    [NativeTypeName("VkDeviceSize")]
    public nuint alignment;

    [NativeTypeName("VmaVirtualAllocationCreateFlags")]
    public uint flags;

    [NativeTypeName("void * _Nullable")]
    public void* pUserData;
}
