using System;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class AsgardsValorDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override void Load()
        {
            ID = DashID;
        }

        public override float CalculateDashSpeed(Player player) => 16.9f;

        public override void DashStartupEffects(Player player)
        {
            player.velocity *= 0.9f;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {

            // Dash at a faster speed than the default value.
            dashSpeed = 12.5f;
            // Spawn fire dust around the player's body.
            if (DashTimeAdjustedForStartup > 14)
                return;
            for (int d = 0; d < 3; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, Main.rand.NextBool() ? 296 : 158, 0f, 0f, 0, default, 1.2f);
                holyFireDashDust.velocity = -player.velocity * Main.rand.NextFloat(0.1f, 0.75f);
                holyFireDashDust.scale *= Main.rand.NextFloat(2f, 2.4f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                holyFireDashDust.noGravity = true;
                if (Main.rand.NextBool())
                    holyFireDashDust.fadeIn = 0.1f;
            }
            Vector2 dustPosition = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.7f);
            Dust dust = Dust.NewDustPerfect(dustPosition, DustID.FireworkFountain_Yellow, -player.velocity * Main.rand.NextFloat(0.15f, 0.4f), 0, default, 0.5f);
            dust.noGravity = false;
            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (DashTimeAdjustedForStartup > 14)
                return;
            DashHitContextHelper.Set(player, ref hitContext, AsgardsValor.ShieldSlamIFrames, DamageClass.Melee, AsgardsValor.ShieldSlamDamage, AsgardsValor.ShieldSlamKnockback);

            int Dusts = 12;
            float radians = MathHelper.TwoPi / Dusts;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
            for (int k = 0; k < Dusts; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k);
                Dust dust = Dust.NewDustPerfect(npc.Center, DustID.CrimsonTorch, velocity * 3f, 0, default, 2.5f);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);

                Dust dust2 = Dust.NewDustPerfect(npc.Center, DustID.OrangeTorch, velocity * 5f, 0, default, 2.2f);
                dust2.noGravity = true;
                dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                dust2.color = Color.Salmon;

                Dust dust3 = Dust.NewDustPerfect(npc.Center, DustID.IchorTorch, velocity * 7f, 0, default, 1.9f);
                dust3.noGravity = true;
                dust3.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                dust3.color = Color.SandyBrown;

            }
            for (int k = 0; k < 5; k++)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center, DustID.FireworkFountain_Yellow, new Vector2(0, -3.5f).RotatedByRandom(0.7f) * Main.rand.NextFloat(0.8f, 1.4f), 0, default, 1.2f);
                dust.noGravity = false;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f, PitchVariance = 0.3f }, npc.Center);
        }
    }
}
