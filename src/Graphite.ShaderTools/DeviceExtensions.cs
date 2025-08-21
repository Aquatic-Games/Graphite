namespace Graphite.ShaderTools;

public static class DeviceExtensions
{
    /// <summary>
    /// Create a <see cref="ShaderModule"/> from HLSL (Shader Model 6.0) code.
    /// </summary>
    /// <param name="device">The <see cref="Device"/> used create the module.</param>
    /// <param name="stage">The <see cref="ShaderStage"/> of the shader.</param>
    /// <param name="hlsl">The HLSL code to compile.</param>
    /// <param name="entryPoint">The entry point for the given <paramref name="stage"/>.</param>
    /// <param name="includeDir">The include directory, if any.</param>
    /// <param name="debug">If true, the shader will be compiled with debugging enabled. (-Od parameter)</param>
    /// <returns>The created <see cref="ShaderModule"/>.</returns>
    public static ShaderModule CreateShaderModuleFromHLSL(this Device device, ShaderStage stage, string hlsl,
        string entryPoint, string? includeDir = null, bool debug = false)
    {
        byte[] compiled = Compiler.CompileHLSL(device.Backend, stage, hlsl, entryPoint, out ShaderMappingInfo mapping,
            includeDir, debug);

        return device.CreateShaderModule(compiled, entryPoint, mapping);
    }
    
    /// <summary>
    /// Create a <see cref="ShaderModule"/> from SPIR-V bytecode.
    /// </summary>
    /// <param name="device">The <see cref="Device"/> used to create the module.</param>
    /// <param name="stage">The <see cref="ShaderStage"/> of the shader.</param>
    /// <param name="spirv">The SPIR-V bytecode.</param>
    /// <param name="entryPoint">The entry point for the given <paramref name="stage"/>.</param>
    /// <returns>The created <see cref="ShaderModule"/>.</returns>
    public static ShaderModule CreateShaderModuleFromSpirv(this Device device, ShaderStage stage, byte[] spirv,
        string entryPoint)
    {
        byte[] compiled =
            Compiler.TranspileSpirv(device.Backend, spirv, stage, entryPoint, out ShaderMappingInfo mapping);

        return device.CreateShaderModule(compiled, entryPoint, mapping);
    }
}