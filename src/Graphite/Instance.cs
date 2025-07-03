using Graphite.Vulkan;

namespace Graphite;

public abstract class Instance : IDisposable
{
    public abstract void Dispose();

    public static Instance Create(in InstanceInfo info)
    {
        return new VulkanInstance(in info);
    }
}