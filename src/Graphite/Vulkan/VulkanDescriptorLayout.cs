using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal unsafe class VulkanDescriptorLayout : DescriptorLayout
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public DescriptorSetLayout Layout;
    
    public VulkanDescriptorLayout(Vk vk, VkDevice device, ReadOnlySpan<DescriptorBinding> bindings)
    {
        _vk = vk;
        _device = device;

        DescriptorSetLayoutBinding* vkBindings = stackalloc DescriptorSetLayoutBinding[bindings.Length];
        for (int i = 0; i < bindings.Length; i++)
        {
            ref readonly DescriptorBinding binding = ref bindings[i];

            ShaderStageFlags shaderFlags = ShaderStageFlags.None;

            if ((binding.Stages & ShaderStage.Vertex) != 0)
                shaderFlags |= ShaderStageFlags.VertexBit;
            if ((binding.Stages & ShaderStage.Pixel) != 0)
                shaderFlags |= ShaderStageFlags.FragmentBit;

            vkBindings[i] = new DescriptorSetLayoutBinding
            {
                Binding = binding.Binding,
                DescriptorCount = 1,
                DescriptorType = binding.Type.ToVk(),
                StageFlags = shaderFlags
            };
        }

        DescriptorSetLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.DescriptorSetLayoutCreateInfo,
            BindingCount = (uint) bindings.Length,
            PBindings = vkBindings
        };
        
        GraphiteLog.Log("Creating descriptor set layout.");
        _vk.CreateDescriptorSetLayout(_device, &layoutInfo, null, out Layout).Check("Create descriptor set layout");
    }


    public override void Dispose()
    {
        GraphiteLog.Log("Destroying descriptor layout.");
        _vk.DestroyDescriptorSetLayout(_device, Layout, null);
    }
}