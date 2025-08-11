namespace Graphite.ShaderTools;

public static class DeviceExtensions
{
    public static ShaderModule CreateShaderModuleFromSpirv(this Device device, ShaderStage stage, byte[] spirv,
        string entryPoint)
    {
        byte[] compiled =
            SpirvTools.TranspileSpirv(device.Backend, spirv, stage, entryPoint, out ShaderMappingInfo mapping);

        return device.CreateShaderModule(compiled, entryPoint, mapping);
    }
}