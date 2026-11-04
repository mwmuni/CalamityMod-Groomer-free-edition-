sampler cloudsTexture : register(s0);
sampler distortionTexture : register(s1);
sampler erosionTexture : register(s2);

float time;
float overallOpacity;
float distortionStrength;
float mainNoiseTextureScale;
float distortionTextureScale;
float erosionTextureScale;
float erosionMin;
float gradientPrecision;

float2 pixelationFactor;
float2 worldOffset;

float3 darkerPixelColor;
float3 brighterPixelColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Pixelation.
  coords = round(coords * pixelationFactor) / pixelationFactor;

  // Stretch the coords out to better mimic a blowing wind effect.
  coords *= float2(0.95, 1.05);

  // Move with the player in the world.
  coords += worldOffset;

  // Calculate the distortion that'll be applied to our base noise coordinates.
  float2 distortedCoords =
      coords + float2(time * -0.03, time * -0.01) * distortionTextureScale;
  float distortion =
      tex2D(distortionTexture, distortedCoords).r * distortionStrength;

  // Calculate the base noise colors.
  float2 adjustedCoords = (coords + distortion) * mainNoiseTextureScale;
  float4 scrollingNoise1 =
      tex2D(cloudsTexture, adjustedCoords + float2(time * -0.065, 0));
  float4 scrollingNoise2 =
      tex2D(cloudsTexture, adjustedCoords + float2(time * -0.015, 0));

  // Multiply the red values of the noise maps together to get the horizontal
  // brightness of both noise maps.
  float combinedBrightness = scrollingNoise1.r * scrollingNoise2.r;

  // Lerp between the two colors based on the brightness of a pixel.
  float3 colorFromBrightness =
      lerp(darkerPixelColor, brighterPixelColor, combinedBrightness);

  // Erode the final output to break up the entire effect slightly.
  float erosionColor = tex2D(
      erosionTexture, adjustedCoords + float2(time * -0.07, time * -0.02) *
                                           erosionTextureScale);
  float erosionMax = erosionMin + 1;
  float erosion = smoothstep(erosionMin, erosionMax, erosionColor.r);

  float4 finalColor =
      (float4(colorFromBrightness, 1) * (scrollingNoise1 + scrollingNoise2));
  finalColor *= erosion;

  // Apply posterization.
  finalColor = round(finalColor * gradientPrecision) / gradientPrecision;

  return finalColor * overallOpacity;
}

technique Technique1 {
  pass DoGFogPass { PixelShader = compile ps_2_0 PixelShaderFunction(); }
}
