sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
matrix uWorldViewProjection;
float4 uShaderSpecificData;

struct VertexShaderInput {
  float4 Position : POSITION0;
  float4 Color : COLOR0;
  float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput {
  float4 Position : SV_POSITION;
  float4 Color : COLOR0;
  float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input) {
  VertexShaderOutput output = (VertexShaderOutput)0;
  float4 pos = mul(input.Position, uWorldViewProjection);
  output.Position = pos;

  output.Color = input.Color;
  output.TextureCoordinates = input.TextureCoordinates;

  return output;
}

// The X coordinate is the trail completion, the Y coordinate is the same as any
// other. This is simply how the primitive TextCoord is layed out in the C#
// code. Inputted images go into uImage1 sampler, in case you have a noise map
// or something similar.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0 {
  float4 color = input.Color;
  float2 coords = input.TextureCoordinates;
  float2 pixellatedCoords = round(coords * 20) / 20;
  float4 noiseColor = tex2D(
      uImage1, float2(pixellatedCoords.x - uTime * 1.3, pixellatedCoords.y) *
                   float2(0.5, 1));
  float4 noiseColor2 =
      tex2D(uImage1, float2(pixellatedCoords.x - uTime * 1.1,
                            pixellatedCoords.y + uTime * 0.44) *
                         float2(0.5, 1));
  float4 baseColor = lerp(noiseColor, noiseColor2,
                          sin(uTime * 5.1 + coords.x * 3.4) * 0.5 + 0.5);
  baseColor = pow(baseColor, 2);
  return lerp(color * 0.4, 1, baseColor.r) * uOpacity;
}

technique Technique1 {
  pass TrailPass {
    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader = compile ps_2_0 PixelShaderFunction();
  }
}
