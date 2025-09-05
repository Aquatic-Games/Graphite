using Graphite.Core;
using Silk.NET.OpenGL;

namespace Graphite.OpenGL;

internal sealed class GLDevice : Device
{
    private readonly GL _gl;
    private readonly GLContext _context;

    private readonly Dictionary<int, uint> _framebufferCache;

    public override Backend Backend => OpenGLBackend.Backend;
    
    public GLDevice(GL gl, GLContext context)
    {
        _gl = gl;
        _context = context;

        _framebufferCache = [];
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new GLSwapchain(_gl, _context, in info);
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
    
    public override unsafe Texture CreateTexture(in TextureInfo info, void* pData)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params ReadOnlySpan<Descriptor> descriptors)
    {
        throw new NotImplementedException();
    }
    
    public override Sampler CreateSampler(in SamplerInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe void UpdateBuffer(Buffer buffer, uint offset, uint size, void* pData)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe void UpdateTexture(Texture texture, in Region3D region, void* pData)
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
    
    public override void Dispose() { }

    public uint GetFramebuffer(ReadOnlySpan<GLTexture> colorAttachments, GLTexture? depthAttachment = null)
    {
        HashCode code = new HashCode();
        
        foreach (GLTexture texture in colorAttachments)
            code.Add(texture);

        if (depthAttachment != null)
            code.Add(depthAttachment);

        int hashCode = code.ToHashCode();
        
        if (!_framebufferCache.TryGetValue(hashCode, out uint framebuffer))
        {
            GraphiteLog.Log($"Creating framebuffer {hashCode}.");
            framebuffer = _gl.CreateFramebuffer();

            foreach (GLTexture texture in colorAttachments)
            {
                _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, texture.Texture, 0);
            }

            if (depthAttachment != null)
                throw new NotImplementedException();
            
            _framebufferCache.Add(hashCode, framebuffer);
        }
        
        return framebuffer;
    }
}