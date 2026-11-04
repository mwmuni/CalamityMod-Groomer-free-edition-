sampler uImage0 : register(s0);
float uOpacity;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  float4 baseColor = tex2D(uImage0, coords);
  return float4(0, 0, 0, (1 - baseColor.x) * uOpacity);
}
technique Technique1 {
  pass ShadowPass { PixelShader = compile ps_2_0 PixelShaderFunction(); }
}