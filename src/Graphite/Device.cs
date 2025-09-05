using System.Runtime.CompilerServices;
using Graphite.Core;

namespace Graphite;

public abstract class Device : IDisposable
{
    /// <summary>
    /// The API <see cref="Graphite.Backend"/> of this device.
    /// </summary>
    public abstract Backend Backend { get; }
    
    /// <summary>
    /// Create a <see cref="Swapchain"/> from the given info.
    /// </summary>
    /// <param name="info">The <see cref="SwapchainInfo"/> to use when creating the swapchain.</param>
    /// <returns>The created <see cref="Swapchain"/>.</returns>
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    /// <summary>
    /// Create a <see cref="CommandList"/>.
    /// </summary>
    /// <returns>The created <see cref="CommandList"/>.</returns>
    public abstract CommandList CreateCommandList();

    /// <summary>
    /// Create a <see cref="ShaderModule"/> from the given backend-specific shader code:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Backend</term>
    ///         <description>Code Type</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Vulkan</term>
    ///         <description>Spir-V</description>
    ///     </item>
    ///     <item>
    ///         <term>D3D12</term>
    ///         <description>DXIL</description>
    ///     </item>
    ///     <item>
    ///         <term>D3D11</term>
    ///         <description>DXBC</description>
    ///     </item>
    ///     <item>
    ///         <term>OpenGL</term>
    ///         <description>GLSL</description>
    ///     </item>
    /// </list>
    /// In order to translate vertex input and descriptors for backends that may not support them, you must provide the
    /// <paramref name="mapping"/> parameter.
    /// </summary>
    /// <param name="code">The shader bytecode/string.</param>
    /// <param name="entryPoint">The entry point of the shader.</param>
    /// <param name="mapping">The shader mapping to other backends, if appropriate.</param>
    /// <returns>The created <see cref="ShaderModule"/>.</returns>
    public abstract ShaderModule CreateShaderModule(byte[] code, string entryPoint, ShaderMappingInfo mapping = default);

    /// <summary>
    /// Create a graphics <see cref="Pipeline"/>.
    /// </summary>
    /// <param name="info">The <see cref="GraphicsPipelineInfo"/> to use when creating the pipeline.</param>
    /// <returns>The created graphics <see cref="Pipeline"/>.</returns>
    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    /// <summary>
    /// Create a <see cref="Buffer"/> with the given info and optional data.
    /// </summary>
    /// <param name="info">The <see cref="BufferInfo"/> to use when creating the buffer.</param>
    /// <param name="data">A pointer to the data, if any. This must be the same size as the <see cref="BufferInfo.SizeInBytes"/>, or null.</param>
    /// <returns>The created <see cref="Buffer"/>.</returns>
    public abstract unsafe Buffer CreateBuffer(in BufferInfo info, void* data);

    /// <summary>
    /// Create a <see cref="Buffer"/> with the given info.
    /// </summary>
    /// <param name="info">The <see cref="BufferInfo"/> to use when creating the buffer.</param>
    /// <returns>The created <see cref="Buffer"/>.</returns>
    /// <remarks>The buffer is not created with any data - while most drivers should zero out the data, this is not a
    /// guarantee, and therefore the contents of the empty buffer are undefined.</remarks>
    public unsafe Buffer CreateBuffer(in BufferInfo info)
        => CreateBuffer(in info, null);

