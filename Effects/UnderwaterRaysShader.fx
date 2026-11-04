sampler baseTexture : register(s0);

float time;
float fadeOutMargin;
float overallOpacity;
float pixelationAmount;
float scrollSpeedX;
float scrollSpeedY;
float2 noiseScale;
float3 rayColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Pixelation.
  coords = round(coords * pixelationAmount) / pixelationAmount;

  // Distort the coords into a sine wave.
  coords.y += sin(coords.x + time * 0.25) * 0.05;

  // Get the correct scale for our two noise maps.
  float2 scaledCoords = coords * noiseScale;

  // Calculate the offsets for each noise map.
  float2 noiseOffset1 = float2(time * scrollSpeedX, time * scrollSpeedY);
  float2 noiseOffset2 = float2(time * -scrollSpeedX, time * -scrollSpeedY);

  // Calculate the colors of the two noise maps.
  float4 noiseTexture1 = tex2D(baseTexture, scaledCoords + noiseOffset1);
  float4 noiseTexture2 = tex2D(baseTexture, scaledCoords + noiseOffset2);

  // Add the noise maps together to layer them over each other.
  // Ensure to add them at 50% intensity to avoid making a super bright effect.
  float combinedNoise = (noiseTexture1.r * 0.5) + (noiseTexture2.r * 0.5);

  // Fade out at a certain margin.
  float opacity = (1 - (coords.y * 2 - fadeOutMargin));

  return (float4(rayColor, 1) * combinedNoise * opacity) * overallOpacity;
}

technique Technique1 {
  pass UnderwaterRayPass { PixelShader = compile ps_2_0 PixelShaderFunction(); }
}
