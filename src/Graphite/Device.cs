namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);
    
    public abstract void Dispose();
}