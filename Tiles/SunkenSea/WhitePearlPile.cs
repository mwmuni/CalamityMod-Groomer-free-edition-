using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class WhitePearlPile : PearlPileBase
    {
        protected override int PearlDust => DustID.Slush;
        protected override int FallingProjectile => ModContent.ProjectileType<WhitePearlFalling>();
        protected override Color PearlMapColor => new Color(217, 209, 213);
        protected override Vector3 PearlLightColor => new Vector3(185f / 255f, 164f / 255f, 184f / 255f);
    }
}
