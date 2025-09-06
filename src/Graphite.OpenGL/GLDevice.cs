using Graphite.Core;
using Graphite.OpenGL.Instructions;
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
        return new GLCommandList();
    }
    
    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint,
        ShaderMappingInfo mapping = default)
    {
        return new GLShaderModule(_gl, stage, code);
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new GLPipeline(_gl, in info);
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
        GLCommandList glList = (GLCommandList) cl;

        foreach (IInstruction instruction in glList.Instructions)
        {
            switch (instruction)
            {
                case BeginRenderPassInstruction renderPass:
                {
                    uint framebuffer = GetFramebuffer(renderPass.ColorAttachments);
                    _gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

                    _gl.ClearColor(renderPass.ClearColor.R, renderPass.ClearColor.G, renderPass.ClearColor.B,
                        renderPass.ClearColor.A);
                    _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            foreach (GLTexture texture in colorAttachments)
            {
                _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, texture.Texture, 0);
            }

            if (depthAttachment != null)
                throw new NotImplementedException();

            if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
                throw new Exception("Framebuffer is not complete!");
            
            _framebufferCache.Add(hashCode, framebuffer);
        }
        
        return framebuffer;
    }
}