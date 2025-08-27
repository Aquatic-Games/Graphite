struct VSInput
{
    float2 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

Texture2D Texture0    : register(t0, space0);
SamplerState Sampler0 : register(s0, space0);
Texture2D Texture1    : register(t1, space0);
SamplerState Sampler1 : register(s1, space0);

cbuffer TransformMatrix : register(b0, space1)
{
    float4x4 Transform;
}

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Transform, float4(input.Position, 0.0, 1.0));
    //output.Position = float4(input.Position, 0.0, 1.0);
    output.TexCoord = input.TexCoord;
    
    return output;
}

float4 PSMain(const in VSOutput input): SV_Target0
{
    return lerp(Texture0.Sample(Sampler0, input.TexCoord), Texture1.Sample(Sampler1, input.TexCoord), 0.8);
    //return float4(input.TexCoord, 0.0, 1.0);
}