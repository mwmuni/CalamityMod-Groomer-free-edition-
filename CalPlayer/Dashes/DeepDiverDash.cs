using System;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class DeepDiverDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override void Load()
        {
            ID = DashID;
        }

        public override float CalculateDashSpeed(Player player) => 20f;
        public int Time = 0;

        public override void OnDashEffects(Player player)
        {
            Time = 0;
            for (int m = 0; m < 3; m++)
            {
                PointParticle spark = new PointParticle(player.Center - player.velocity, -player.velocity * (0.08f * m), false, 25, 4f - (0.5f * m), (Main.rand.NextBool() ? Color.Aqua : Color.DodgerBlue) * 0.4f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Time++;
            for (int m = 0; m < 5; m++)
            {
                Vector2 dustVel3 = -player.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.03f, 0.2f);
                Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.7f), Main.rand.NextBool(7) ? 278 : 80, dustVel3);
                if (dust.type == 278)
                {
                    dust.scale = 1.2f;
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = false;
                    dust.color = Main.rand.NextBool() ? Color.Aqua : Color.DodgerBlue;
                }
                else
                {
                    dust.scale = Main.rand.NextFloat(0.6f, 1.8f);
                    dust.alpha = 125;
                    dust.noGravity = true;
                }
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }
            if (Time % 2 == 0)
            {
                Vector2 dustVel = -player.velocity.RotatedBy(0.05f + MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;
                Vector2 dustVel2 = -player.velocity.RotatedBy(-0.05f - MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;

                PointParticle spark = new PointParticle(player.Center + new Vector2(0, -15 * player.direction) + dustVel, dustVel, false, 8, 1.4f, (Main.rand.NextBool() ? Color.Aqua : Color.DodgerBlue) * 0.5f);
                GeneralParticleHandler.SpawnParticle(spark);
                PointParticle spark2 = new PointParticle(player.Center + new Vector2(0, 15 * player.direction) + dustVel2, dustVel2, false, 8, 1.4f, (Main.rand.NextBool() ? Color.Aqua : Color.DodgerBlue) * 0.5f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            player.velocity.X *= 0.967f;

            // Dash at a faster speed than the default value.
            dashSpeed = 25f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            SoundStyle hit = new("CalamityMod/Sounds/NPCHit/PerfSmallHit3");
            SoundEngine.PlaySound(hit with { Pitch = 0.7f, Volume = 0.4f }, player.Center);
            for (int i = 0; i <= 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(player.Center, Main.rand.NextBool() ? 278 : 132, player.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.5f, 1f) + new Vector2(0, -3f));
                if (dust.type == 278)
                {
                    dust.scale = 1.2f;
                    dust.color = Main.rand.NextBool() ? Color.Aqua : Color.DodgerBlue;
                }
                else
                {
                    dust.scale = 0.9f;
                }
                dust.noGravity = false;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }

            DashHitContextHelper.Set(player, ref hitContext, DeepDiver.ShieldSlamIFrames, DamageClass.Melee, DeepDiver.ShieldSlamDamage, DeepDiver.ShieldSlamKnockback);
        }
    }
}
