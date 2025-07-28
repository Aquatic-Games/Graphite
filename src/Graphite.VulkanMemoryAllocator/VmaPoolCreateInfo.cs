using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public unsafe partial struct VmaPoolCreateInfo
{
    [NativeTypeName("uint32_t")]
    public uint memoryTypeIndex;

    [NativeTypeName("VmaPoolCreateFlags")]
    public uint flags;

    [NativeTypeName("VkDeviceSize")]
    public nuint blockSize;

    [NativeTypeName("size_t")]
    public nuint minBlockCount;

    [NativeTypeName("size_t")]
    public nuint maxBlockCount;

    public float priority;

    [NativeTypeName("VkDeviceSize")]
    public nuint minAllocationAlignment;

    [NativeTypeName("void * _Nullable")]
    public void* pMemoryAllocateNext;
}
