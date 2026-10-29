using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomShot2 : BasePhantomShotProjectile
    {
        protected override Color ProjectileColor => new Color(250, 100, 100, Projectile.alpha);
        protected override float HomingEndTime => 300f;
        protected override float HomingSpeedMultiplier => 2f;

        public override string Texture => "CalamityMod/Projectiles/Boss/PhantomGhostShot";
    }
}
