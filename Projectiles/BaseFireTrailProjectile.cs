using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public abstract class BaseFireTrailProjectile : ModProjectile
    {
        protected virtual float FireBodyWidth => 24f;
        protected virtual int FireCoreWidth => 16;

        protected void SpawnFireHitDust(Color startColor, bool noLightEmittence)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(3f, 5f);
                Color dustColor = Color.Lerp(startColor, Color.Gold, Main.rand.NextFloat(0.5f, 1f));

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = noLightEmittence;
            }
        }

        protected void DrawPixelatedFireTrail(Graphics.Primitives.PrimitiveSettings.VertexColorFunction bodyColor, Graphics.Primitives.PrimitiveSettings.VertexColorFunction coreColor)
        {
            Terraria.Graphics.Shaders.GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            Graphics.Primitives.PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(FireWidthFunction, bodyColor, (_, _) => Projectile.Size * 0.5f, true, true, Terraria.Graphics.Shaders.GameShaders.Misc["CalamityMod:ImpFlameTrail"]), Projectile.oldPos.Length + 32);
            Vector2[] fireCoreLength = Projectile.oldPos.Take(8).ToArray();
            Terraria.Graphics.Shaders.GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            Graphics.Primitives.PrimitiveRenderer.RenderTrail(fireCoreLength, new(FireCoreWidthFunction, coreColor, (_, _) => Projectile.Size * 0.5f, true, true, Terraria.Graphics.Shaders.GameShaders.Misc["CalamityMod:ImpFlameTrail"]), fireCoreLength.Length + 24);
        }

        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = FireBodyWidth * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * FireCoreWidth;
            float curveRatio = 0.25f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }
    }
}
