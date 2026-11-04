sampler baseTexture : register(s0);
sampler disintegrationTexture : register(s1);

float disintegrationProgress : register(C0);
float disintegrationScale;
float2 worldPosition;
float2 pixelSize;

// This shader is largely based on code used for ExampleMod's cool death
// animation thing; it is suprisingly useful for this use case.
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0 {
  float4 color = tex2D(baseTexture, coords);
  if (!any(color))
    return color;

  coords += worldPosition;
  coords = round(coords * pixelSize) / pixelSize;

  float disintegration = pow(saturate(1 - disintegrationProgress), 0.7);
  float2 disintegrationCoords = coords * disintegrationScale;
  float4 disintegrationColor =
      tex2D(disintegrationTexture, disintegrationCoords);

  float disappearThreshold = disintegration * 1.05;
  if (disintegrationColor.r > disappearThreshold) {
    color.rgba = 0;
  } else if (disintegrationColor.b > disintegration) {
    color = float4(1, 105.0 / 255, 180.0 / 255, 1);
  }

  return color;
}

technique Technique1 {
  pass DisintegrationPass {
    PixelShader = compile ps_3_0 PixelShaderFunction();
  }
}