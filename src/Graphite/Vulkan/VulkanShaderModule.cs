global using VkShaderModule = Silk.NET.Vulkan.ShaderModule;
using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanShaderModule : ShaderModule
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly VkShaderModule Module;

    public readonly string EntryPoint;
    
    public VulkanShaderModule(Vk vk, VkDevice device, in ReadOnlySpan<byte> data, string entryPoint)
    {
        _vk = vk;
        _device = device;
        EntryPoint = entryPoint;

        fixed (byte* pData = data)
        {
            ShaderModuleCreateInfo shaderModuleInfo = new()
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint) data.Length,
                PCode = (uint*) pData
            };

            GraphiteLog.Log("Creating shader module.");
            _vk.CreateShaderModule(_device, &shaderModuleInfo, null, out Module).Check("Create shader module");
        }
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying shader module.");
        _vk.DestroyShaderModule(_device, Module, null);
    }
}