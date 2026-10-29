using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear3 : BaseSpatialSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private const int TimeLeft = 75;
        private const int TotalSplits = 2;
        protected override int Lifetime => TimeLeft;
        protected override int? Penetration => 2;
        protected override Vector3 LightColor => new Vector3(1f, 0.05f, 0.05f);
        protected override Color AlphaColor => new Color(byte.MaxValue, 128, 128);
        protected override int DustType => DustID.PinkFairy;
        protected override int SplitCount => TotalSplits;
        protected override int SplitAngleDegrees => 20;
        protected override int NextProjectileType => ModContent.ProjectileType<SpatialSpear4>();
    }
}
