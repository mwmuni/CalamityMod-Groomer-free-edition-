using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class ElysianAegisDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;
        public bool PostHit = false;

        public override void Load()
        {
            ID = DashID;
        }

        public override float CalculateDashSpeed(Player player) => 21.5f;

        public override void OnDashEffects(Player player)
        {
            PostHit = false;
        }

        public override void DashStartupEffects(Player player)
        {
            player.velocity *= 0.9f;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {

            if (DashTimeAdjustedForStartup <= 16)
                for (int d = 0; d < 4; d++)
                {
                    Dust hFlameDust = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.2f), Main.rand.NextBool(8) ? 222 : 162, -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                    hFlameDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                    hFlameDust.noGravity = hFlameDust.type == 222 ? false : true;
                    hFlameDust.fadeIn = 0.5f;
                    if (hFlameDust.type == 222)
                    {
                        hFlameDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        hFlameDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                    }
                    if (hFlameDust.type == 180)
                        hFlameDust.scale = Main.rand.NextFloat(1.6f, 2.2f);

                    Dust dust = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(6, 6) - player.velocity * 2, DustID.GoldFlame);
                    dust.velocity = -player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                    dust.noGravity = true;
                    if (d < 1)
                    {
                        Particle spark = new CustomSpark(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.2f), -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 17, Main.rand.NextFloat(1.15f, 1.25f), Main.rand.NextBool(4) ? Color.Khaki : Color.Orange, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.4f, 0.5f));
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            // Dash at a faster speed than the default value.
            dashSpeed = 14f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (DashTimeAdjustedForStartup > 16)
                return;
            if (!PostHit)
            {
                player.SetScreenshake(3.5f);
                PostHit = true;
            }
            float particleScale = Main.rand.NextFloat(0.45f, 0.55f);
            Particle explosion = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Gray * 0.6f, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale + 0.07f, 20, false);
            GeneralParticleHandler.SpawnParticle(explosion);
            Particle explosion2 = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Orange, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale, 20);
            GeneralParticleHandler.SpawnParticle(explosion2);

            DashHitContextHelper.Set(player, ref hitContext, ElysianAegis.ShieldSlamIFrames, DamageClass.Melee, ElysianAegis.ShieldSlamDamage, ElysianAegis.ShieldSlamKnockback);

            // On-hit Supreme Holy Explosion
            int supremeExplosionDamage = (int)player.GetBestClassDamage().ApplyTo(ElysianAegis.RamExplosionDamage);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), supremeExplosionDamage, ElysianAegis.RamExplosionKnockback, Main.myPlayer, 1f, 0f);
            npc.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}
