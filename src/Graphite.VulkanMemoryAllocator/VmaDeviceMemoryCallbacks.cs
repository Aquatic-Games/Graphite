using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaDeviceMemoryCallbacks
    {
        [NativeTypeName("PFN_vmaAllocateDeviceMemoryFunction _Nullable")]
        public delegate* unmanaged[Cdecl]<Allocator*, uint, DeviceMemory, nuint, void*, void> pfnAllocate;

        [NativeTypeName("PFN_vmaFreeDeviceMemoryFunction _Nullable")]
        public delegate* unmanaged[Cdecl]<Allocator*, uint, DeviceMemory, nuint, void*, void> pfnFree;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;
    }
}
