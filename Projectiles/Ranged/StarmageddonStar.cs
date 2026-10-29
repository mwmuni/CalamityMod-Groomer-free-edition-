using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarmageddonStar : BaseStarmageddonStarProjectile
    {
        protected override int StarProjectileType => ModContent.ProjectileType<StarmageddonStar>();
        protected override float LightRed => 0.5f;
        protected override float LightGreen => 0.4f;
        protected override float LightBlue => 0.05f;
        protected override Color ParticleColor => Color.Orange;
        protected override Color MistColor => Color.Red;
        protected override int ExplosionDustType => DustID.Torch;
    }
}
