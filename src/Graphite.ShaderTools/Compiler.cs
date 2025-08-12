using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Graphite.Core;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using GrBackend = Graphite.Backend;
using SpvBackend = Silk.NET.SPIRV.Cross.Backend;
using SpvCompiler = Silk.NET.SPIRV.Cross.Compiler;

namespace Graphite.ShaderTools;

public static unsafe class Compiler
{
    private static readonly Cross _spirv;

    static Compiler()
    {
        _spirv = Cross.GetApi();
    }

    public static byte[] CompileHLSL(GrBackend backend, ShaderStage stage, string hlsl, string entryPoint,
        out ShaderMappingInfo mapping)
    {
        if (backend == GrBackend.D3D12)
            throw new NotImplementedException();
        
        IDXC
    }

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
            CheckResult(_spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50), "Set shader model");
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

            mapping = new ShaderMappingInfo();
            Resources* resources;
            CheckResult(_spirv.CompilerCreateShaderResources(compiler, &resources), "Create shader resources");
            
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
        }
        finally
        {
            _spirv.ContextDestroy(context);
        }

        throw new NotImplementedException();
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
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
        if (DirectX.D3DCompile(hlsl, hlslLength, null, null, null, pEntryPoint, pTarget, 0, 0, &blob, &errorBlob)
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

    private static void CheckResult(Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Spirv-Cross operation '{operation}' failed: {result}");
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