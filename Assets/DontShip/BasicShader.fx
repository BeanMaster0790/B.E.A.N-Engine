// BasicEffect.fx
matrix World;
matrix View;
matrix Projection;

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float4 Color    : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(float4(input.Position, 1.0), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Color = input.Color;
    return output;
}

float4 PS(VertexShaderOutput input) : SV_Target
{
    return input.Color;
}

technique BasicTech
{
    pass P0
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader  = compile ps_4_0 PS();
    }
}
