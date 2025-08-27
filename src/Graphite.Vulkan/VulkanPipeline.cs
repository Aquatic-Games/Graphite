global using VkPipeline = Silk.NET.Vulkan.Pipeline;
using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanPipeline : Pipeline
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly PipelineLayout Layout;
    public readonly VkPipeline Pipeline;

    public readonly PipelineBindPoint BindPoint;
    
    public VulkanPipeline(Vk vk, VkDevice device, ref readonly GraphicsPipelineInfo info)
    {
        _vk = vk;
        _device = device;
        BindPoint = PipelineBindPoint.Graphics;

        DescriptorSetLayout* descriptorLayouts = stackalloc DescriptorSetLayout[info.Descriptors.Length];
        for (int i = 0; i < info.Descriptors.Length; i++)
            descriptorLayouts[i] = ((VulkanDescriptorLayout) info.Descriptors[i]).Layout;
        
        PipelineLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.PipelineLayoutCreateInfo,
            SetLayoutCount = (uint) info.Descriptors.Length,
            PSetLayouts = descriptorLayouts
        };
        
        GraphiteLog.Log("Creating pipeline layout.");
        _vk.CreatePipelineLayout(_device, &layoutInfo, null, out Layout).Check("Create pipeline layout");

        VulkanShaderModule vertexShader = (VulkanShaderModule) info.VertexShader;
        using Utf8String pVertexEntryPoint = vertexShader.EntryPoint;

        VulkanShaderModule pixelShader = (VulkanShaderModule) info.PixelShader;
        using Utf8String pPixelEntryPoint = pixelShader.EntryPoint;

        const int numShaderStages = 2;
        PipelineShaderStageCreateInfo* shaderStages = stackalloc PipelineShaderStageCreateInfo[numShaderStages];
        shaderStages[0] = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.VertexBit,
            Module = vertexShader.Module,
            PName = pVertexEntryPoint
        };
        shaderStages[1] = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.FragmentBit,
            Module = pixelShader.Module,
            PName = pPixelEntryPoint
        };

        VertexInputAttributeDescription* vertexAttributes =
            stackalloc VertexInputAttributeDescription[info.InputLayout.Length];

        for (int i = 0; i < info.InputLayout.Length; i++)
        {
            ref readonly InputElementDescription element = ref info.InputLayout[i];

            vertexAttributes[i] = new VertexInputAttributeDescription
            {
                Format = element.Format.ToVk(),
                Offset = element.Offset,
                Location = element.Location,
                Binding = element.Slot
            };
        }

        VertexInputBindingDescription vertexBinding = new()
        {
            Binding = 0,
            InputRate = VertexInputRate.Vertex
        };
        
        PipelineVertexInputStateCreateInfo vertexInputState = new()
        {
            SType = StructureType.PipelineVertexInputStateCreateInfo,
            VertexAttributeDescriptionCount = (uint) info.InputLayout.Length,
            PVertexAttributeDescriptions = vertexAttributes,
            VertexBindingDescriptionCount = 1,
            PVertexBindingDescriptions = &vertexBinding
        };

        PipelineInputAssemblyStateCreateInfo inputAssemblyState = new()
        {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            // TODO: Primitive topology
            Topology = PrimitiveTopology.TriangleList
        };

        PipelineViewportStateCreateInfo viewportState = new()
        {
            SType = StructureType.PipelineViewportStateCreateInfo,
            ViewportCount = 1,
            ScissorCount = 1 // TODO: Enable scissor rectangle if enabled in rasterizer state?
        };

        PipelineRasterizationStateCreateInfo rasterizationState = new()
        {
            SType = StructureType.PipelineRasterizationStateCreateInfo,
            CullMode = CullModeFlags.None,
            PolygonMode = PolygonMode.Fill,
            LineWidth = 1.0f
        };

        PipelineMultisampleStateCreateInfo multisampleState = new()
        {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            RasterizationSamples = SampleCountFlags.Count1Bit
        };

        PipelineDepthStencilStateCreateInfo depthStencilState = new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
            DepthTestEnable = false
        };

        PipelineColorBlendAttachmentState* blendAttachments =
            stackalloc PipelineColorBlendAttachmentState[info.ColorTargets.Length];
        VkFormat* formats = stackalloc VkFormat[info.ColorTargets.Length];

        for (int i = 0; i < info.ColorTargets.Length; i++)
        {
            blendAttachments[i] = new PipelineColorBlendAttachmentState
            {
                BlendEnable = false,
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit |
                                 ColorComponentFlags.ABit
            };

            formats[i] = info.ColorTargets[i].Format.ToVk();
        }

        PipelineColorBlendStateCreateInfo colorBlendState = new()
        {
            SType = StructureType.PipelineColorBlendStateCreateInfo,
            AttachmentCount = (uint) info.ColorTargets.Length,
            PAttachments = blendAttachments
        };

        PipelineRenderingCreateInfo renderingInfo = new()
        {
            SType = StructureType.PipelineRenderingCreateInfo,
            ColorAttachmentCount = (uint) info.ColorTargets.Length,
            PColorAttachmentFormats = formats
        };

        uint numDynamicStates = 2;
        DynamicState* states = stackalloc DynamicState[3];
        states[0] = DynamicState.Viewport;
        states[1] = DynamicState.Scissor;

        // Only set the dynamic state if an input layout is defined (meaning that a vertex buffer will be bound).
        // The vulkan spec requires a vertex buffer to be bound if this dynamic state is set, meaning that a draw
        // command may fail if a vertex buffer has not previously been bound.
        if (info.InputLayout.Length > 0)
        {
            numDynamicStates = 3;
            states[2] = DynamicState.VertexInputBindingStride;
        }

        PipelineDynamicStateCreateInfo dynamicState = new()
        {
            SType = StructureType.PipelineDynamicStateCreateInfo,
            DynamicStateCount = numDynamicStates,
            PDynamicStates = states
        };

        GraphicsPipelineCreateInfo pipelineInfo = new()
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            Layout = Layout,
            
            StageCount = numShaderStages,
            PStages = shaderStages,
            
            PVertexInputState = &vertexInputState,
            PInputAssemblyState = &inputAssemblyState,
            PViewportState = &viewportState,
            
            PRasterizationState = &rasterizationState,
            PMultisampleState = &multisampleState,
            PDepthStencilState = &depthStencilState,
            PColorBlendState = &colorBlendState,
            /* PRenderingInfo = */ PNext = &renderingInfo,
            PDynamicState = &dynamicState
        };

        GraphiteLog.Log("Creating pipeline.");
        _vk.CreateGraphicsPipelines(_device, new PipelineCache(), 1, &pipelineInfo, null, out Pipeline).Check("Create pipeline");
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying pipeline.");
        _vk.DestroyPipeline(_device, Pipeline, null);
        
        GraphiteLog.Log("Destroying pipeline layout.");
        _vk.DestroyPipelineLayout(_device, Layout, null);
    }
}