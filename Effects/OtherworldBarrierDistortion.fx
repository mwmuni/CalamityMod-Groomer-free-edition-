sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;    // Opacity works as the intensity of the filter
float uSaturation; // Saturation works as the speed of the noise movement
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;

float4 AdjustPixelPosition(float4 sampleColor : COLOR0,
                           float2 coords : TEXCOORD0)
    : COLOR0 {
  // Drift to offset the noisemap. X multiplier is a prime number to ensure the
  // "loop time" of this is very high, it takes 100 uTime to return to the
  // original state
  float2 drift = float2(0.23 * uTime * uSaturation, uTime * uSaturation);

  // This is the uv distance from the center of the texture
  float2 uvOffsetFromCenter = (coords - 0.5);

  float angle =
      atan2(uvOffsetFromCenter.y, uvOffsetFromCenter.x) / (2 * 3.1415926);
  float dist = length(uvOffsetFromCenter);
  float2 polarUV = float2(angle, dist);

  // Gets the offset from the red channel of whatever pixel this should get from
  // the noisemap. This is fine because the noisemap is greyscale.
  float offsetToHave = tex2D(uImage1, polarUV + drift).x;

  // This mask is to prevent weird stretching effects at the edge of the texture
  float2 modifiedCoords = coords + uvOffsetFromCenter * offsetToHave * uOpacity;
  float mask = step(0, modifiedCoords.x) * step(modifiedCoords.y, 1) *
               step(0, modifiedCoords.y) * step(modifiedCoords.x, 1);

  // This makes a greyscale image designed for additve use transparent for
  // non-additve use. I would like to be able to draw directly additive, but I
  // have been unable to make that work and this has worked well.
  float4 distortedColor = tex2D(uImage0, modifiedCoords);
  distortedColor.a = distortedColor.x;

  return distortedColor * sampleColor * mask;
}

technique Technique1 {
  pass DistortionPass { PixelShader = compile ps_2_0 AdjustPixelPosition(); }
}