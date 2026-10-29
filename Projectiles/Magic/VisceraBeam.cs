using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VisceraBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int storedPenetrate;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 9;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            storedPenetrate = Projectile.penetrate = 7;
            Projectile.MaxUpdates = 100;
            Projectile.timeLeft = 900;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTileCollision(oldVelocity);
            for (int i = 0; i <= 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, (!ChildSafety.Disabled ? DustID.Cloud : (Main.rand.NextBool() ? 60 : DustID.Blood)));
                dust.position = Projectile.Center;
                dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
                dust.velocity = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.9f);
                dust.noGravity = true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 60);

            if (Projectile.ai[1] > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VisceraBoom>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack * 4, Projectile.owner, 0f, Projectile.ai[1]);
            }

            if (Projectile.ai[2] < 1)
            { 
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) * Main.rand.NextFloat(3, 5), ModContent.ProjectileType<BloodstoneHealOrb>(), 5, 0f, Projectile.owner);
                Projectile.ai[2]++;
            }
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (Projectile.ai[1] > 0)
                Projectile.penetrate = 1;

            // On-hit effects of piercing beams
            if (Projectile.ai[1] == 0f && Projectile.penetrate != storedPenetrate)
            {
                SoundStyle hitSound = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
                SoundEngine.PlaySound(hitSound with { Volume = 0.7f }, Projectile.Center);

                for (int i = 0; i <= 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (!ChildSafety.Disabled ? DustID.Cloud : (Main.rand.NextBool() ? 60 : DustID.Blood)));
                    dust.scale = Main.rand.NextFloat(0.7f, 1.4f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.8f, 1.9f);
                    dust.noGravity = true;
                }
                storedPenetrate = Projectile.penetrate;
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 16f)
            {
                if (Projectile.ai[1] > 0)
                {
                    for (int i = 0; i <= 25; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, (!ChildSafety.Disabled ? DustID.Cloud : (Main.rand.NextBool() ? 60 : DustID.Blood)));
                        dust.scale = Main.rand.NextFloat(0.9f, 1.9f);
                        dust.velocity = Projectile.velocity.RotatedByRandom(0.6) * Main.rand.NextFloat(1.8f, 2.9f);
                        dust.noGravity = true;
                    }
                }
                else
                {
                    for (int i = 0; i <= 10; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, (!ChildSafety.Disabled ? DustID.Cloud : (Main.rand.NextBool() ? 60 : DustID.Blood)));
                        dust.scale = Main.rand.NextFloat(0.7f, 1.4f);
                        dust.velocity = Projectile.velocity.RotatedByRandom(0.6) * Main.rand.NextFloat(0.8f, 1.9f);
                        dust.noGravity = true;
                    }
                }
            }
            if (Projectile.localAI[0] > 16f)
            {
                int bloody = Dust.NewDust(Projectile.Center, 1, 1, (!ChildSafety.Disabled ? DustID.Cloud : DustID.Blood));
                Main.dust[bloody].position = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                Main.dust[bloody].scale = Main.rand.NextFloat(0.3f, 0.8f);
                Main.dust[bloody].velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f);
                Main.dust[bloody].noGravity = true;
                if (Projectile.localAI[0] % 3 == 0 && targetDist < 1400f)
                {
                    AltSparkParticle spark = new AltSparkParticle(Projectile.Center - Projectile.velocity * 0.5f, Projectile.velocity * 0.01f, false, 7, 0.8f, (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.DarkRed));
                    GeneralParticleHandler.SpawnParticle(spark);
                    SparkParticle spark2 = new SparkParticle(Projectile.Center - Projectile.velocity * 0.5f, Projectile.velocity * 0.01f, false, 4, 0.65f, (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.Red));
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
