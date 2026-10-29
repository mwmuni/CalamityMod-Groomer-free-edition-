using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomShot : BasePhantomShotProjectile
    {
        protected override Color ProjectileColor => new Color(100, 250, 250, Projectile.alpha);
        protected override float HomingEndTime => 240f;
        protected override float HomingSpeedMultiplier => 3f;

        public override string Texture => "CalamityMod/Projectiles/Boss/PhantomHookShot";
    }
}
