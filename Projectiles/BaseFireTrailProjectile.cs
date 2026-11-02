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
            float maxBodyWidth = Projectile.scale * 16;
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
