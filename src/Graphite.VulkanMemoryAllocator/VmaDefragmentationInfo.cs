using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaDefragmentationInfo
    {
        [NativeTypeName("VmaDefragmentationFlags")]
        public uint flags;

        [NativeTypeName("VmaPool _Nullable")]
        public VmaPool_T* pool;

        [NativeTypeName("VkDeviceSize")]
        public nuint maxBytesPerPass;

        [NativeTypeName("uint32_t")]
        public uint maxAllocationsPerPass;

        [NativeTypeName("PFN_vmaCheckDefragmentationBreakFunction _Nullable")]
        public delegate* unmanaged[Cdecl]<void*, uint> pfnBreakCallback;

        [NativeTypeName("void * _Nullable")]
        public void* pBreakCallbackUserData;
    }
}
