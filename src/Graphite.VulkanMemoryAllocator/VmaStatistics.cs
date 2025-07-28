using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public partial struct VmaStatistics
{
    [NativeTypeName("uint32_t")]
    public uint blockCount;

    [NativeTypeName("uint32_t")]
    public uint allocationCount;

    [NativeTypeName("VkDeviceSize")]
    public nuint blockBytes;

    [NativeTypeName("VkDeviceSize")]
    public nuint allocationBytes;
}
