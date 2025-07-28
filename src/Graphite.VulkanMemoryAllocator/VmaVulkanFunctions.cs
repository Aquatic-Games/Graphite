using Silk.NET.Vulkan;

namespace Graphite.VulkanMemoryAllocator
{
    public unsafe partial struct VmaVulkanFunctions
    {
        [NativeTypeName("PFN_vkGetInstanceProcAddr _Nullable")]
        public delegate* unmanaged[Cdecl]<Instance, sbyte*, delegate* unmanaged[Cdecl]<void>> vkGetInstanceProcAddr;

        [NativeTypeName("PFN_vkGetDeviceProcAddr _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, sbyte*, delegate* unmanaged[Cdecl]<void>> vkGetDeviceProcAddr;

        [NativeTypeName("PFN_vkGetPhysicalDeviceProperties _Nullable")]
        public delegate* unmanaged[Cdecl]<PhysicalDevice, PhysicalDeviceProperties*, void> vkGetPhysicalDeviceProperties;

        [NativeTypeName("PFN_vkGetPhysicalDeviceMemoryProperties _Nullable")]
        public delegate* unmanaged[Cdecl]<PhysicalDevice, PhysicalDeviceMemoryProperties*, void> vkGetPhysicalDeviceMemoryProperties;

        [NativeTypeName("PFN_vkAllocateMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, MemoryAllocateInfo*, AllocationCallbacks*, DeviceMemory*, Result> vkAllocateMemory;

        [NativeTypeName("PFN_vkFreeMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, DeviceMemory, AllocationCallbacks*, void> vkFreeMemory;

        [NativeTypeName("PFN_vkMapMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, DeviceMemory, nuint, nuint, uint, void**, Result> vkMapMemory;

        [NativeTypeName("PFN_vkUnmapMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, DeviceMemory, void> vkUnmapMemory;

        [NativeTypeName("PFN_vkFlushMappedMemoryRanges _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, uint, MappedMemoryRange*, Result> vkFlushMappedMemoryRanges;

        [NativeTypeName("PFN_vkInvalidateMappedMemoryRanges _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, uint, MappedMemoryRange*, Result> vkInvalidateMappedMemoryRanges;

        [NativeTypeName("PFN_vkBindBufferMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Silk.NET.Vulkan.Buffer, DeviceMemory, nuint, Result> vkBindBufferMemory;

        [NativeTypeName("PFN_vkBindImageMemory _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Image, DeviceMemory, nuint, Result> vkBindImageMemory;

        [NativeTypeName("PFN_vkGetBufferMemoryRequirements _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Silk.NET.Vulkan.Buffer, MemoryRequirements*, void> vkGetBufferMemoryRequirements;

        [NativeTypeName("PFN_vkGetImageMemoryRequirements _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Image, MemoryRequirements*, void> vkGetImageMemoryRequirements;

        [NativeTypeName("PFN_vkCreateBuffer _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, BufferCreateInfo*, AllocationCallbacks*, Silk.NET.Vulkan.Buffer*, Result> vkCreateBuffer;

        [NativeTypeName("PFN_vkDestroyBuffer _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Silk.NET.Vulkan.Buffer, AllocationCallbacks*, void> vkDestroyBuffer;

        [NativeTypeName("PFN_vkCreateImage _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, ImageCreateInfo*, AllocationCallbacks*, Image*, Result> vkCreateImage;

        [NativeTypeName("PFN_vkDestroyImage _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, Image, AllocationCallbacks*, void> vkDestroyImage;

        [NativeTypeName("PFN_vkCmdCopyBuffer _Nullable")]
        public delegate* unmanaged[Cdecl]<CommandBuffer, Silk.NET.Vulkan.Buffer, Silk.NET.Vulkan.Buffer, uint, BufferCopy*, void> vkCmdCopyBuffer;

        [NativeTypeName("PFN_vkGetBufferMemoryRequirements2KHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, BufferMemoryRequirementsInfo2*, MemoryRequirements2*, void> vkGetBufferMemoryRequirements2KHR;

        [NativeTypeName("PFN_vkGetImageMemoryRequirements2KHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, ImageMemoryRequirementsInfo2*, MemoryRequirements2*, void> vkGetImageMemoryRequirements2KHR;

        [NativeTypeName("PFN_vkBindBufferMemory2KHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, uint, BindBufferMemoryInfo*, Result> vkBindBufferMemory2KHR;

        [NativeTypeName("PFN_vkBindImageMemory2KHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, uint, BindImageMemoryInfo*, Result> vkBindImageMemory2KHR;

        [NativeTypeName("PFN_vkGetPhysicalDeviceMemoryProperties2KHR _Nullable")]
        public delegate* unmanaged[Cdecl]<PhysicalDevice, PhysicalDeviceMemoryProperties2*, void> vkGetPhysicalDeviceMemoryProperties2KHR;

        [NativeTypeName("PFN_vkGetDeviceBufferMemoryRequirementsKHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, DeviceBufferMemoryRequirements*, MemoryRequirements2*, void> vkGetDeviceBufferMemoryRequirements;

        [NativeTypeName("PFN_vkGetDeviceImageMemoryRequirementsKHR _Nullable")]
        public delegate* unmanaged[Cdecl]<Device, DeviceImageMemoryRequirements*, MemoryRequirements2*, void> vkGetDeviceImageMemoryRequirements;

        [NativeTypeName("void * _Nullable")]
        public void* vkGetMemoryWin32HandleKHR;
    }
}
