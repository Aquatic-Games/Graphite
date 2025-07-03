using Graphite.Vulkan;

namespace Graphite;

public abstract class Instance : IDisposable
{
    public abstract Adapter[] EnumerateAdapters();

    public abstract Surface CreateSurface(in SurfaceInfo info);
    
    public abstract void Dispose();

    public static Instance Create(in InstanceInfo info)
    {
        return new VulkanInstance(in info);
    }
}