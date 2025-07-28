using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    [NativeTypeName("unsigned int")]
    public enum VmaDefragmentationMoveOperation : uint
    {
        VMA_DEFRAGMENTATION_MOVE_OPERATION_COPY = 0,
        VMA_DEFRAGMENTATION_MOVE_OPERATION_IGNORE = 1,
        VMA_DEFRAGMENTATION_MOVE_OPERATION_DESTROY = 2,
    }
}
