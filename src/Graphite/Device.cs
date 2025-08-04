namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(byte[] data, string entryPoint);

    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract Buffer CreateBuffer(in BufferInfo info);

    public abstract DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings);

    public abstract DescriptorSet CreateDescriptorSet(DescriptorLayout layout,
        params ReadOnlySpan<Descriptor> descriptors);

    public abstract void ExecuteCommandList(CommandList cl);

    public abstract nint MapBuffer(Buffer buffer);

    public abstract void UnmapBuffer(Buffer buffer);
    
    public abstract void Dispose();
}