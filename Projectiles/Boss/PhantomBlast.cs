using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomBlast : BasePhantomBlastProjectile
    {
        protected override Color ProjectileColor => new Color(100, 250, 250, Projectile.alpha);
        protected override int BlastDustType => DustID.DungeonSpirit;
        protected override float MinimumVelocity => 10f;
    }
}
