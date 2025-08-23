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

cbuffer TransformMatrix : register(b0)
{
    float4x4 Transform;
}

Texture2D Texture    : register(t1);
SamplerState Sampler : register(s1);

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
    return Texture.Sample(Sampler, input.TexCoord);
    //return float4(input.TexCoord, 0.0, 1.0);
}