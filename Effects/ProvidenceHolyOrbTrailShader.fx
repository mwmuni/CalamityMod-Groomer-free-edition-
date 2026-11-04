// 29JUN2026: fryzahh
// Programmed to work under the use of SanePrimitiveRenderer.
// Never use this shader with PrimitveRenderer.

sampler baseStreakTexture : register(s0);
sampler distortionTexture : register(s1);

matrix uTransformMatrix;
float time;
float glowPower;
float pixelationFactor;

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
  VertexShaderOutput output;
  output.Position = mul(input.Position, uTransformMatrix);
  output.Color = input.Color;
  output.TextureCoordinates = input.TextureCoordinates;
  return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0 {
  float4 color = input.Color;
  float2 coords = input.TextureCoordinates;

  // Fade out at the horizontal and vertical ends of the streak.
  float horizonatalDistanceFromCenter = distance(0.5, coords.y);
  float opacity = smoothstep(0.5, 0.5 - 0.486, horizonatalDistanceFromCenter);

  // Glow from the center go out.
  float glow = pow(0.1 / horizonatalDistanceFromCenter, glowPower);

  float2 distortionCoords = coords + float2(time * -3.12, 0);
  distortionCoords *= float2(0.25, 1);
  float distortion = tex2D(distortionTexture, distortionCoords).r * 0.43;

  float2 streakCoords = coords + float2(time * -2.34, 0);
  streakCoords *= float2(1.25, 1);
  float4 streakMapColor = tex2D(baseStreakTexture, streakCoords + distortion);

  float4 returnColor = (streakMapColor * color * glow) * opacity;
  returnColor = round(returnColor * 7) / 7;
  return returnColor;
}

technique Technique1 {
  pass TrailPass {
    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader = compile ps_2_0 PixelShaderFunction();
  }
}