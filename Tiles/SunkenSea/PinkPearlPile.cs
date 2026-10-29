using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class PinkPearlPile : PearlPileBase
    {
        protected override int PearlDust => DustID.Ice_Pink;
        protected override int FallingProjectile => ModContent.ProjectileType<PinkPearlFalling>();
        protected override Color PearlMapColor => new Color(204, 143, 174);
        protected override Vector3 PearlLightColor => new Vector3(172f / 255f, 112f / 255f, 145f / 255f);
    }
}
