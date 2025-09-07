using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Graphite.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_COLOR_WRITE_ENABLE;
using static TerraFX.Interop.DirectX.D3D11_CULL_MODE;
using static TerraFX.Interop.DirectX.D3D11_FILL_MODE;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader* VertexShader;
    public readonly ID3D11PixelShader* PixelShader;

    public readonly ID3D11InputLayout* InputLayout;
    
    public readonly ID3D11BlendState* BlendState;
    public readonly ID3D11DepthStencilState* DepthStencilState;
    public readonly ID3D11RasterizerState* RasterizerState;

    public readonly Dictionary<uint, Dictionary<uint, uint>>? VertexDescriptors;
    public readonly Dictionary<uint, Dictionary<uint, uint>>? PixelDescriptors;
    public readonly D3D11DescriptorLayout[] DescriptorLayouts;
    
    public D3D11Pipeline(ID3D11Device1* device, ref readonly GraphicsPipelineInfo info)
    {
        D3D11ShaderModule vertexShader = (D3D11ShaderModule) info.VertexShader;
        VertexDescriptors = GetDescriptorRemappings(vertexShader.Mapping);
        
        D3D11ShaderModule pixelShader = (D3D11ShaderModule) info.PixelShader;
        PixelDescriptors = GetDescriptorRemappings(pixelShader.Mapping);

        DescriptorLayouts = new D3D11DescriptorLayout[info.Descriptors.Length];
        for (int i = 0; i < DescriptorLayouts.Length; i++)
            DescriptorLayouts[i] = (D3D11DescriptorLayout) info.Descriptors[i];
        
        // pressed the wrong key and I laughed so I am not changing it
        fixed (ID3D11VertexShader** pBertexShader = &VertexShader)
        {
            device->CreateVertexShader(vertexShader.Data, vertexShader.DataLength, null, pBertexShader).Check("Create vertex shader");
        }
        
        // Create the 🅱️ixel shader
        fixed (ID3D11PixelShader** pBixelShader = &PixelShader)
        {
            device->CreatePixelShader(pixelShader.Data, pixelShader.DataLength, null, pBixelShader).Check("Create pixel shader");
        }

        if (info.InputLayout.Length > 0)
        {
            int numInputElements = info.InputLayout.Length;
            
            // For D3D support, there must be a shader mapping, and it must have the same number of elements as the
            // input layout.
            Debug.Assert(vertexShader.Mapping.VertexInput != null,
                "The shader mapping for the vertex shader cannot be null.");
            Debug.Assert(vertexShader.Mapping.VertexInput.Length == numInputElements,
                "The shader mapping for the vertex shader must have the same number of Vertex Input mappings as the Input Layout in the pipeline.");
            
            GCHandle* handles = stackalloc GCHandle[numInputElements];
            D3D11_INPUT_ELEMENT_DESC* elements = stackalloc D3D11_INPUT_ELEMENT_DESC[numInputElements];
            
            for (int i = 0; i < numInputElements; i++)
            {
                ref readonly InputElementDescription element = ref info.InputLayout[i];
                ref readonly VertexInputMapping input = ref vertexShader.Mapping.VertexInput[i];

                string semantic = input.Semantic switch
                {
                    Semantic.Position => "POSITION",
                    Semantic.TexCoord => "TEXCOORD",
                    Semantic.Color => "COLOR",
                    Semantic.Normal => "NORMAL",
                    Semantic.Tangent => "TANGENT",
                    Semantic.Bitangent => "BITANGENT",
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                handles[i] = GCHandle.Alloc(Encoding.UTF8.GetBytes(semantic), GCHandleType.Pinned);

                elements[i] = new D3D11_INPUT_ELEMENT_DESC
                {
                    SemanticName = (sbyte*) handles[i].AddrOfPinnedObject(),
                    SemanticIndex = input.Index,
                    Format = element.Format.ToD3D(),
                    AlignedByteOffset = element.Offset,
                    InputSlot = element.Slot
                };
            }
            
            fixed (ID3D11InputLayout** layout = &InputLayout)
            {
                device->CreateInputLayout(elements, (uint) numInputElements, vertexShader.Data, vertexShader.DataLength,
                    layout).Check("Create input layout");
            }
            
            for (int i = 0; i < numInputElements; i++)
                handles[i].Free();
        }

        D3D11_BLEND_DESC blendDesc = new()
        {
            IndependentBlendEnable = true
        };
        
        // D3D11 spec shows that the maximum number of blend attachments (and therefore color targets) is 8.
        Debug.Assert(info.ColorTargets.Length > 0 && info.ColorTargets.Length < 8);

        for (int i = 0; i < info.ColorTargets.Length; i++)
        {
            ref readonly BlendStateDescription state = ref info.ColorTargets[i].BlendState;

            blendDesc.RenderTarget[i] = new D3D11_RENDER_TARGET_BLEND_DESC
            {
                BlendEnable = state.EnableBlending,
                SrcBlend = state.SrcColorFactor.ToD3D(),
                DestBlend = state.DestColorFactor.ToD3D(),
                BlendOp = state.ColorBlendOp.ToD3D(),
                SrcBlendAlpha = state.SrcAlphaFactor.ToD3D(),
                DestBlendAlpha = state.DestAlphaFactor.ToD3D(),
                BlendOpAlpha = state.AlphaBlendOp.ToD3D(),
                RenderTargetWriteMask = (byte) D3D11_COLOR_WRITE_ENABLE_ALL
            };
        }
        
        GraphiteLog.Log("Creating blend state.");
        fixed (ID3D11BlendState** blendState = &BlendState)
            device->CreateBlendState(&blendDesc, blendState).Check("Create blend state");

        D3D11_DEPTH_STENCIL_DESC depthStencilDesc = new()
        {
            DepthEnable = false
        };

        GraphiteLog.Log("Creating depth-stencil state.");
        fixed (ID3D11DepthStencilState** depthStencilState = &DepthStencilState)
            device->CreateDepthStencilState(&depthStencilDesc, depthStencilState).Check("Create depth-stencil state");

        D3D11_RASTERIZER_DESC rasterizerDesc = new()
        {
            CullMode = D3D11_CULL_NONE,
            FillMode = D3D11_FILL_SOLID,
            ScissorEnable = true
        };
        
        GraphiteLog.Log("Creating rasterizer state.");
        fixed (ID3D11RasterizerState** rasterizerState = &RasterizerState)
            device->CreateRasterizerState(&rasterizerDesc, rasterizerState).Check("Create rasterizer state");
    }
    
    public override void Dispose()
    {
        RasterizerState->Release();
        DepthStencilState->Release();
        BlendState->Release();
        
        if (InputLayout != null)
            InputLayout->Release();
        
        PixelShader->Release();
        VertexShader->Release();
    }

    private static Dictionary<uint, Dictionary<uint, uint>>? GetDescriptorRemappings(ShaderMappingInfo shaderMapping)
    {
        if (shaderMapping.Descriptors is not { } descriptors)
            return null;

        Dictionary<uint, Dictionary<uint, uint>> remapping = [];
        foreach (DescriptorMapping mapping in descriptors)
        {
            if (!remapping.TryGetValue(mapping.Set, out Dictionary<uint, uint> remappedSet))
            {
                remappedSet = [];
                remapping.Add(mapping.Set, remappedSet);
            }
            
            remappedSet.Add(mapping.Binding, mapping.Slot);
        }

        return remapping;
    }
}