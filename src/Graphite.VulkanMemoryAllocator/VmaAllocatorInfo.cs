using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator;

public partial struct VmaAllocatorInfo
{
    [NativeTypeName("VkInstance _Nonnull")]
    public Instance instance;

    [NativeTypeName("VkPhysicalDevice _Nonnull")]
    public PhysicalDevice physicalDevice;

    [NativeTypeName("VkDevice _Nonnull")]
    public Device device;
}
