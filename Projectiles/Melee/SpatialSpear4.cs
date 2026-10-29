using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear4 : BaseSpatialSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private const int TimeLeft = 60;
        protected override int Lifetime => TimeLeft;
        protected override int? Penetration => null;
        protected override Vector3 LightColor => new Vector3(0.5f, 0.5f, 0.05f);
        protected override Color AlphaColor => new Color(byte.MaxValue, byte.MaxValue, 128);
        protected override int DustType => DustID.CopperCoin;
    }
}
