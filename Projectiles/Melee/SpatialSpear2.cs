using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear2 : BaseSpatialSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private const int TimeLeft = 100;
        private int TotalSplits = Main.zenithWorld ? 4 : 3;
        protected override int Lifetime => TimeLeft;
        protected override int? Penetration => 3;
        protected override Vector3 LightColor => new Vector3(0.05f, 0.05f, 1f);
        protected override Color AlphaColor => new Color(128, 128, byte.MaxValue);
        protected override int DustType => DustID.BlueFairy;
        protected override int SplitCount => TotalSplits;
        protected override int SplitAngleDegrees => 10;
        protected override int NextProjectileType => ModContent.ProjectileType<SpatialSpear3>();
        protected override bool PlayOnKillSound => true;
    }
}
