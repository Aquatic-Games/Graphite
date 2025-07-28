namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(byte[] data, string entryPoint);

    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract void ExecuteCommandList(CommandList cl);
    
    public abstract void Dispose();
}