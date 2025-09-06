using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Graphite.Core;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.Windows.Windows;
using GrBackend = Graphite.Backend;
using SpvBackend = Silk.NET.SPIRV.Cross.Backend;
using SpvCompiler = Silk.NET.SPIRV.Cross.Compiler;

namespace Graphite.ShaderTools;

/// <summary>
/// Contains various methods useful for compiling and transpiling shaders into code for <see cref="Backend"/>s to consume.
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static unsafe class Compiler
{
    private static readonly Cross _spirv;

    static Compiler()
    {
        _spirv = Cross.GetApi();
        ResolveLibrary += OnResolveLibrary;
    }

    private static IntPtr OnResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        string newLibName = libraryName switch
        {
            "d3dcompiler47" => "libvkd3d-utils",
            _ => libraryName
        };

        return NativeLibrary.Load(newLibName, assembly, searchPath);
    }

    /// <summary>
    /// Compile HLSL (SM6.0) into bytecode that the given <paramref name="backend"/> can use in the creation of a <see cref="ShaderModule"/>.
    /// </summary>
    /// <param name="backend">The <see cref="Backend"/> to compile for.</param>
    /// <param name="stage">The <see cref="ShaderStage"/> to compile for.</param>
    /// <param name="hlsl">The HLSL (Shader Model 6.0) code.</param>
    /// <param name="entryPoint">The entry point of the shader.</param>
    /// <param name="mapping">Returns the <see cref="ShaderMappingInfo"/> which defines how the shader should be remapped for the given <paramref name="backend"/>.</param>
    /// <param name="includeDir">The include directory of the code, if any.</param>
    /// <param name="debug">If true, the shader will be compiled with debugging enabled. (-Od parameter)</param>
    /// <returns>The shader bytecode.</returns>
    /// <remarks>This does <b>NOT</b> only compile to SPIR-V. It will compile to the correct shader format of the given <paramref name="backend"/>.</remarks>
    public static byte[] CompileHLSL(GrBackend backend, ShaderStage stage, string hlsl, string entryPoint,
        out ShaderMappingInfo mapping, string? includeDir = null, bool debug = false)
    {
        // Use DXC to compile SM6 HLSL into bytecode. Typically Spir-V which then gets passed to TranspileSpirv so it
        // can be in the correct format for each backend.
        // D3D12 backend has a special case, as DXIL is the primary target of DXC. So it's pointless to compile to Spir-V,
        // then transpile it back to DXIL again. So for D3D12 we compile straight to DXIL.
        // The reason the D3D11 backend doesn't directly compile to DXBC is because we must support descriptor sets
        // which are only "supported" in SM5.1 and up. We could technically compile SM5.1 to FXC instead as
        // right now Graphite doesn't support any features offered by SM6, however it's just easier to do it this way.
        
        // TODO: CompileHLSLToSpirV: Very simple to do, just call CompileHLSL with Backend.Vulkan.
        // Or alternatively move some functionality from this method into that one and call that method in this one.
        
        Guid dxcUtils = CLSID_DxcUtils;
        Guid dxcCompiler = CLSID_DxcCompiler;

        IDxcUtils* utils;
        CheckResult(DxcCreateInstance(&dxcUtils, __uuidof<IDxcUtils>(), (void**) &utils),
            "Create DXC utils");

        IDxcCompiler3* compiler;
        CheckResult(DxcCreateInstance(&dxcCompiler, __uuidof<IDxcCompiler3>(), (void**) &compiler),
            "Create DXC compiler");

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        List<string> args = [];
        
        if (backend != GrBackend.D3D12)
            args.Add("-spirv");

        if (includeDir != null)
        {
            args.Add("-I");
            args.Add(includeDir);
        }
        
        if (debug)
            args.Add("-Od");

        using Utf8String pHlsl = hlsl;
        using DxcString pEntryPoint = entryPoint;
        using DxcString pProfile = profile;
        using DxcStringArray pArgs = new DxcStringArray(args);

        IDxcCompilerArgs* compilerArgs;
        CheckResult(utils->BuildArguments(null, pEntryPoint, pProfile, pArgs, pArgs.Length, null, 0, &compilerArgs),
            "Build arguments");

        IDxcIncludeHandler* includeHandler;
        CheckResult(utils->CreateDefaultIncludeHandler(&includeHandler), "Create include handler");

        DxcBuffer buffer = new()
        {
            Ptr = (void*) pHlsl.Handle,
            Size = (nuint) hlsl.Length
        };

        IDxcResult* compileResult;
        CheckResult(
            compiler->Compile(&buffer, compilerArgs->GetArguments(), compilerArgs->GetCount(), includeHandler,
                __uuidof<IDxcResult>(), (void**) &compileResult), "Compile");

        HRESULT compileStatus;
        CheckResult(compileResult->GetStatus(&compileStatus), "Get compile status");

        if (compileStatus.FAILED)
        {
            IDxcBlobEncoding* errorBlob;
            CheckResult(compileResult->GetErrorBuffer(&errorBlob), "Get error buffer");
            // TODO: CompilationException
            throw new Exception(
                $"Failed to compile {stage} shader: {new string((sbyte*) errorBlob->GetBufferPointer())}");
        }

        IDxcBlob* bResult;
        CheckResult(compileResult->GetResult(&bResult), "Get result");

        byte[] result = new byte[bResult->GetBufferSize()];
        fixed (byte* pResult = result)
            Unsafe.CopyBlock(pResult, bResult->GetBufferPointer(), (uint) result.Length);
        
        if (backend == GrBackend.D3D12)
            throw new NotImplementedException();

        return TranspileSpirv(backend, result, stage, entryPoint, out mapping);
    }

    /// <summary>
    /// Transpile SPIR-V bytecode into the correct shader format for the given <paramref name="backend"/>.
    /// </summary>
    /// <param name="backend">The <see cref="Backend"/> to compile for.</param>
    /// <param name="spirv">The SPIR-V bytecode.</param>
    /// <param name="stage">The <see cref="ShaderStage"/> to compile for.</param>
    /// <param name="entryPoint">The entry point of the shader.</param>
    /// <param name="mapping">Returns the <see cref="ShaderMappingInfo"/> which defines how the shader should be remapped for the given <paramref name="backend"/>.</param>
    /// <returns>The shader bytecode.</returns>
    public static byte[] TranspileSpirv(GrBackend backend, byte[] spirv, ShaderStage stage, string entryPoint,
        out ShaderMappingInfo mapping)
    {
        if (backend == GrBackend.Other)
            throw new NotSupportedException("Custom backends must use pre-compiled shaders, or a custom transpiler!");
        
        // Can't transpile spirv to spirv! Or can we...
        //⠀⠀⠀⠀⠀⠀⣠⢀⣠⣤⣄⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⠀⠀⠀⠀⣠⣾⣷⣿⣿⣿⣿⣿⣿⣾⣿⣿⣿⣿⣶⣽⣿⣿⣦⣤⣲⣤⣤⡠⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⠀⠀⣰⣾⣻⣿⠿⠟⠉⠙⠁⠈⠉⠙⠛⠻⠿⢿⢿⣿⣿⣿⣿⣿⣿⣿⣽⣽⡤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⠀⠀⣼⠟⠁⠀⠀⠀⡀⠀⠀⠀⢑⣒⣶⣀⣀⣤⣀⡨⠈⠉⠉⠓⠉⠛⠛⢻⣝⠪⠀⠀⠀⠀⠀⠀⠐⢆⣆⣤⢶⣒⡒⣲⢲⣤⣠⣤⣀⣠⣴⣤⡴⢶⣤⣶⣖⣢⣴⣤⣤⣠⣀⣀⠀⠀⠀⠀⠀⠀
        // ⣀⣀⣁⣀⣠⣤⣤⣤⣶⣶⣾⣿⣭⣿⣷⣿⣾⣿⣭⣿⣿⣾⣦⣤⣤⣦⣤⣤⣀⣀⡀⠀⠀⠀⠀⠀⠀⢀⣁⣘⣻⣬⣿⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣿⣿⣿⣿⣻⣿⣿⣿⣿⣿⣿⣯⣴⣄⣀⣀
        // ⣿⣿⠟⠋⠉⠉⠉⠙⠛⢻⣾⠛⠃⢠⡴⣶⣾⣓⢦⣌⠛⠳⣿⣛⣛⠛⠛⠿⢿⢿⣿⣷⣶⣶⣶⣶⣾⣿⣿⣿⣿⠟⠛⠋⠙⠻⢛⡿⠋⠉⡿⣿⣿⣿⣿⣍⠉⠙⠿⣿⣿⣄⡀⠀⠈⠛⠛⠙⣿⣿
        // ⣿⢸⠀⠀⠀⠀⠀⠀⠀⠈⢬⣀⢀⣙⣷⠭⣿⠯⠶⠛⠀⣀⣐⠭⠛⢫⣀⠀⡂⢸⣿⣿⠟⠉⠉⠉⠛⢿⣿⣿⠇⠀⠀⠀⠀⠈⠉⠀⠀⠘⠷⠽⠿⠯⡽⣿⣄⠂⠀⠈⠙⠈⠉⠁⠀⠀⠀⠀⢸⣿
        // ⣿⡸⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⢠⣿⡿⠃⠀⠀⠀⠀⠀⠀⠙⢿⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣼⣿
        // ⢿⣷⡳⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣶⡿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣠⢶⣯⣿⠏
        // ⠝⠉⠛⠳⠿⠿⠿⢷⣶⣶⣤⣤⣤⣤⣴⣶⣦⣤⣶⣶⡶⠶⠶⢶⡶⣺⡿⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠿⣶⣶⣶⣶⠶⠶⣶⣶⣶⣶⣶⣶⣶⣶⡶⠶⠶⠶⠾⠿⠓⠛⠋⠉⠁⠀
        // ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠤⠾⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠭⠭⠥⠅⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠄⡀⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⣇⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⢦⡛⣻⠿⡶⢌⡂⠀⠀⠀⠀⢰⠖⠛⠛⠛⣫⣉⠠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        // ⣿⣄⣷⢄⢴⡗⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⢀⣠⣤⣓⢀⣬⣴⣾⣧⣼⡽⣦⡔⣀⣀⣼⣶⣿⣿⣿⣿⣄⡞⣦⣄⢲⠲⣳⡢⣄⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣄⡲⡔⢦⣦⡀
        // ⣿⣿⣯⣏⣾⣼⣃⡔⡀⠐⠀⠀⠀⠀⠀⠀⠀⣴⣦⣾⣿⣫⣟⣽⣷⣿⣿⣿⣿⣿⣷⣿⣾⣿⣿⣿⣿⣿⣿⣿⣿⣯⣿⣿⣿⣿⣿⣿⣶⣯⣿⣿⣿⣜⣏⠳⣦⡀⠀⠀⠀⠀⣐⢷⣾⣶⣳⣺⡈⢀
        // ⣿⣿⣟⣿⣿⣾⣫⣼⣾⣈⡤⢢⢤⠀⣀⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡟⣻⢿⠿⡿⣿⣿⡿⠿⠛⠿⠿⠿⠿⠟⠋⡻⣿⣿⢛⣿⣟⣿⣿⣿⣿⣿⣿⣿⣾⣿⣷⢄⡆⢦⢰⣧⣾⣾⣿⣯⣯⣷⣼
        // ⣿⣿⣿⣿⣿⣿⣿⣿⢟⣽⡷⣼⡻⣱⡟⢿⡽⣟⣿⣿⣿⣿⣿⣷⣿⠷⠿⠟⠛⠓⠒⠒⡛⠛⠛⠒⠒⠛⠛⠉⠉⠉⢉⠉⠉⠉⠉⠉⠉⠉⠛⠻⠿⣿⢿⣿⣿⣿⡇⣽⢶⣷⣿⣿⣿⣿⣟⣿⣏⣿
        // ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣄⠛⡟⣞⣻⣟⣿⠛⠋⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⢶⡒⣖⢆⠀⡈⣾⣰⡦⣤⣥⣠⢤⣄⠀⠀⠀⠀⠀⠀⢰⣼⣿⣿⣿⣟⣰⣿⣗⣿⣿⣿⣿⣿⣿⣿⣿⣿
        // ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣾⣿⡾⣿⣿⣿⣿⣷⣄⠀⣀⡀⠀⠀⠀⠀⠀⠀⡛⢛⣱⣯⣿⣾⡿⢿⣿⠿⠿⣿⣽⣿⣿⣾⣾⣷⣴⣬⣶⣶⣿⣿⣿⣿⣿⣿⣿⣿⠿⢿⣯⣿⣿⣿⣿⣿⣿⣿⣿
        // (credit https://emojicombos.com/vsauce) i don't know why it's so big but i'm keeping it now
        if (backend == GrBackend.Vulkan)
        {
            mapping = default;
            return spirv;
        }

        SpvBackend spvBackend = backend switch
        {
            GrBackend.D3D12 => SpvBackend.Hlsl,
            GrBackend.D3D11 => SpvBackend.Hlsl,
            GrBackend.OpenGL => SpvBackend.Glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(backend), backend, null)
        };

        Context* context;
        CheckResult(_spirv.ContextCreate(&context), "Create context");
        
        try
        {
            ParsedIr* ir;
            fixed (byte* pSpv = spirv)
            {
                Result result = _spirv.ContextParseSpirv(context, (uint*) pSpv, (nuint) spirv.Length / 4, &ir);
                if (result == Result.ErrorInvalidSpirv)
                    // TODO: CompilationException
                    throw new Exception($"Failed to parse Spir-V: {_spirv.ContextGetLastErrorStringS(context)}");
            }

            SpvCompiler* compiler;
            CheckResult(_spirv.ContextCreateCompiler(context, spvBackend, ir, CaptureMode.TakeOwnership, &compiler),
                "Create compiler");

            CompilerOptions* options;
            CheckResult(_spirv.CompilerCreateCompilerOptions(compiler, &options), "Create compiler options");

            switch (spvBackend)
            {
                case SpvBackend.Hlsl:
                {
                    CheckResult(_spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50), "Set shader model");
                    break;
                }
                case SpvBackend.Glsl:
                {
                    CheckResult(_spirv.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 430), "Set GLSL version");
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            CheckResult(_spirv.CompilerInstallCompilerOptions(compiler, options), "Install compiler options");

            ExecutionModel model = stage switch
            {
                ShaderStage.Vertex => ExecutionModel.Vertex,
                ShaderStage.Pixel => ExecutionModel.Fragment,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            };
            
            _spirv.CompilerSetEntryPoint(compiler, entryPoint, model);

            byte* compiled;
            CheckResult(_spirv.CompilerCompile(compiler, &compiled), "Compile");

            GraphiteLog.Log(new string((sbyte*) compiled));
            
            CheckResult(_spirv.CompilerBuildCombinedImageSamplers(compiler), "Build combined image samplers");
            uint samplerId;
            CheckResult(_spirv.CompilerBuildDummySamplerForCombinedImages(compiler, &samplerId), "Build dummy sampler");
            
            CombinedImageSampler* combinedSamplers;
            nuint numSamplers;
            CheckResult(_spirv.CompilerGetCombinedImageSamplers(compiler, &combinedSamplers, &numSamplers), "Get combined image samplers");

            for (uint i = 0; i < numSamplers; i++)
            {
                uint id = combinedSamplers[i].ImageId;
                uint newId = combinedSamplers[i].CombinedId;
                
                uint set = _spirv.CompilerGetDecoration(compiler, id, Decoration.DescriptorSet);
                uint binding = _spirv.CompilerGetDecoration(compiler, id, Decoration.Binding);
                _spirv.CompilerSetDecoration(compiler, newId, Decoration.DescriptorSet, set);
                _spirv.CompilerSetDecoration(compiler, newId, Decoration.Binding, binding);
            }
            
            mapping = new ShaderMappingInfo();
            Resources* resources;
            CheckResult(_spirv.CompilerCreateShaderResources(compiler, &resources), "Create shader resources");
            
            List<DescriptorMapping> descriptorMappings = [];
            
            RemapDescriptorsForType(compiler, resources, ResourceType.UniformBuffer, descriptorMappings);
            RemapDescriptorsForType(compiler, resources, ResourceType.SampledImage, descriptorMappings);

            mapping.Descriptors = descriptorMappings.ToArray();
            
            if (backend == GrBackend.D3D11)
            {
                nuint numResources;
                ReflectedResource* reflectedResources;
                _spirv.ResourcesGetResourceListForType(resources, ResourceType.StageInput, &reflectedResources, &numResources);

                VertexInputMapping[] vertexInput = new VertexInputMapping[numResources];
                for (uint i = 0; i < numResources; i++)
                    vertexInput[i] = new VertexInputMapping(Semantic.TexCoord, i);

                mapping.VertexInput = vertexInput;
                return CompileDXBC(compiled, "main", stage);
            }

            if (backend == GrBackend.OpenGL)
            {
                nuint length = strlen(compiled);
                byte[] glslBytes = new byte[length];
                fixed (byte* pGlsl = glslBytes)
                    Unsafe.CopyBlock(pGlsl, compiled, (uint) length);

                return glslBytes;
            }
        }
        finally
        {
            _spirv.ContextDestroy(context);
        }

        throw new NotImplementedException();
    }
    
    // Compile SM5 HLSL to DXBC.
    private static byte[] CompileDXBC(byte* hlsl, string entryPoint, ShaderStage stage)
    {
        nuint hlslLength = strlen(hlsl);
        using Utf8String pEntryPoint = entryPoint;

        string target = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        using Utf8String pTarget = target;

        ID3DBlob* blob;
        ID3DBlob* errorBlob;
        if (D3DCompile(hlsl, hlslLength, null, null, null, pEntryPoint, pTarget, 0, 0, &blob, &errorBlob)
            .FAILED)
        {
            throw new Exception($"Failed to compile shader: {new string((sbyte*) errorBlob->GetBufferPointer())}");
        }

        if (errorBlob != null)
            errorBlob->Release();

        byte[] compiled = new byte[blob->GetBufferSize()];
        fixed (byte* pCompiled = compiled)
            Unsafe.CopyBlock(pCompiled, blob->GetBufferPointer(), (uint) compiled.Length);

        blob->Release();
        return compiled;
    }

    private static void RemapDescriptorsForType(SpvCompiler* compiler, Resources* resources, ResourceType type, List<DescriptorMapping> mappings)
    {
        ReflectedResource* resource;
        nuint numResources;
        _spirv.ResourcesGetResourceListForType(resources, type, &resource, &numResources);

        for (uint i = 0; i < numResources; i++)
        {
            uint id = resource[i].Id;
            
            uint set = _spirv.CompilerGetDecoration(compiler, id, Decoration.DescriptorSet);
            uint binding = _spirv.CompilerGetDecoration(compiler, id, Decoration.Binding);

            uint slot = i;
            _spirv.CompilerSetDecoration(compiler, id, Decoration.DescriptorSet, 0);
            _spirv.CompilerSetDecoration(compiler, id, Decoration.Binding, slot);
            
            mappings.Add(new DescriptorMapping(set, binding, slot));
        }
    }

    private static void CheckResult(Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Spirv-Cross operation '{operation}' failed: {result}");
    }

    private static void CheckResult(HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new Exception($"DXC operation '{operation}' failed with HRESULT: 0x{result.Value:x8}");
    }

    private static nuint strlen(byte* str)
    {
        byte b;
        nuint len = 0;

        do
        {
            b = str[len++];
        } while (b != 0);

        return len;
    }
}