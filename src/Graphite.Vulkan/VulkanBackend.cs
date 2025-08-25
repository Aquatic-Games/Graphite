namespace Graphite.Vulkan;

public sealed class VulkanBackend : IBackend
{
    public static string Name => "Vulkan";

    public static Backend Backend => Backend.Vulkan;
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new VulkanInstance(in info);
    }
}