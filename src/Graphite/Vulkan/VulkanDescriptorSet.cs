global using VkDescriptorSet = Silk.NET.Vulkan.DescriptorSet;
using System.Diagnostics;
using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDescriptorSet : DescriptorSet
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    private readonly DescriptorPool _pool;
    private readonly Sampler _sampler;

    public readonly VkDescriptorSet Set;
    
    public VulkanDescriptorSet(Vk vk, VkDevice device, DescriptorLayout layout, ReadOnlySpan<Descriptor> descriptors)
    {
        _vk = vk;
        _device = device;

        VulkanDescriptorLayout vkLayout = (VulkanDescriptorLayout) layout;
        DescriptorSetLayout setLayout = vkLayout.Layout;
        
        int numPools = vkLayout.DescriptorCounts.Count;
        DescriptorPoolSize* pools = stackalloc DescriptorPoolSize[numPools];

        int countIndex = 0;
        foreach ((VkDescriptorType type, uint count) in vkLayout.DescriptorCounts)
        {
            pools[countIndex++] = new DescriptorPoolSize
            {
                Type = type,
                DescriptorCount = count
            };
        }
        
        DescriptorPoolCreateInfo poolInfo = new()
        {
            SType = StructureType.DescriptorPoolCreateInfo,
            MaxSets = 1,
            PoolSizeCount = (uint) numPools,
            PPoolSizes = pools
        };
        
        GraphiteLog.Log("Creating descriptor pool.");
        _vk.CreateDescriptorPool(_device, &poolInfo, null, out _pool).Check("Create descriptor pool");
        
        DescriptorSetAllocateInfo allocInfo = new()
        {
            SType = StructureType.DescriptorSetAllocateInfo,
            DescriptorPool = _pool,
            DescriptorSetCount = 1,
            PSetLayouts = &setLayout
        };
        
        GraphiteLog.Log("Allocating descriptor set.");
        _vk.AllocateDescriptorSets(_device, &allocInfo, out Set).Check("Allocate descriptor set");

        if (descriptors.Length > 0)
        {
            WriteDescriptorSet* writeSets = stackalloc WriteDescriptorSet[descriptors.Length];

            for (int i = 0; i < descriptors.Length; i++)
            {
                ref readonly Descriptor descriptor = ref descriptors[i];
                DescriptorBufferInfo bufferInfo;
                DescriptorImageInfo imageInfo;
                
                writeSets[i] = new WriteDescriptorSet
                {
                    SType = StructureType.WriteDescriptorSet,
                    DescriptorCount = 1,
                    DstSet = Set,
                    DstBinding = descriptor.Binding,
                    DescriptorType = descriptor.Type.ToVk(),
                };
                
                if (descriptor.Buffer is { } buffer)
                {
                    VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
                    bufferInfo.Buffer = vkBuffer.Buffer;
                    bufferInfo.Offset = descriptor.BufferOffset;
                    bufferInfo.Range = descriptor.BufferRange == uint.MaxValue ? Vk.WholeSize : descriptor.BufferRange;
                    writeSets[i].PBufferInfo = &bufferInfo;
                }

                if (descriptor.Texture is { } texture)
                {
                    SamplerCreateInfo samplerInfo = new()
                    {
                        SType = StructureType.SamplerCreateInfo,
                        MagFilter = Filter.Linear,
                        MinFilter = Filter.Linear,
                        MipmapMode = SamplerMipmapMode.Linear,
                        AddressModeU = SamplerAddressMode.Repeat,
                        AddressModeW = SamplerAddressMode.Repeat
                    };
                    
                    // Create a temporary sampler
                    // TODO: Samplers
                    GraphiteLog.Log("Creating temporary sampler.");
                    _vk.CreateSampler(_device, &samplerInfo, null, out _sampler).Check("Create sampler");
                    
                    VulkanTexture vkTexture = (VulkanTexture) texture;
                    Debug.Assert(vkTexture.IsSampled,
                        "Texture has not been created with the \"TextureUsage.ShaderResource\" flag.");
                    
                    imageInfo.ImageLayout = ImageLayout.ShaderReadOnlyOptimal;
                    imageInfo.ImageView = vkTexture.View;
                    imageInfo.Sampler = _sampler;
                    writeSets[i].PImageInfo = &imageInfo;
                }
            }

            _vk.UpdateDescriptorSets(_device, (uint) descriptors.Length, writeSets, null);
        }
    }
    
    public override void Dispose()
    {
        if (_sampler.Handle != 0)
            _vk.DestroySampler(_device, _sampler, null);
        
        // Destroy the pool and therefore free all allocated descriptor sets.
        GraphiteLog.Log("Destroying descriptor pool.");
        _vk.DestroyDescriptorPool(_device, _pool, null);
    }
}