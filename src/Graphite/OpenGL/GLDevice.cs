using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLDevice : Device
{
    private readonly GL _gl;
    private readonly GLContext _context;
    
    public override Backend Backend => Backend.OpenGL;

    public GLDevice(GL gl, GLContext context)
    {
        _gl = gl;
        _context = context;
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }
    
    public override ShaderModule CreateShaderModule(byte[] code, string entryPoint, ShaderMappingInfo mapping = default)
    {
        throw new NotImplementedException();
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* data)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe Texture CreateTexture(in TextureInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorLayout CreateDescriptorLayout(params ReadOnlySpan<DescriptorBinding> bindings)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params ReadOnlySpan<Descriptor> descriptors)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override IntPtr MapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}