using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanDescriptorLayout : DescriptorLayout
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly Dictionary<VkDescriptorType, uint> DescriptorCounts;
    public readonly DescriptorSetLayout Layout;
    
    public VulkanDescriptorLayout(Vk vk, VkDevice device, ref readonly DescriptorLayoutInfo info)
    {
        _vk = vk;
        _device = device;
        DescriptorCounts = [];

        ref readonly ReadOnlySpan<DescriptorBinding> bindings = ref info.Bindings;

        DescriptorSetLayoutBinding* vkBindings = stackalloc DescriptorSetLayoutBinding[bindings.Length];
        for (int i = 0; i < bindings.Length; i++)
        {
            ref readonly DescriptorBinding binding = ref bindings[i];

            ShaderStageFlags shaderFlags = ShaderStageFlags.None;
            VkDescriptorType type = binding.Type.ToVk();

            if ((binding.Stages & ShaderStage.Vertex) != 0)
                shaderFlags |= ShaderStageFlags.VertexBit;
            if ((binding.Stages & ShaderStage.Pixel) != 0)
                shaderFlags |= ShaderStageFlags.FragmentBit;

            vkBindings[i] = new DescriptorSetLayoutBinding
            {
                Binding = binding.Binding,
                DescriptorCount = 1,
                DescriptorType = type,
                StageFlags = shaderFlags
            };

            // Is there a better way to do this or am I being stupid?
            if (!DescriptorCounts.TryGetValue(type, out uint count))
                count = 0;
            count++;
            DescriptorCounts[type] = count;
        }

        DescriptorSetLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.DescriptorSetLayoutCreateInfo,
            Flags = info.PushDescriptor ? DescriptorSetLayoutCreateFlags.PushDescriptorBitKhr : 0,
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