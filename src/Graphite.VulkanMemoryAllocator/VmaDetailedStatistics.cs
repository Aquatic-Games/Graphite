using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public partial struct VmaDetailedStatistics
{
    public VmaStatistics statistics;

    [NativeTypeName("uint32_t")]
    public uint unusedRangeCount;

    [NativeTypeName("VkDeviceSize")]
    public nuint allocationSizeMin;

    [NativeTypeName("VkDeviceSize")]
    public nuint allocationSizeMax;

    [NativeTypeName("VkDeviceSize")]
    public nuint unusedRangeSizeMin;

    [NativeTypeName("VkDeviceSize")]
    public nuint unusedRangeSizeMax;
}
