using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarmageddonStar2 : BaseStarmageddonStarProjectile
    {
        protected override int StarProjectileType => ModContent.ProjectileType<StarmageddonStar2>();
        protected override float LightRed => 0.05f;
        protected override float LightGreen => 0.4f;
        protected override float LightBlue => 0.5f;
        protected override Color ParticleColor => Color.Cyan;
        protected override Color MistColor => Color.Blue;
        protected override int ExplosionDustType => DustID.IceTorch;
    }
}
