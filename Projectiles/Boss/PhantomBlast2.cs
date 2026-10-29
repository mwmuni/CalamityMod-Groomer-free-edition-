using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomBlast2 : BasePhantomBlastProjectile
    {
        protected override Color ProjectileColor => new Color(250, 100, 100, Projectile.alpha);
        protected override int BlastDustType => DustID.RedTorch;
        protected override float MinimumVelocity => 12f;
    }
}
