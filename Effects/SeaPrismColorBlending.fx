sampler baseTexture : register(s0);
sampler greenTexture : register(s1);
sampler purpleTexture : register(s2);
sampler glintTexture : register(s3);

float time;
float2 screenOffset;
float2 offscreenOffset;
float diagonalScreenLength;
bool doGlint;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0, float4 screenPos : VPOS)
    : COLOR {
  // Calculate world position
  float2 worldPos = screenPos.xy + screenOffset - offscreenOffset;

  // Sample all three textures
  float4 blueColor = tex2D(baseTexture, coords);
  float4 purpleColor = tex2D(purpleTexture, coords);
  float4 greenColor = tex2D(greenTexture, coords);
  float4 glintColor = tex2D(glintTexture, coords);

  // Calculate fade values
  float fade1 = (sin(time * 0.2) + 1.0) / 2.0;
  float fade2 =
      (sin(time * 0.1 + worldPos.x * 0.005 - worldPos.y * 0.003) + 1.0) / 2.0;

  // Blend the colors
  float4 result = blueColor;
  result.rgb = lerp(result.rgb, greenColor.rgb, greenColor.a * fade1);
  result.rgb = lerp(result.rgb, purpleColor.rgb, purpleColor.a * fade2);

  if (doGlint) {
    // Calculate glint effect
    float2 projPos = screenPos.xy - offscreenOffset;
    float projection = (screenPos.x / 2.0) - (screenPos.y / 2.0);

    // Define glint beam centers
    float beamCenter1 = diagonalScreenLength * 0.05;
    float beamCenter2 = diagonalScreenLength * 0.5;
    float beamCenter3 = diagonalScreenLength * 1.05;

    // Calculate glint strength for each beam
    float dist1 = abs(projection - beamCenter1);
    float dist2 = abs(projection - beamCenter2);
    float dist3 = abs(projection - beamCenter3);

    float strength1 = saturate(1.0 - dist1 / 100.0);
    float strength2 = saturate(1.0 - dist2 / 100.0);
    float strength3 = saturate(1.0 - dist3 / 100.0);

    float totalGlintStrength = max(strength1, max(strength2, strength3));

    // Apply glint
    result.rgb =
        lerp(result.rgb, glintColor.rgb, glintColor.a * totalGlintStrength);

    // Additive Blend Version (VERY BRIGHT)
    // result.rgb = lerp(result.rgb, result.rgb + glintColor.rgb, glintColor.a *
    // totalGlintStrength);
  }
  return result * sampleColor;
}

technique Technique1 {
  pass SeaPrismBlendingPass {
    PixelShader = compile ps_3_0 PixelShaderFunction();
  }
}