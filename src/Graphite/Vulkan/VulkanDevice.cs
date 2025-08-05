global using VkDevice = Silk.NET.Vulkan.Device;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Graphite.Core;
using Graphite.VulkanMemoryAllocator;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    private readonly CommandPool _pool;
    private readonly CommandBuffer _singleTimeBuffer;
    private readonly Allocator* _allocator;
    
    public readonly VkInstance Instance;
    
    public readonly PhysicalDevice PhysicalDevice;
    public readonly Queues Queues;
    public readonly VkDevice Device;

    public override Backend Backend => Backend.Vulkan;

    public VulkanDevice(Vk vk, VkInstance instance, VulkanSurface surface, PhysicalDevice physicalDevice)
    {
        _vk = vk;
        Instance = instance;
        PhysicalDevice = physicalDevice;

        uint? graphicsQueue = null;
        uint? presentQueue = null;

        uint numQueues;
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueues, null);
        QueueFamilyProperties* queues = stackalloc QueueFamilyProperties[(int) numQueues];
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueues, queues);

        for (uint i = 0; i < numQueues; i++)
        {
            if ((queues[i].QueueFlags & QueueFlags.GraphicsBit) != 0)
                graphicsQueue = i;

            surface.SurfaceExt.GetPhysicalDeviceSurfaceSupport(PhysicalDevice, i, surface.Surface,
                out Bool32 supported);

            if (supported)
                presentQueue = i;

            if (graphicsQueue.HasValue && presentQueue.HasValue)
                break;
        }

        if (!graphicsQueue.HasValue || !presentQueue.HasValue)
        {
            throw new NotSupportedException(
                "Graphics/Present queue not present for the current adapter. Try a different adapter.");
        }

        Queues.GraphicsIndex = graphicsQueue.Value;
        Queues.PresentIndex = presentQueue.Value;

        HashSet<uint> uniqueFamilies = Queues.UniqueQueues;
        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueFamilies.Count];

        uint currentQueue = 0;
        float queuePriority = 1.0f;
        foreach (uint family in uniqueFamilies)
        {
            queueInfos[currentQueue++] = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                PQueuePriorities = &queuePriority,
                QueueCount = 1,
                QueueFamilyIndex = family
            };
        }

        using Utf8StringArray pExtensions = new Utf8StringArray(KhrSwapchain.ExtensionName);

        PhysicalDeviceFeatures deviceFeatures = new();

        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,

            QueueCreateInfoCount = (uint) uniqueFamilies.Count,
            PQueueCreateInfos = queueInfos,

            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions,

            PEnabledFeatures = &deviceFeatures
        };

        PhysicalDeviceDynamicRenderingFeatures dynamicRendering = new()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };
        deviceInfo.PNext = &dynamicRendering;
        
        GraphiteLog.Log("Creating device.");
        _vk.CreateDevice(PhysicalDevice, &deviceInfo, null, out Device).Check("Create device");
        
        GraphiteLog.Log("Getting queues.");
        _vk.GetDeviceQueue(Device, Queues.GraphicsIndex, 0, out Queues.Graphics);
        _vk.GetDeviceQueue(Device, Queues.PresentIndex, 0, out Queues.Present);

        CommandPoolCreateInfo poolInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = Queues.GraphicsIndex,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        GraphiteLog.Log("Creating command pool.");
        _vk.CreateCommandPool(Device, &poolInfo, null, out _pool).Check("Create command pool");

        CommandBufferAllocateInfo allocInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandBufferCount = 1,
            CommandPool = _pool,
            Level = CommandBufferLevel.Primary
        };
        GraphiteLog.Log("Allocating single-time buffer.");
        _vk.AllocateCommandBuffers(Device, &allocInfo, out _singleTimeBuffer);

        VmaVulkanFunctions functions = new()
        {
            vkGetInstanceProcAddr =
                (delegate* unmanaged[Cdecl]<VkInstance, sbyte*, delegate* unmanaged[Cdecl]<void>>) SilkMarshal
                    .DelegateToPtr(GetInstanceProcAddrFunc),
            vkGetDeviceProcAddr =
                (delegate* unmanaged[Cdecl]<VkDevice, sbyte*, delegate* unmanaged[Cdecl]<void>>) SilkMarshal
                    .DelegateToPtr(GetDeviceProcAddrFunc)
        };
        
        AllocatorCreateInfo allocatorInfo = new()
        {
            instance = Instance,
            device = Device,
            physicalDevice = PhysicalDevice,
            pVulkanFunctions = &functions,
            vulkanApiVersion = Vk.Version13
        };
        
        GraphiteLog.Log("Creating allocator.");
        Vma.CreateAllocator(&allocatorInfo, out _allocator).Check("Create allocator");
    }

    public CommandBuffer BeginCommands()
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit
        };
        
        _vk.BeginCommandBuffer(_singleTimeBuffer, &beginInfo).Check("Begin single-time command buffer");
        return _singleTimeBuffer;
    }

    public void EndCommands()
    {
        _vk.EndCommandBuffer(_singleTimeBuffer).Check("End single-time command buffer");
        CommandBuffer cb = _singleTimeBuffer;
        
        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &cb
        };

        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit single-time command buffer");
        _vk.QueueWaitIdle(Queues.Graphics).Check("Wait for queue to idle");
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new VulkanSwapchain(_vk, this, in info);
    }

    public override CommandList CreateCommandList()
    {
        return new VulkanCommandList(_vk, Device, _pool);
    }

    public override ShaderModule CreateShaderModule(byte[] code, string entryPoint)
    {
        return new VulkanShaderModule(_vk, Device, code, entryPoint);
    }

    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new VulkanPipeline(_vk, Device, in info);
    }

    public override Buffer CreateBuffer(in BufferInfo info, void* data)
    {
        return new VulkanBuffer(_vk, this, _allocator, in info, data);
    }

    public override DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings)
    {
        return new VulkanDescriptorLayout(_vk, Device, bindings);
    }

    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params ReadOnlySpan<Descriptor> descriptors)
    {
        return new VulkanDescriptorSet(_vk, Device, layout, descriptors);
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        VulkanCommandList vulkanCl = (VulkanCommandList) cl;
        CommandBuffer buffer = vulkanCl.Buffer;

        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &buffer
        };

        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit queue");
        // TODO: Obviously waiting for the queue to idle is not a good way of synchronization.
        // Use semaphores.
        _vk.QueueWaitIdle(Queues.Graphics).Check("Wait for queue idle");
    }

    public override IntPtr MapBuffer(Buffer buffer)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
        Debug.Assert(vkBuffer.IsMappable);
        
        void* mappedMemory;
        Vma.MapMemory(_allocator, vkBuffer.Allocation, &mappedMemory).Check("Map memory");
        return (nint) mappedMemory;
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
        Vma.UnmapMemory(_allocator, vkBuffer.Allocation);
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Destroying allocator.");
        Vma.DestroyAllocator(_allocator);
        
        GraphiteLog.Log("Destroying command pool.");
        _vk.DestroyCommandPool(Device, _pool, null);
        
        GraphiteLog.Log("Destroying device.");
        _vk.DestroyDevice(Device, null);
    }

    private nint GetInstanceProcAddrFunc(VkInstance instance, byte* pName)
    {
        return _vk.GetInstanceProcAddr(instance, pName);
    }
    
    private nint GetDeviceProcAddrFunc(VkDevice device, byte* pName)
    {
        return _vk.GetDeviceProcAddr(device, pName);
    }
}