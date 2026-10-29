using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseEnergyNeedleProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        protected virtual int TrailCacheLength => 5;
        protected virtual float CollisionBladeLength => 44f;
        protected virtual float CollisionStartOffset => 16f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = TrailCacheLength;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        protected virtual void BeforeHitEffects(NPC target)
        {
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = CollisionBladeLength * Projectile.scale;
            Vector2 start = -Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * CollisionStartOffset;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + start, Projectile.Center + start + Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * bladeLength, 24, ref collisionPoint);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1.2f, 2.3f);
                Particle energyLeak = new SquishyLightParticle(Projectile.Center + Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * 40f, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BeforeHitEffects(target);
            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f;
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(particleOrigin, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }
    }
}
