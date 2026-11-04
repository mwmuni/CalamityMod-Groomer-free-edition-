sampler mainWindTexture : register(s0);
sampler windHighlightsTexture : register(s1);
sampler distortionTexture : register(s2);
sampler erosionTexture : register(s3);

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
float3 highlightsColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Pixelation.
  coords = round(coords * pixelationFactor) / pixelationFactor;

  // Stretch the coords out to better mimic a blowing wind effect.
  coords *= float2(0.25, 1.25);

  // Distort the coords in a sine wave so that they bend up and down.
  coords.y += sin(coords.x * 10 + time) * 0.25;

  // Move with the player in the world.
  coords += worldOffset;

  // Calculate the distortion that'll be applied to our base noise coordinates.
  float2 distortedCoords =
      coords + float2(time * -0.18, 0) * distortionTextureScale;
  float distortion =
      tex2D(distortionTexture, distortedCoords).r * distortionStrength;

  // Calculate the base noise colors.
  float2 adjustedCoords = (coords + distortion) * mainNoiseTextureScale;
  float4 scrollingNoise1 =
      tex2D(mainWindTexture, adjustedCoords + float2(time * -0.092, 0));
  float4 scrollingNoise2 =
      tex2D(mainWindTexture, adjustedCoords + float2(time * -0.031, 0));

  // Multiply the red values of the noise maps together to get the horizontal
  // brightness of both noise maps.
  float combinedBrightness = scrollingNoise1.r * scrollingNoise2.r;

  // Lerp between the two colors based on the brightness of a pixel.
  float3 colorFromBrightness =
      lerp(darkerPixelColor, brighterPixelColor, combinedBrightness);

  // Sample another texture to be used as highlights alongside the two noise
  // textures.
  float4 highlightNoise =
      tex2D(windHighlightsTexture, adjustedCoords + float2(time * -0.115, 0));
  float4 windHighlights = highlightNoise * float4(highlightsColor, 1);

  // Erode the final output to break up the entire effect slightly.
  float erosionColor =
      tex2D(erosionTexture,
            adjustedCoords + float2(time * -0.087, 0) * erosionTextureScale);
  float erosionMax = erosionMin + 1;
  float erosion = smoothstep(erosionMin, erosionMax, erosionColor.r);

  float4 finalColor =
      (float4(colorFromBrightness, 1) * (scrollingNoise1 + scrollingNoise2)) +
      (windHighlights * 2.25);
  finalColor *= erosion;

  // Apply posterization.
  finalColor = round(finalColor * gradientPrecision) / gradientPrecision;

  return finalColor * overallOpacity;
}

technique Technique1 {
  pass DistortionWindsPass {
    PixelShader = compile ps_2_0 PixelShaderFunction();
  }
}
