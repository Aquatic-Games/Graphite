global using VkDevice = Silk.NET.Vulkan.Device;
using Graphite.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    
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
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new VulkanSwapchain(_vk, this, in info);
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Destroying device.");
        _vk.DestroyDevice(Device, null);
    }
}