    /// <summary>
    /// Create a <see cref="Buffer"/> with the given info and data.
    /// </summary>
    /// <param name="usage">How the buffer will be used.</param>
    /// <param name="data">The data the buffer should contain.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created <see cref="Buffer"/>.</returns>
    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, T data) where T : unmanaged
        => CreateBuffer(new BufferInfo(usage, (uint) sizeof(T)), Unsafe.AsPointer(ref data));

    /// <summary>
    /// Create a <see cref="Buffer"/> with the given info and data.
    /// </summary>
    /// <param name="usage">How the buffer will be used.</param>
    /// <param name="data">The data the buffer should contain.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created <see cref="Buffer"/>.</returns>
    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(new BufferInfo(usage, (uint) (data.Length * sizeof(T))), pData);
    }

    /// <summary>
    /// Create a <see cref="Buffer"/> with the given info and data.
    /// </summary>
    /// <param name="usage">How the buffer will be used.</param>
    /// <param name="data">The data the buffer should contain.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created <see cref="Buffer"/>.</returns>
    public Buffer CreateBuffer<T>(BufferUsage usage, T[] data) where T : unmanaged
        => CreateBuffer<T>(usage, data.AsSpan());

    /// <summary>
    /// Create a <see cref="Texture"/> from the given info and optional data.
    /// </summary>
    /// <param name="info">The <see cref="TextureInfo"/> to use when creating the texture.</param>
    /// <param name="pData">A pointer to the data to give to the texture, if any.</param>
    /// <returns>The created <see cref="Texture"/>.</returns>
    /// <remarks>The initial data provided will only be uploaded to Mip 0, Layer 0. Use <see cref="UpdateTexture"/> for
    /// finer control.</remarks>
    public abstract unsafe Texture CreateTexture(in TextureInfo info, void* pData);

    /// <summary>
    /// Create a <see cref="Texture"/> from the given info.
    /// </summary>
    /// <param name="info">The <see cref="TextureInfo"/> to use when creating the texture.</param>
    /// <returns>The created <see cref="Texture"/>.</returns>
    public unsafe Texture CreateTexture(in TextureInfo info)
        => CreateTexture(in info, null);

    /// <summary>
    /// Create a <see cref="Texture"/> from the given info and optional data.
    /// </summary>
    /// <param name="info">The <see cref="TextureInfo"/> to use when creating the texture.</param>
    /// <param name="data">The data to give to the texture.</param>
    /// <returns>The created <see cref="Texture"/>.</returns>
    /// <remarks>The initial data provided will only be uploaded to Mip 0, Layer 0. Use <see cref="UpdateTexture"/> for
    /// finer control.</remarks>
    public unsafe Texture CreateTexture<T>(in TextureInfo info, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateTexture(in info, pData);
    }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given info and optional data.
    /// </summary>
    /// <param name="info">The <see cref="TextureInfo"/> to use when creating the texture.</param>
    /// <param name="data">The data to give to the texture.</param>
    /// <returns>The created <see cref="Texture"/>.</returns>
    /// <remarks>The initial data provided will only be uploaded to Mip 0, Layer 0. Use <see cref="UpdateTexture"/> for
    /// finer control.</remarks>
    public Texture CreateTexture<T>(in TextureInfo info, T[] data) where T : unmanaged
        => CreateTexture<T>(in info, data.AsSpan());

    /// <summary>
    /// Create a <see cref="DescriptorLayout"/> with the given <see cref="DescriptorBinding"/>s.
    /// </summary>
    /// <param name="info">The <see cref="DescriptorLayoutInfo"/> to use when creating the layout.</param>
    /// <returns>The created <see cref="DescriptorLayout"/>.</returns>
    public abstract DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutInfo info);

    /// <summary>
    /// Create a <see cref="DescriptorSet"/> for the given <see cref="DescriptorLayout"/>.
    /// </summary>
    /// <param name="layout">The <see cref="DescriptorLayout"/> the descriptor set will use.</param>
    /// <param name="descriptors">The <see cref="Descriptor"/> values.</param>
    /// <returns>The created <see cref="DescriptorSet"/>.</returns>
    public abstract DescriptorSet CreateDescriptorSet(DescriptorLayout layout,
        params ReadOnlySpan<Descriptor> descriptors);

    /// <summary>
    /// Create a <see cref="Sampler"/>.
    /// </summary>
    /// <param name="info">The <see cref="SamplerInfo"/> to use when creating the sampler.</param>
    /// <returns></returns>
    public abstract Sampler CreateSampler(in SamplerInfo info);
    
    /// <summary>
    /// Execute a <see cref="CommandList"/>.
    /// </summary>
    /// <param name="cl">The <see cref="CommandList"/> to execute.</param>
    /// <remarks>This <b>must</b> be called on the graphics thread.<br />
    /// The command list may not necessarily be executed immediately, however they are always executed in  order of
    /// execution.</remarks>
    public abstract void ExecuteCommandList(CommandList cl);

    public abstract unsafe void UpdateBuffer(Buffer buffer, uint offset, uint size, void* pData);

    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offset, T data) where T : unmanaged
        => UpdateBuffer(buffer, offset, (uint) sizeof(T), Unsafe.AsPointer(ref data));
    
    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offset, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateBuffer(buffer, offset, (uint) (data.Length * sizeof(T)), pData);
    }

    public void UpdateBuffer<T>(Buffer buffer, uint offset, T[] data) where T : unmanaged
        => UpdateBuffer<T>(buffer, offset, data.AsSpan());

    public abstract unsafe void UpdateTexture(Texture texture, in Region3D region, void* pData);

    public unsafe void UpdateTexture<T>(Texture texture, in Region3D region, in ReadOnlySpan<T> data)
        where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateTexture(texture, in region, pData);
    }

    public void UpdateTexture<T>(Texture texture, in Region3D region, T[] data) where T : unmanaged
        => UpdateTexture<T>(texture, in region, data.AsSpan());

    /// <summary>
    /// Map a <see cref="Buffer"/> into CPU-addressable memory so it can be written to/read from.
    /// </summary>
    /// <param name="buffer">The <see cref="Buffer"/> to map.</param>
    /// <returns>A pointer in memory to the mapped data.</returns>
    /// <remarks>The buffer must be created with <see cref="BufferUsage.MapWrite"/> and/or
    /// <see cref="BufferUsage.MapRead"/>, otherwise this method will fail.</remarks>
    public abstract nint MapBuffer(Buffer buffer);

    /// <summary>
    /// Unmap a mapped <see cref="Buffer"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Buffer"/> to unmap.</param>
    public abstract void UnmapBuffer(Buffer buffer);
    
    /// <summary>
    /// Dispose of this <see cref="Device"/>.
    /// </summary>
    public abstract void Dispose();
}