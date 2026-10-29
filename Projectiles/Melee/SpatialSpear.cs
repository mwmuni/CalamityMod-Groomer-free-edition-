using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear : BaseSpatialSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private int TimeLeft = Main.zenithWorld ? 300 : 180;
        private int TotalSplits = Main.zenithWorld ? 30: 6;
        protected override int Lifetime => TimeLeft;
        protected override int? Penetration => -1;
        protected override Vector3 LightColor => new Vector3(0.05f, 1f, 0.05f);
        protected override Color AlphaColor => new Color(128, byte.MaxValue, 128);
        protected override int DustType => DustID.TerraBlade;
        protected override int SplitCount => TotalSplits;
        protected override int SplitAngleDegrees => 5;
        protected override int NextProjectileType => ModContent.ProjectileType<SpatialSpear2>();
        protected override bool PlayOnKillSound => true;
    }
}
