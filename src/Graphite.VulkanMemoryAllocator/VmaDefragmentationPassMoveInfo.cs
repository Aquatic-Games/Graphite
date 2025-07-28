using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaDefragmentationPassMoveInfo
    {
        [NativeTypeName("uint32_t")]
        public uint moveCount;

        [NativeTypeName("VmaDefragmentationMove * _Nullable")]
        public VmaDefragmentationMove* pMoves;
    }
}
