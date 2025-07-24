global using VkDevice = Silk.NET.Vulkan.Device;
using System.Runtime.CompilerServices;
using Graphite.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    private readonly CommandPool _pool;
    private readonly CommandBuffer _singleTimeBuffer;
    
    public readonly VkInstance Instance;
    
    public readonly PhysicalDevice PhysicalDevice;
    public readonly Queues Queues;
    public readonly VkDevice Device;
    
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

    public override void Dispose()
    {
        GraphiteLog.Log("Destroying command pool.");
        _vk.DestroyCommandPool(Device, _pool, null);
        
        GraphiteLog.Log("Destroying device.");
        _vk.DestroyDevice(Device, null);
    }
}