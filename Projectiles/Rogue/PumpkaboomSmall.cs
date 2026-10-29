using CalamityMod.Particles;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class PumpkaboomSmall : BasePumpkaboomProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Pumpkaboom";

        protected override int ProjectileWidth => 26;
        protected override int ProjectileHeight => 34;
        protected override float MidAirRotationSpeed => 0.06f;
        protected override string StuckTickSoundPath => "CalamityMod/Sounds/Item/PumpkaboomNormalTicking";
        protected override float CollisionRadius => 18;
        protected override float GlowPulseSpeed => 20f;
        protected override int GlowCopyCount => 15;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle w = new("CalamityMod/Sounds/Item/WulfrumScrewdriverThud");
            SoundEngine.PlaySound(w with { Volume = 0.7f, Pitch = 0f, MaxInstances = 6 }, Projectile.Center);

            StickToTarget(target);
            Projectile.netUpdate = true;

            for (int i = 0; i < 4; i++)
            {
                int sparkLifetime = Main.rand.Next(11, 19);
                float sparkScale = Main.rand.NextFloat(0.5f, 1f);
                Color sparkColor = Main.rand.NextBool() ? c1 : c2;

                Vector2 burstDirection = Projectile.oldVelocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-6f, 6f));
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
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.localAI[1] = 5; // Reset firing state

            if (flungState != -1f)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PumpkinExplode1") with { Pitch = 0f, Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") with { Pitch = 0f, Volume = 0.4f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumpkaboomBoomSmall>(), Projectile.damage * 2, Projectile.knockBack * 2, Projectile.owner, 0);

                float scale = 0.1f;
                Vector2 spawnPos = Projectile.Center;
                Particle blastRing = new CustomPulse(spawnPos, Vector2.Zero, c1, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0.07f, scale, 10);
                GeneralParticleHandler.SpawnParticle(blastRing);
                Particle blastRing2 = new CustomPulse(spawnPos, Vector2.Zero, c1 * 0.33f, "CalamityMod/Particles/HighResHollowCircleHardEdge", Vector2.One, Main.rand.NextFloat(-10, 10), 0.07f, scale * 1.66f, 11);
                GeneralParticleHandler.SpawnParticle(blastRing2);
                for (int k = 0; k < 8; k++)
                {
                    Vector2 velocity = new Vector2(12, 12).RotatedByRandom(100) * Main.rand.NextFloat(0.66f, 1f);
                    Particle spark = new SparkParticle(Projectile.Center + velocity, velocity, false, 24, Main.rand.NextFloat(0.75f, 1.15f), c1);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int k = 0; k < 20; k++)
                {
                    Vector2 velocity = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + velocity, DustID.SolarFlare, velocity);
                    dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                    dust2.noGravity = true;
                }
            }
        }
    }
}
