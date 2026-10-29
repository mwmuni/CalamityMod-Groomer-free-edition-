using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class CursedFire : BaseTwinsFireProjectile, ILocalizedModType
    {
        protected override Vector3 LightColor => new Vector3(0.15f, 0.75f, 0.15f);
        protected override Color FireColor1 => new Color(100, 255, 20, 200);
        protected override Color FireColor2 => new Color(100, 128, 10, 70);
        protected override Color FireColor3 => new Color(100, 240, 20, 100);
        protected override Color FireColor4 => new Color(80, 80, 0, 100);

        protected override void ApplyDebuff(Player target) => target.AddBuff(BuffID.CursedInferno, 120);
    }
}
