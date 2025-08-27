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
            DescriptorBufferInfo* bufferInfos = stackalloc DescriptorBufferInfo[descriptors.Length];
            DescriptorImageInfo* imageInfos = stackalloc DescriptorImageInfo[descriptors.Length];
            WriteDescriptorSet* writeSets = stackalloc WriteDescriptorSet[descriptors.Length];

            PopulateWriteDescriptorSets(in descriptors, Set, writeSets, bufferInfos, imageInfos);

            _vk.UpdateDescriptorSets(_device, (uint) descriptors.Length, writeSets, null);
        }
    }
    
    public override void Dispose()
    {
        // Destroy the pool and therefore free all allocated descriptor sets.
        GraphiteLog.Log("Destroying descriptor pool.");
        _vk.DestroyDescriptorPool(_device, _pool, null);
    }

    public static void PopulateWriteDescriptorSets(in ReadOnlySpan<Descriptor> descriptors, VkDescriptorSet set,
        WriteDescriptorSet* writeSets, DescriptorBufferInfo* bufferInfos, DescriptorImageInfo* imageInfos)
    {
        for (int i = 0; i < descriptors.Length; i++)
        {
            ref readonly Descriptor descriptor = ref descriptors[i];
            
            writeSets[i] = new WriteDescriptorSet
            {
                SType = StructureType.WriteDescriptorSet,
                DescriptorCount = 1,
                DstSet = set,
                DstBinding = descriptor.Binding,
                DescriptorType = descriptor.Type.ToVk(),
            };
            
            if (descriptor.Buffer is { } buffer)
            {
                VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
                DescriptorBufferInfo bufferInfo = new()
                {
                    Buffer = vkBuffer.Buffer,
                    Offset = descriptor.BufferOffset,
                    Range = descriptor.BufferRange == uint.MaxValue ? Vk.WholeSize : descriptor.BufferRange
                };
                bufferInfos[i] = bufferInfo;
                writeSets[i].PBufferInfo = &bufferInfos[i];
            }

            if (descriptor.Texture is { } texture)
            {
                VulkanTexture vkTexture = (VulkanTexture) texture;
                Debug.Assert(vkTexture.IsSampled,
                    "Texture has not been created with the \"TextureUsage.ShaderResource\" flag.");
                Debug.Assert(descriptor.Sampler != null,
                    "DescriptorType.Texture requires a sampler to be provided.");
                
                VulkanSampler vkSampler = (VulkanSampler) descriptor.Sampler;

                DescriptorImageInfo imageInfo = new()
                {
                    ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
                    ImageView = vkTexture.View,
                    Sampler = vkSampler.Sampler
                };
                imageInfos[i] = imageInfo;
                writeSets[i].PImageInfo = &imageInfos[i];
            }
        }
    }
}