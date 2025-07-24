using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;
    
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
    
    public override void Dispose()
    {
        GraphiteLog.Log("Freeing command buffer.");
        _vk.FreeCommandBuffers(_device, _pool, 1, in Buffer);
    }
}