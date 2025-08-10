struct VSInput
{
    float2 Position: POSITION0;
    float3 Color:    COLOR0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float3 Color:    COLOR0;
};

cbuffer TransformMatrix : register(b0)
{
    float4x4 Transform;
}

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Transform, float4(input.Position, 0.0, 1.0));
    //output.Position = float4(input.Position, 0.0, 1.0);
    output.Color = input.Color;
    
    return output;
}

float4 PSMain(const in VSOutput input): SV_Target0
{
    return float4(input.Color, 1.0);
}