global using VkDescriptorSet = Silk.NET.Vulkan.DescriptorSet;
using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDescriptorSet : DescriptorSet
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    private readonly DescriptorPool _pool;

    public readonly VkDescriptorSet Set;
    
    public VulkanDescriptorSet(Vk vk, VkDevice device, ReadOnlySpan<DescriptorLayout> layouts)
    {
        _vk = vk;
        _device = device;

        Dictionary<VkDescriptorType, DescriptorPoolSize> poolSizes = [];
        
        foreach (DescriptorLayout layout in layouts)
        {
            VulkanDescriptorLayout vkLayout = (VulkanDescriptorLayout) layout;
            if (!poolSizes.TryGetValue(vkLayout.Type, out DescriptorPoolSize size))
                size = new DescriptorPoolSize();

            size.Type = vkLayout.Type;
            size.DescriptorCount += 1;

            poolSizes[vkLayout.Type] = size;
        }

        DescriptorPoolSize[] pools = poolSizes.Values.ToArray();

        fixed (DescriptorPoolSize* pPools = pools)
        {
            DescriptorPoolCreateInfo poolInfo = new()
            {
                SType = StructureType.DescriptorPoolCreateInfo,
                MaxSets = 1,
                PoolSizeCount = (uint) poolSizes.Count,
                PPoolSizes = pPools
            };
            
            GraphiteLog.Log("Creating descriptor pool.");
            _vk.CreateDescriptorPool(_device, &poolInfo, null, out _pool).Check("Create descriptor pool");
        }

        DescriptorSetLayout* vkLayouts = stackalloc DescriptorSetLayout[layouts.Length];
        for (int i = 0; i < layouts.Length; i++)
            vkLayouts[i] = ((VulkanDescriptorLayout) layouts[i]).Layout;

        DescriptorSetAllocateInfo allocInfo = new()
        {
            SType = StructureType.DescriptorSetAllocateInfo,
            DescriptorPool = _pool,
            DescriptorSetCount = 1,
            PSetLayouts = vkLayouts
        };
        
        GraphiteLog.Log("Allocating descriptor set.");
        _vk.AllocateDescriptorSets(_device, &allocInfo, out Set).Check("Allocate descriptor set");
    }
    
    public override void Dispose()
    {
        // Destroy the pool and therefore free all allocated descriptor sets.
        GraphiteLog.Log("Destroying descriptor pool.");
        _vk.DestroyDescriptorPool(_device, _pool, null);
    }
}