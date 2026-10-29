using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Summon
{
    public class LostSoulGold : LostSoulBase
    {
        protected override int ProjectileSize => 38;

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.DarkGoldenrod, Color.Gold, Projectile.identity % 5f / 5f);
            Color color = Color.Lerp(baseColor, Color.White, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.7f) * 0.04f + 0.5f);
            color.A = 127;
            return color * Projectile.Opacity;
        }
    }
}
