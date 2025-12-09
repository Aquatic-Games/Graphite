using Graphite.Exceptions;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly CommandPool _commandPool;
    private readonly CommandBuffer _commandBuffer;
    
    public readonly VkInstance Instance;

    public readonly PhysicalDevice PhysicalDevice;
    public readonly VkDevice Device;

    public readonly Queues Queues;

    public VulkanDevice(Vk vk, VkInstance instance, VulkanSurface surface, PhysicalDevice physicalDevice)
    {
        _vk = vk;
        Instance = instance;
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
            throw new UnsupportedFeatureException(
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
        
        Graphite.Instance.Log("Creating device.");
        _vk.CreateDevice(PhysicalDevice, &deviceInfo, null, out Device).Check("Create device");

        SilkMarshal.Free(pExtensions);
        
        Graphite.Instance.Log("Getting queues.");
        _vk.GetDeviceQueue(Device, Queues.GraphicsIndex, 0, out Queues.Graphics);
        _vk.GetDeviceQueue(Device, Queues.PresentIndex, 0, out Queues.Present);

        CommandPoolCreateInfo commandPoolInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            QueueFamilyIndex = Queues.GraphicsIndex
        };
        
        Graphite.Instance.Log("Creating command pool.");
        _vk.CreateCommandPool(Device, &commandPoolInfo, null, out _commandPool).Check("Create command pool");

        CommandBufferAllocateInfo allocInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandBufferCount = 1,
            CommandPool = _commandPool,
            Level = CommandBufferLevel.Primary
        };
        _vk.AllocateCommandBuffers(Device, &allocInfo, out _commandBuffer);
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new VulkanSwapchain(_vk, this, in info);
    }

    public CommandBuffer BeginCommands()
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit
        };

        _vk.BeginCommandBuffer(_commandBuffer, &beginInfo).Check("Begin command buffer");
        return _commandBuffer;
    }

    public void EndCommands()
    {
        _vk.EndCommandBuffer(_commandBuffer).Check("End command buffer");

        CommandBuffer buffer = _commandBuffer;
        PipelineStageFlags flags = PipelineStageFlags.ColorAttachmentOutputBit;

        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &buffer,
            PWaitDstStageMask = &flags
        };

        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit queue");
        _vk.QueueWaitIdle(Queues.Graphics).Check("Wait for queue idle");
    }

    public override void Dispose()
    {
        _vk.DeviceWaitIdle(Device).Check("Wait for device idle");
        
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.DestroyCommandPool(Device, _commandPool, null);
        _vk.DestroyDevice(Device, null);
    }
}