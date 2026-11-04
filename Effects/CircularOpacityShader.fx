sampler baseTexture : register(s0);

float opacityCutoffValue;
float fadeoutPower;
float overallOpacity;

float4 PixelShaderFunction(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Convert the regular coordinates to polar coordinates to get a circle shape.
  float2 centeredCoords = coords - 0.5;
  float polarAngle = atan2(centeredCoords.y, centeredCoords.x) / 6.283;
  float distanceToCenter = length(centeredCoords);

  float2 polarCoords = float2(polarAngle, distanceToCenter);
  float4 color = tex2D(baseTexture, coords);

  // Fade out from the edges of the texture to the middle depending on the
  // cutoff value. "fadeoutPower" values which vary from 1 will affect how
  // strong the edges fade while also increasing the brightness of the non-faded
  // parts of the image to the middle. Use this wisely.
  if (-polarCoords.y < opacityCutoffValue)
    color.rgba *= pow(-polarCoords.y / (polarCoords.y - opacityCutoffValue),
                      -fadeoutPower);

  return color * sampleColor * overallOpacity;
}

technique Technique1 {
  pass CircularOpacityPass {
    PixelShader = compile ps_2_0 PixelShaderFunction();
  }
}
