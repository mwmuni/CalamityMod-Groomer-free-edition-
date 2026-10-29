using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class PumpkaboomBig : BasePumpkaboomProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/PumpkaboomBig";

        protected override int ProjectileWidth => 32;
        protected override int ProjectileHeight => 40;
        protected override float MidAirRotationSpeed => 0.03f;
        protected override string StuckTickSoundPath => "CalamityMod/Sounds/Item/PumpkaboomStealthTicking";
        protected override float CollisionRadius => 21;
        protected override float GlowPulseSpeed => 25f;
        protected override int GlowCopyCount => 20;

        protected override void UpdateStuckEffects()
        {
            float pullStrength = 9f;
            float centerShift = 1f;
            float maxPullDistance = 240f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target != null && target.active && target.CanBeMoved() && Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1) && Vector2.Distance(target.Center, Projectile.Center) < maxPullDistance)
                {
                    if (target.whoAmI == stuckNPC)
                        continue;

                    Vector2 moveDir = target.Center.DirectionTo(Projectile.Center).SafeNormalize(Vector2.UnitX);
                    target.velocity = Vector2.Lerp(target.velocity, moveDir * pullStrength, 0.15f);
                    target.Center += moveDir * centerShift;
                }
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 dustVel2 = Vector2.UnitX.RotatedByRandom(100) * Main.rand.NextFloat(9.5f, 13f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel2.SafeNormalize(Vector2.UnitX) * 160, ModContent.DustType<SquashDust>(), -dustVel2 * 1.2f, 0, default, Main.rand.NextFloat(0.9f, 1.4f));
                dust2.noGravity = true;
                dust2.fadeIn = 0.8f;
                dust2.color = c1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle w = new("CalamityMod/Sounds/Item/WulfrumScrewdriverThud");
            SoundEngine.PlaySound(w with { Volume = 0.7f, Pitch = 0f, MaxInstances = 6 }, Projectile.Center);

            StickToTarget(target);

            for (int i = 0; i < 6; i++)
            {
                int sparkLifetime = Main.rand.Next(13, 22);
                float sparkScale = Main.rand.NextFloat(0.55f, 1.1f);
                Color sparkColor = Main.rand.NextBool() ? c1 : c2;

                Vector2 burstDirection = Projectile.oldVelocity + new Vector2(Main.rand.NextFloat(-2.25f, 2.25f), Main.rand.NextFloat(-6.75f, 6.75f));
                Vector2 spawnPos = target.Center + burstDirection * target.width * 0.15f;
                Vector2 sparkVelocity = burstDirection * Main.rand.NextFloat(0.7f, 1.3f);

                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(spawnPos, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(spawnPos, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.localAI[1] = 5; // Reset firing state
            if (flungState != -1f)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PumpkinExplode1") with { Pitch = 0f, Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") with { Pitch = 0f, Volume = 0.5f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    Owner.SetScreenshake(5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumpkaboomBoomBig>(), Projectile.damage * 2, Projectile.knockBack * 2, Projectile.owner, 0);
                }

                float scale = 0.18f;
                Vector2 spawnPos = Projectile.Center;
                Particle blastRing = new CustomPulse(spawnPos, Vector2.Zero, c1, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0.07f, scale, 10);
                GeneralParticleHandler.SpawnParticle(blastRing);
                Particle blastRing2 = new CustomPulse(spawnPos, Vector2.Zero, c1 * 0.33f, "CalamityMod/Particles/HighResHollowCircleHardEdge", Vector2.One, Main.rand.NextFloat(-10, 10), 0.07f, scale * 1.66f, 11);
                GeneralParticleHandler.SpawnParticle(blastRing2);
                for (int k = 0; k < 16; k++)
                {
                    Vector2 velocity = new Vector2(12, 12).RotatedByRandom(100) * Main.rand.NextFloat(0.7f, 1.3f);
                    Particle spark = new SparkParticle(Projectile.Center + velocity, velocity, false, 45, Main.rand.NextFloat(0.95f, 1.35f), c1);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int k = 0; k < 30; k++)
                {
                    Vector2 velocity = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + velocity, DustID.SolarFlare, velocity);
                    dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                    dust2.noGravity = true;
                }
            }

            Projectile.netUpdate = true;
        }
    }
}
