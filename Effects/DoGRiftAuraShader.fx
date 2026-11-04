sampler baseTexture : register(s0);
sampler distortionTexture : register(s1);

float time;
float overallOpacity;
float distortionStrength;
float opacityCutoffValue;
float fadeoutPower;
float minBrightnessValue;
float gradientPrecision;
float2 pixelSize;
float3 brighterPixelColor;

float InverseLerp(float from, float to, float value) {
  return (value - from) / (to - from);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Pixelate.
  coords = round(coords * pixelSize) / pixelSize;

  // Convert the regular coordinates to polar coordinates to get a circle shape.
  float2 centeredCoords = coords - 0.5;
  float polarAngle = atan2(centeredCoords.y, centeredCoords.x) / 6.283;
  float distanceToCenter = length(centeredCoords);
  float2 polarCoords = float2(polarAngle, distanceToCenter);

  // Apply distortion to the main texture.
  float distortion = tex2D(distortionTexture,
                           polarCoords + float2(time * 0.017, time * -0.026))
                         .r *
                     distortionStrength;
  float2 baseCoords = coords * distortion;
  baseCoords *= 0.4;
  float4 color = tex2D(baseTexture, baseCoords);

  // Get the horizontal brightness of the sampled texture and apply coloration.
  float brightness = (color.r + color.g + color.b) / 3;
  float brightnessRatio = InverseLerp(minBrightnessValue, 1, brightness);
  float3 colorFromBrightness =
      lerp(float3(0, 0, 0), brighterPixelColor, brightnessRatio);

  float4 coloredSample = color * float4(colorFromBrightness, 1);

  // Fade out from the edges of the texture to the middle depending on the
  // cutoff value. "fadeoutPower" values which vary from 1 will affect how
  // strong the edges fade while also increasing/decreasing the brightness of
  // the non-faded parts of the image to the middle. Use this wisely.
  if (-polarCoords.y < opacityCutoffValue)
    coloredSample.rgba *= pow(
        -polarCoords.y / (polarCoords.y - opacityCutoffValue), -fadeoutPower);

  // Apply optional posterization.
  if (gradientPrecision > 0)
    coloredSample =
        round(coloredSample * gradientPrecision) / gradientPrecision;

  return coloredSample * sampleColor * overallOpacity;
}

technique Technique1 {
  pass DoGRiftAuraPass { PixelShader = compile ps_3_0 PixelShaderFunction(); }
}
