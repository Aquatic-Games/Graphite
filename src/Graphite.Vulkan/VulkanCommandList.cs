using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Offset3D = Graphite.Core.Offset3D;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;

    private readonly KhrPushDescriptor? _pushDescriptor;

    private VulkanTexture? _swapchainTexture;
    
    public readonly CommandBuffer Buffer;

    public VulkanCommandList(Vk vk, VkInstance instance, VkDevice device, CommandPool pool)
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

        _vk.TryGetDeviceExtension(instance, device, out _pushDescriptor);
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

    public override void CopyBufferToBuffer(Buffer src, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0)
    {
        VulkanBuffer vkSrc = (VulkanBuffer) src;
        VulkanBuffer vkDest = (VulkanBuffer) dest;

        BufferCopy copy = new()
        {
            SrcOffset = srcOffset,
            DstOffset = destOffset,
            Size = copySize == 0 ? dest.Info.SizeInBytes : copySize
        };

        _vk.CmdCopyBuffer(Buffer, vkSrc.Buffer, vkDest.Buffer, 1, &copy);
    }

    public override void CopyBufferToTexture(Buffer src, uint srcOffset, Texture dest, Region3D? region = null)
    {
        VulkanBuffer vkSrc = (VulkanBuffer) src;
        VulkanTexture vkDest = (VulkanTexture) dest;

        Silk.NET.Vulkan.Offset3D offset;
        Extent3D extent;

        if (region is { } reg)
        {
            offset = reg.Offset.ToVk();
            extent = reg.Size.ToVk();
        }
        else
        {
            offset = new Silk.NET.Vulkan.Offset3D();
            extent = vkDest.Info.Size.ToVk();
        }

        BufferImageCopy copy = new()
        {
            BufferOffset = srcOffset,
            ImageExtent = extent,
            ImageOffset = offset,
            // TODO: Mip level, array layer
            ImageSubresource = new ImageSubresourceLayers
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseArrayLayer = 0,
                LayerCount = 1,
                MipLevel = 0
            }
        };

        vkDest.Transition(Buffer, vkDest.CurrentLayout, ImageLayout.TransferDstOptimal, AccessFlags.TransferWriteBit,
            AccessFlags.TransferReadBit, PipelineStageFlags.TransferBit, PipelineStageFlags.TransferBit);
        _vk.CmdCopyBufferToImage(Buffer, vkSrc.Buffer, vkDest.Image, vkDest.CurrentLayout, 1, &copy);

        if (vkDest.IsSampled)
        {
            vkDest.Transition(Buffer, ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal, AccessFlags.TransferReadBit,
                AccessFlags.ShaderReadBit, PipelineStageFlags.TransferBit, PipelineStageFlags.AllGraphicsBit);
        }
    }

    public override void GenerateMipmaps(Texture texture)
    {
        VulkanTexture vkTexture = (VulkanTexture) texture;

        // TODO: Look into image transitions. I don't quite understand what's going on here. Vulkan sample doesn't seem
        // to explain it in detail, or maybe I've misread it.
        
        vkTexture.Transition(Buffer, vkTexture.CurrentLayout, ImageLayout.TransferSrcOptimal,
            AccessFlags.TransferWriteBit, AccessFlags.TransferReadBit, PipelineStageFlags.TransferBit,
            PipelineStageFlags.TransferBit);

        for (uint i = 1; i < vkTexture.MipLevels; i++)
        {
            ImageBlit blit = new ImageBlit
            {
                SrcSubresource = new ImageSubresourceLayers
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    LayerCount = 1,
                    MipLevel = i - 1
                },
                DstSubresource = new ImageSubresourceLayers
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    LayerCount = 1,
                    MipLevel = i
                }
            };

            // TODO: Look into this. Why is the offset 1?
            blit.SrcOffsets[1].X = (int) (vkTexture.Info.Size.Width >> (int) (i - 1));
            blit.SrcOffsets[1].Y = (int) (vkTexture.Info.Size.Height >> (int) (i - 1));
            blit.SrcOffsets[1].Z = 1;
            
            blit.DstOffsets[1].X = (int) (vkTexture.Info.Size.Width >> (int) i);
            blit.DstOffsets[1].Y = (int) (vkTexture.Info.Size.Height >> (int) i);
            blit.DstOffsets[1].Z = 1;

            vkTexture.Transition(Buffer, ImageLayout.Undefined, ImageLayout.TransferDstOptimal, 0,
                AccessFlags.TransferWriteBit, PipelineStageFlags.TransferBit, PipelineStageFlags.TransferBit,
                baseMipLevel: i);

            _vk.CmdBlitImage(Buffer, vkTexture.Image, ImageLayout.TransferSrcOptimal, vkTexture.Image,
                ImageLayout.TransferDstOptimal, 1, &blit, VkFilter.Linear);

            vkTexture.Transition(Buffer, ImageLayout.TransferDstOptimal, ImageLayout.TransferSrcOptimal,
                AccessFlags.TransferWriteBit, AccessFlags.TransferReadBit, PipelineStageFlags.TransferBit,
                PipelineStageFlags.TransferBit, baseMipLevel: i);
        }

        vkTexture.Transition(Buffer, ImageLayout.TransferSrcOptimal, ImageLayout.ShaderReadOnlyOptimal,
            AccessFlags.TransferReadBit, AccessFlags.TransferWriteBit, PipelineStageFlags.TransferBit,
            PipelineStageFlags.AllGraphicsBit, mipLevels: vkTexture.MipLevels);
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

        Size3D attachmentSize = colorAttachments[0].Texture.Info.Size;
        
        RenderingInfo renderingInfo = new()
        {
            SType = StructureType.RenderingInfo,
            ColorAttachmentCount = (uint) colorAttachments.Length,
            PColorAttachments = colorRenderingAttachments,
            RenderArea = new Rect2D(new Offset2D(0, 0), new Extent2D(attachmentSize.Width, attachmentSize.Height)),
            LayerCount = 1
        };
        
        _vk.CmdBeginRendering(Buffer, &renderingInfo);

        // Vulkan requires a reverse viewport
        // TODO: SetViewport method.
        Viewport viewport = new Viewport(0, attachmentSize.Height, attachmentSize.Width, -attachmentSize.Height, 0, 1);
        _vk.CmdSetViewport(Buffer, 0, 1, &viewport);

        Rect2D scissor = renderingInfo.RenderArea;
        _vk.CmdSetScissor(Buffer, 0, 1, &scissor);
    }
    
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(Buffer);
    }

    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;
        _vk.CmdBindPipeline(Buffer, PipelineBindPoint.Graphics, vkPipeline.Pipeline);
    }

    public override void SetDescriptorSet(uint slot, Pipeline pipeline, DescriptorSet set)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;
        VulkanDescriptorSet vkSet = (VulkanDescriptorSet) set;
        VkDescriptorSet s = vkSet.Set;
        _vk.CmdBindDescriptorSets(Buffer, vkPipeline.BindPoint, vkPipeline.Layout, slot, 1, &s, 0, null);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
        VkBuffer buf = vkBuffer.Buffer;
        // eeewwww
        ulong lStride = stride;
        ulong lOffset = offset;
        _vk.CmdBindVertexBuffers2(Buffer, slot, 1, &buf, &lOffset, null, &lStride);
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;

        IndexType type = format switch
        {
            Format.R16_UInt => IndexType.Uint16,
            Format.R32_UInt => IndexType.Uint32,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
        
        _vk.CmdBindIndexBuffer(Buffer, vkBuffer.Buffer, offset, type);
    }

    public override void PushDescriptors(uint slot, Pipeline pipeline, params ReadOnlySpan<Descriptor> descriptors)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;

        DescriptorBufferInfo* bufferInfos = stackalloc DescriptorBufferInfo[descriptors.Length];
        DescriptorImageInfo* imageInfos = stackalloc DescriptorImageInfo[descriptors.Length];
        WriteDescriptorSet* writeSets = stackalloc WriteDescriptorSet[descriptors.Length];

        VulkanDescriptorSet.PopulateWriteDescriptorSets(in descriptors, new VkDescriptorSet(), writeSets, bufferInfos,
            imageInfos);

        _pushDescriptor!.CmdPushDescriptorSet(Buffer, vkPipeline.BindPoint, vkPipeline.Layout, slot,
            (uint) descriptors.Length, writeSets);
    }

    public override void Draw(uint numVertices, uint firstVertex = 0)
    {
        _vk.CmdDraw(Buffer, numVertices, 1, firstVertex, 0);
    }

    public override void DrawIndexed(uint numIndices, uint firstIndex = 0, int baseVertex = 0)
    {
        _vk.CmdDrawIndexed(Buffer, numIndices, 1, firstIndex, baseVertex, 0);
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Freeing command buffer.");
        _vk.FreeCommandBuffers(_device, _pool, 1, in Buffer);
    }
}