using Silk.NET.Vulkan;
using System.Runtime.InteropServices;

namespace Graphite.VulkanMemoryAllocator;

public static unsafe partial class Vma
{
    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAllocator", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateAllocator([NativeTypeName("const VmaAllocatorCreateInfo * _Nonnull")] AllocatorCreateInfo* pCreateInfo, [NativeTypeName("VmaAllocator  _Nullable * _Nonnull")] Allocator** pAllocator);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyAllocator", ExactSpelling = true)]
    public static extern void DestroyAllocator([NativeTypeName("VmaAllocator _Nullable")] Allocator* allocator);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetAllocatorInfo", ExactSpelling = true)]
    public static extern void GetAllocatorInfo([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocatorInfo * _Nonnull")] VmaAllocatorInfo* pAllocatorInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetPhysicalDeviceProperties", ExactSpelling = true)]
    public static extern void GetPhysicalDeviceProperties([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkPhysicalDeviceProperties * _Nullable * _Nonnull")] PhysicalDeviceProperties** ppPhysicalDeviceProperties);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetMemoryProperties", ExactSpelling = true)]
    public static extern void GetMemoryProperties([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkPhysicalDeviceMemoryProperties * _Nullable * _Nonnull")] PhysicalDeviceMemoryProperties** ppPhysicalDeviceMemoryProperties);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetMemoryTypeProperties", ExactSpelling = true)]
    public static extern void GetMemoryTypeProperties([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint memoryTypeIndex, [NativeTypeName("VkMemoryPropertyFlags * _Nonnull")] uint* pFlags);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaSetCurrentFrameIndex", ExactSpelling = true)]
    public static extern void SetCurrentFrameIndex([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint frameIndex);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCalculateStatistics", ExactSpelling = true)]
    public static extern void CalculateStatistics([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaTotalStatistics * _Nonnull")] VmaTotalStatistics* pStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetHeapBudgets", ExactSpelling = true)]
    public static extern void GetHeapBudgets([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaBudget * _Nonnull")] VmaBudget* pBudgets);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFindMemoryTypeIndex", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result FindMemoryTypeIndex([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint memoryTypeBits, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("uint32_t * _Nonnull")] uint* pMemoryTypeIndex);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFindMemoryTypeIndexForBufferInfo", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result FindMemoryTypeIndexForBufferInfo([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("uint32_t * _Nonnull")] uint* pMemoryTypeIndex);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFindMemoryTypeIndexForImageInfo", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result FindMemoryTypeIndexForImageInfo([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkImageCreateInfo * _Nonnull")] ImageCreateInfo* pImageCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("uint32_t * _Nonnull")] uint* pMemoryTypeIndex);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreatePool", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreatePool([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VmaPoolCreateInfo * _Nonnull")] VmaPoolCreateInfo* pCreateInfo, [NativeTypeName("VmaPool  _Nullable * _Nonnull")] VmaPool_T** pPool);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyPool", ExactSpelling = true)]
    public static extern void DestroyPool([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nullable")] VmaPool_T* pool);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetPoolStatistics", ExactSpelling = true)]
    public static extern void GetPoolStatistics([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nonnull")] VmaPool_T* pool, [NativeTypeName("VmaStatistics * _Nonnull")] VmaStatistics* pPoolStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCalculatePoolStatistics", ExactSpelling = true)]
    public static extern void CalculatePoolStatistics([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nonnull")] VmaPool_T* pool, [NativeTypeName("VmaDetailedStatistics * _Nonnull")] VmaDetailedStatistics* pPoolStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCheckPoolCorruption", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CheckPoolCorruption([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nonnull")] VmaPool_T* pool);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetPoolName", ExactSpelling = true)]
    public static extern void GetPoolName([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nonnull")] VmaPool_T* pool, [NativeTypeName("const char * _Nullable * _Nonnull")] sbyte** ppName);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaSetPoolName", ExactSpelling = true)]
    public static extern void SetPoolName([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaPool _Nonnull")] VmaPool_T* pool, [NativeTypeName("const char * _Nullable")] sbyte* pName);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaAllocateMemory", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result AllocateMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkMemoryRequirements * _Nonnull")] MemoryRequirements* pVkMemoryRequirements, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pCreateInfo, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaAllocateMemoryPages", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result AllocateMemoryPages([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkMemoryRequirements * _Nonnull")] MemoryRequirements* pVkMemoryRequirements, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pCreateInfo, [NativeTypeName("size_t")] nuint allocationCount, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocations, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaAllocateMemoryForBuffer", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result AllocateMemoryForBuffer([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VkBuffer _Nonnull")] Silk.NET.Vulkan.Buffer buffer, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pCreateInfo, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaAllocateMemoryForImage", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result AllocateMemoryForImage([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VkImage _Nonnull")] Image image, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pCreateInfo, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFreeMemory", ExactSpelling = true)]
    public static extern void FreeMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nullable")] Allocation* allocation);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFreeMemoryPages", ExactSpelling = true)]
    public static extern void FreeMemoryPages([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("size_t")] nuint allocationCount, [NativeTypeName("VmaAllocation  _Nullable const * _Nonnull")] Allocation** pAllocations);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetAllocationInfo", ExactSpelling = true)]
    public static extern void GetAllocationInfo([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VmaAllocationInfo * _Nonnull")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetAllocationInfo2", ExactSpelling = true)]
    public static extern void GetAllocationInfo2([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VmaAllocationInfo2 * _Nonnull")] VmaAllocationInfo2* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaSetAllocationUserData", ExactSpelling = true)]
    public static extern void SetAllocationUserData([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("void * _Nullable")] void* pUserData);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaSetAllocationName", ExactSpelling = true)]
    public static extern void SetAllocationName([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("const char * _Nullable")] sbyte* pName);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetAllocationMemoryProperties", ExactSpelling = true)]
    public static extern void GetAllocationMemoryProperties([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkMemoryPropertyFlags * _Nonnull")] uint* pFlags);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaMapMemory", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result MapMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("void * _Nullable * _Nonnull")] void** ppData);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaUnmapMemory", ExactSpelling = true)]
    public static extern void UnmapMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFlushAllocation", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result FlushAllocation([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint offset, [NativeTypeName("VkDeviceSize")] nuint size);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaInvalidateAllocation", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result InvalidateAllocation([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint offset, [NativeTypeName("VkDeviceSize")] nuint size);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFlushAllocations", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result FlushAllocations([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint allocationCount, [NativeTypeName("VmaAllocation  _Nonnull const * _Nullable")] Allocation** allocations, [NativeTypeName("const VkDeviceSize * _Nullable")] nuint* offsets, [NativeTypeName("const VkDeviceSize * _Nullable")] nuint* sizes);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaInvalidateAllocations", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result InvalidateAllocations([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint allocationCount, [NativeTypeName("VmaAllocation  _Nonnull const * _Nullable")] Allocation** allocations, [NativeTypeName("const VkDeviceSize * _Nullable")] nuint* offsets, [NativeTypeName("const VkDeviceSize * _Nullable")] nuint* sizes);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCopyMemoryToAllocation", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CopyMemoryToAllocation([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const void * _Nonnull")] void* pSrcHostPointer, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* dstAllocation, [NativeTypeName("VkDeviceSize")] nuint dstAllocationLocalOffset, [NativeTypeName("VkDeviceSize")] nuint size);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCopyAllocationToMemory", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CopyAllocationToMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* srcAllocation, [NativeTypeName("VkDeviceSize")] nuint srcAllocationLocalOffset, [NativeTypeName("void * _Nonnull")] void* pDstHostPointer, [NativeTypeName("VkDeviceSize")] nuint size);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCheckCorruption", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CheckCorruption([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("uint32_t")] uint memoryTypeBits);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBeginDefragmentation", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BeginDefragmentation([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VmaDefragmentationInfo * _Nonnull")] VmaDefragmentationInfo* pInfo, [NativeTypeName("VmaDefragmentationContext  _Nullable * _Nonnull")] VmaDefragmentationContext_T** pContext);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaEndDefragmentation", ExactSpelling = true)]
    public static extern void EndDefragmentation([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaDefragmentationContext _Nonnull")] VmaDefragmentationContext_T* context, [NativeTypeName("VmaDefragmentationStats * _Nullable")] VmaDefragmentationStats* pStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBeginDefragmentationPass", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BeginDefragmentationPass([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaDefragmentationContext _Nonnull")] VmaDefragmentationContext_T* context, [NativeTypeName("VmaDefragmentationPassMoveInfo * _Nonnull")] VmaDefragmentationPassMoveInfo* pPassInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaEndDefragmentationPass", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result EndDefragmentationPass([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaDefragmentationContext _Nonnull")] VmaDefragmentationContext_T* context, [NativeTypeName("VmaDefragmentationPassMoveInfo * _Nonnull")] VmaDefragmentationPassMoveInfo* pPassInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBindBufferMemory", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BindBufferMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkBuffer _Nonnull")] Silk.NET.Vulkan.Buffer buffer);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBindBufferMemory2", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BindBufferMemory2([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint allocationLocalOffset, [NativeTypeName("VkBuffer _Nonnull")] Silk.NET.Vulkan.Buffer buffer, [NativeTypeName("const void * _Nullable")] void* pNext);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBindImageMemory", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BindImageMemory([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkImage _Nonnull")] Image image);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBindImageMemory2", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result BindImageMemory2([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint allocationLocalOffset, [NativeTypeName("VkImage _Nonnull")] Image image, [NativeTypeName("const void * _Nullable")] void* pNext);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateBuffer", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateBuffer([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("VkBuffer  _Nullable * _Nonnull")] Silk.NET.Vulkan.Buffer* pBuffer, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateBufferWithAlignment", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateBufferWithAlignment([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("VkDeviceSize")] nuint minAlignment, [NativeTypeName("VkBuffer  _Nullable * _Nonnull")] Silk.NET.Vulkan.Buffer* pBuffer, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAliasingBuffer", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateAliasingBuffer([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("VkBuffer  _Nullable * _Nonnull")] Silk.NET.Vulkan.Buffer* pBuffer);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAliasingBuffer2", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateAliasingBuffer2([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint allocationLocalOffset, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("VkBuffer  _Nullable * _Nonnull")] Silk.NET.Vulkan.Buffer* pBuffer);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyBuffer", ExactSpelling = true)]
    public static extern void DestroyBuffer([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VkBuffer _Nullable")] Silk.NET.Vulkan.Buffer buffer, [NativeTypeName("VmaAllocation _Nullable")] Allocation* allocation);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateImage", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateImage([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("const VkImageCreateInfo * _Nonnull")] ImageCreateInfo* pImageCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] AllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("VkImage  _Nullable * _Nonnull")] Image* pImage, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] Allocation** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAliasingImage", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateAliasingImage([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("const VkImageCreateInfo * _Nonnull")] ImageCreateInfo* pImageCreateInfo, [NativeTypeName("VkImage  _Nullable * _Nonnull")] Image* pImage);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAliasingImage2", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateAliasingImage2([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VmaAllocation _Nonnull")] Allocation* allocation, [NativeTypeName("VkDeviceSize")] nuint allocationLocalOffset, [NativeTypeName("const VkImageCreateInfo * _Nonnull")] ImageCreateInfo* pImageCreateInfo, [NativeTypeName("VkImage  _Nullable * _Nonnull")] Image* pImage);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyImage", ExactSpelling = true)]
    public static extern void DestroyImage([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("VkImage _Nullable")] Image image, [NativeTypeName("VmaAllocation _Nullable")] Allocation* allocation);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateVirtualBlock", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result CreateVirtualBlock([NativeTypeName("const VmaVirtualBlockCreateInfo * _Nonnull")] VmaVirtualBlockCreateInfo* pCreateInfo, [NativeTypeName("VmaVirtualBlock  _Nullable * _Nonnull")] VmaVirtualBlock_T** pVirtualBlock);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyVirtualBlock", ExactSpelling = true)]
    public static extern void DestroyVirtualBlock([NativeTypeName("VmaVirtualBlock _Nullable")] VmaVirtualBlock_T* virtualBlock);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaIsVirtualBlockEmpty", ExactSpelling = true)]
    [return: NativeTypeName("VkBool32")]
    public static extern uint IsVirtualBlockEmpty([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetVirtualAllocationInfo", ExactSpelling = true)]
    public static extern void GetVirtualAllocationInfo([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("VmaVirtualAllocation _Nonnull")] VmaVirtualAllocation_T* allocation, [NativeTypeName("VmaVirtualAllocationInfo * _Nonnull")] VmaVirtualAllocationInfo* pVirtualAllocInfo);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaVirtualAllocate", ExactSpelling = true)]
    [return: NativeTypeName("VkResult")]
    public static extern Result VirtualAllocate([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("const VmaVirtualAllocationCreateInfo * _Nonnull")] VmaVirtualAllocationCreateInfo* pCreateInfo, [NativeTypeName("VmaVirtualAllocation  _Nullable * _Nonnull")] VmaVirtualAllocation_T** pAllocation, [NativeTypeName("VkDeviceSize * _Nullable")] nuint* pOffset);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaVirtualFree", ExactSpelling = true)]
    public static extern void VirtualFree([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("VmaVirtualAllocation _Nullable")] VmaVirtualAllocation_T* allocation);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaClearVirtualBlock", ExactSpelling = true)]
    public static extern void ClearVirtualBlock([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaSetVirtualAllocationUserData", ExactSpelling = true)]
    public static extern void SetVirtualAllocationUserData([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("VmaVirtualAllocation _Nonnull")] VmaVirtualAllocation_T* allocation, [NativeTypeName("void * _Nullable")] void* pUserData);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaGetVirtualBlockStatistics", ExactSpelling = true)]
    public static extern void GetVirtualBlockStatistics([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("VmaStatistics * _Nonnull")] VmaStatistics* pStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCalculateVirtualBlockStatistics", ExactSpelling = true)]
    public static extern void CalculateVirtualBlockStatistics([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("VmaDetailedStatistics * _Nonnull")] VmaDetailedStatistics* pStats);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBuildVirtualBlockStatsString", ExactSpelling = true)]
    public static extern void BuildVirtualBlockStatsString([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("char * _Nullable * _Nonnull")] sbyte** ppStatsString, [NativeTypeName("VkBool32")] uint detailedMap);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFreeVirtualBlockStatsString", ExactSpelling = true)]
    public static extern void FreeVirtualBlockStatsString([NativeTypeName("VmaVirtualBlock _Nonnull")] VmaVirtualBlock_T* virtualBlock, [NativeTypeName("char * _Nullable")] sbyte* pStatsString);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaBuildStatsString", ExactSpelling = true)]
    public static extern void BuildStatsString([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("char * _Nullable * _Nonnull")] sbyte** ppStatsString, [NativeTypeName("VkBool32")] uint detailedMap);

    [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaFreeStatsString", ExactSpelling = true)]
    public static extern void FreeStatsString([NativeTypeName("VmaAllocator _Nonnull")] Allocator* allocator, [NativeTypeName("char * _Nullable")] sbyte* pStatsString);
}
