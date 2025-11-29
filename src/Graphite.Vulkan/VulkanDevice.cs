using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed class VulkanDevice : Device
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;

    public readonly VkDevice Device;

    public VulkanDevice(Vk vk, VkInstance instance, VulkanSurface surface, PhysicalDevice physicalDevice)
    {
        
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}