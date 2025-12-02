using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkInstance _instance;

    public readonly PhysicalDevice PhysicalDevice;
    public readonly VkDevice Device;

    public readonly Queues Queues;

    public VulkanDevice(Vk vk, VkInstance instance, VulkanSurface surface, PhysicalDevice physicalDevice)
    {
        _vk = vk;
        _instance = instance;
        PhysicalDevice = physicalDevice;

        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueFamilyProperties = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueueFamilies, queueFamilyProperties);

        uint? graphicsFamily = null;
        uint? presentFamily = null;
        for (uint i = 0; i < numQueueFamilies; i++)
        {
            if ((queueFamilyProperties[i].QueueFlags & QueueFlags.GraphicsBit) != 0)
                graphicsFamily = i;

            Bool32 isSupported;
            surface.KhrSurface.GetPhysicalDeviceSurfaceSupport(PhysicalDevice, i, surface.Surface, &isSupported);
            if (isSupported)
                presentFamily = i;

            if (graphicsFamily.HasValue && presentFamily.HasValue)
                break;
        }

        if (!graphicsFamily.HasValue || !presentFamily.HasValue)
        {
            throw new Exception(
                $"Graphics/Presentation queues not supported! Graphics: {graphicsFamily.HasValue}, Present: {presentFamily.HasValue}");
        }

        Queues.GraphicsIndex = graphicsFamily.Value;
        Queues.PresentIndex = presentFamily.Value;

        HashSet<uint> uniqueQueues = Queues.UniqueQueues;
        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueQueues.Count];

        uint queueIndex = 0;
        float priority = 1.0f;
        foreach (uint queue in uniqueQueues)
        {
            queueInfos[queueIndex++] = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueCount = 1,
                QueueFamilyIndex = queue,
                PQueuePriorities = &priority
            };
        }

        string[] extensions = [KhrSwapchain.ExtensionName];
        nint pExtensions = SilkMarshal.StringArrayToPtr(extensions);

        PhysicalDeviceFeatures enabledFeatures = new();
        
        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,

            QueueCreateInfoCount = (uint) uniqueQueues.Count,
            PQueueCreateInfos = queueInfos,
            
            EnabledExtensionCount = (uint) extensions.Length,
            PpEnabledExtensionNames = (byte**) pExtensions,
            
            PEnabledFeatures = &enabledFeatures
        };

        PhysicalDeviceDynamicRenderingFeatures dynamicRendering = new()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };
        deviceInfo.PNext = &dynamicRendering;
        
        Instance.Log("Creating device.");
        _vk.CreateDevice(PhysicalDevice, &deviceInfo, null, out Device).Check("Create device");

        SilkMarshal.Free(pExtensions);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.DestroyDevice(Device, null);
    }
}