using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;

    private VulkanTexture? _swapchainTexture;
    
    public readonly CommandBuffer Buffer;

    public VulkanCommandList(Vk vk, VkDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _pool = pool;

        CommandBufferAllocateInfo allocInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandBufferCount = 1,
            CommandPool = _pool,
            Level = CommandBufferLevel.Primary
        };
        
        GraphiteLog.Log("Allocating command buffer.");
        _vk.AllocateCommandBuffers(_device, &allocInfo, out Buffer).Check("Allocate command buffer");
    }

    public override void Begin()
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo
        };
        
        _vk.BeginCommandBuffer(Buffer, &beginInfo).Check("Begin command buffer");
    }
    
    public override void End()
    {
        if (_swapchainTexture != null && _swapchainTexture.CurrentLayout != ImageLayout.PresentSrcKhr)
            _swapchainTexture.Transition(Buffer, ImageLayout.PresentSrcKhr);

        _swapchainTexture = null;
        _vk.EndCommandBuffer(Buffer).Check("End command buffer");
    }

    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        RenderingAttachmentInfo* colorRenderingAttachments = stackalloc RenderingAttachmentInfo[colorAttachments.Length];
        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            VulkanTexture texture = (VulkanTexture) attachment.Texture;
            ColorF color = attachment.ClearColor;
            
            if (texture.CurrentLayout != ImageLayout.ColorAttachmentOptimal)
                texture.Transition(Buffer, ImageLayout.ColorAttachmentOptimal);

            if (texture.IsSwapchainTexture)
                _swapchainTexture = texture;

            colorRenderingAttachments[i] = new RenderingAttachmentInfo
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.View,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue = new ClearValue(new ClearColorValue(color.R, color.G, color.B, color.A)),
                LoadOp = attachment.LoadOp.ToVk(),
                StoreOp = attachment.StoreOp.ToVk()
            };
        }

        Size2D attachmentSize = colorAttachments[0].Texture.Size;
        
        RenderingInfo renderingInfo = new()
        {
            SType = StructureType.RenderingInfo,
            ColorAttachmentCount = (uint) colorAttachments.Length,
            PColorAttachments = colorRenderingAttachments,
            RenderArea = new Rect2D(new Offset2D(0, 0), new Extent2D(attachmentSize.Width, attachmentSize.Height)),
            LayerCount = 1
        };
        
        _vk.CmdBeginRendering(Buffer, &renderingInfo);
    }
    
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(Buffer);
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Freeing command buffer.");
        _vk.FreeCommandBuffers(_device, _pool, 1, in Buffer);
    }
}