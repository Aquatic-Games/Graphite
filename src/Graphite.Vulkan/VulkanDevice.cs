using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed class VulkanDevice : Device
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
        
        
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}