using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Boss
{
    public abstract class BasePhantomShotProjectile : BasePhantomProjectile
    {
        protected override int ProjectileSize => 14;

        protected abstract float HomingEndTime { get; }
        protected abstract float HomingSpeedMultiplier { get; }

        public override void AI()
        {
            PlayInitialSoundAndUpdateRotation();
            UpdateAlphaFade();

            if (Projectile.localAI[0] > 180f && Projectile.localAI[0] < HomingEndTime && Main.expertMode)
            {
                if (Projectile.ai[0] == 0f)
                    Projectile.ai[0] = Projectile.velocity.Length() * HomingSpeedMultiplier;

                int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);
                Vector2 playerDirection = Main.player[playerTracker].Center - Projectile.Center;
                playerDirection.Normalize();
                playerDirection *= Projectile.ai[0];
                Projectile.velocity = (Projectile.velocity * 79 + playerDirection) / 80;
            }
        }
    }
}
