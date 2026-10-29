using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Summon
{
    public class LostSoulLarge : LostSoulBase
    {
        protected override int ProjectileSize => 46;
        protected override float? ProjectileScale => 0.75f;
        protected override float TrailOffset => 1.25f;

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.Orange, Color.IndianRed, Projectile.identity % 5f / 5f * 0.5f);
            Color color = Color.Lerp(baseColor * 1.5f, baseColor, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.7f) * 0.04f + 0.45f);
            color.A = 0;
            return color * Projectile.Opacity;
        }
    }
}
