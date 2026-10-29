using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    internal static class BrimstoneBlastKillBehavior
    {
        internal static void HandleKill<TBarrageProjectile>(Projectile projectile, SoundStyle impactSound, int dartDamage, bool gigablast) where TBarrageProjectile : ModProjectile
        {
            SoundEngine.PlaySound(impactSound, projectile.Center);
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            if (projectile.ai[2] == 0f)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int totalProjectiles = gigablast
                        ? death ? 36 : revenge ? 32 : expertMode ? 28 : 20
                        : death ? 16 : revenge ? 14 : expertMode ? 12 : 8;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<TBarrageProjectile>();
                    float velocity = gigablast ? 6.5f : 8f;
                    Vector2 spinningPoint = new Vector2(0f, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity2, type, dartDamage, 0f, projectile.owner, 0f, projectile.ai[1], velocity * 1.5f);
                    }
                }

                if (projectile.ai[1] == 2f)
                {
                    int particleCount = gigablast ? 25 : 18;
                    Vector2 sparkVelocity = gigablast ? new Vector2(15, 15) : new Vector2(12, 12);
                    float sparkScale = gigablast ? 1.25f : 1.1f;
                    for (int i = 0; i < particleCount; i++)
                    {
                        Vector2 velocity = sparkVelocity.RotatedByRandom(100);
                        PointParticle spark = new PointParticle(projectile.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, sparkScale, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }

                    Vector2 dustVelocity = gigablast ? new Vector2(20, 20) : new Vector2(16, 16);
                    float minimumDustScale = gigablast ? 0.9f : 0.75f, maximumDustScale = gigablast ? 1.8f : 1.3f;
                    for (int i = 0; i < particleCount; i++)
                    {
                        Dust failShotDust = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVelocity.RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(minimumDustScale, maximumDustScale);
                    }
                }
            }
        }
    }
}
