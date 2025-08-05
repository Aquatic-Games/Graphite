using System.Runtime.CompilerServices;

namespace Graphite;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(byte[] data, string entryPoint);

    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract unsafe Buffer CreateBuffer(in BufferInfo info, void* data);

    public unsafe Buffer CreateBuffer(in BufferInfo info)
        => CreateBuffer(in info, null);

    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, T data) where T : unmanaged
        => CreateBuffer(new BufferInfo(usage, (uint) sizeof(T)), Unsafe.AsPointer(ref data));

    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(new BufferInfo(usage, (uint) (data.Length * sizeof(T))), pData);
    }

    public Buffer CreateBuffer<T>(BufferUsage usage, T[] data) where T : unmanaged
        => CreateBuffer<T>(usage, data.AsSpan());

    public abstract DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings);

    public abstract DescriptorSet CreateDescriptorSet(DescriptorLayout layout,
        params ReadOnlySpan<Descriptor> descriptors);

    public abstract void ExecuteCommandList(CommandList cl);

    public abstract nint MapBuffer(Buffer buffer);

    public abstract void UnmapBuffer(Buffer buffer);
    
    public abstract void Dispose();
}