float4 VSMain(const in uint id: SV_VertexID): SV_Position
{
    const float2 positions[] = {
        float2(-0.5, +0.5),
        float2(+0.5, +0.5),
        float2(+0.5, -0.5),
        float2(-0.5, -0.5)
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    return float4(positions[indices[id]], 0.0, 1.0);
}

float4 PSMain(): SV_Target0
{
    return float4(1.0, 0.5, 0.25, 1.0);
}