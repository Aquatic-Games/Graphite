global using VkSampler = Silk.NET.Vulkan.Sampler;
using Graphite.Core;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanSampler : Sampler
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    public readonly VkSampler Sampler;

    public VulkanSampler(Vk vk, VkDevice device, ref readonly SamplerInfo info)
    {
        _vk = vk;
        _device = device;

        SamplerCreateInfo samplerInfo = new()
        {
            SType = StructureType.SamplerCreateInfo,
            MinFilter = info.MinFilter.ToVkFilter(),
            MagFilter = info.MagFilter.ToVkFilter(),
            MipmapMode = info.MipFilter.ToVkMipmapMode(),
            AddressModeU = info.AddressU.ToVk(),
            AddressModeV = info.AddressV.ToVk(),
            AddressModeW = info.AddressW.ToVk(),
            AnisotropyEnable = info.MaxAnisotropy > 0,
            MaxAnisotropy = info.MaxAnisotropy,
            MinLod = info.MinLod,
            MaxLod = info.MaxLod,
            BorderColor = BorderColor.FloatTransparentBlack
        };
        
        GraphiteLog.Log("Creating sampler.");
        _vk.CreateSampler(_device, &samplerInfo, null, out Sampler).Check("Create sampler");
    }
    
    public override void Dispose()
    {
        GraphiteLog.Log("Destroying sampler.");
        _vk.DestroySampler(_device, Sampler, null);
    }
}