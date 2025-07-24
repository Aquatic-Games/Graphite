namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    public abstract CommandList CreateCommandList();
    
    public abstract void Dispose();
}