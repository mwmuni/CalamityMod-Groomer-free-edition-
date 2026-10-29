using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    [PierceResistException]
    // Photoviscerator right click main projectile (invisible flare cluster bomb)
    public class ExoFlareCluster : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int Time = 0;
        public Color sparkColor;
        public bool PostTileHit = false;
        public ref int audioCooldown => ref Main.player[Projectile.owner].Calamity().PhotoAudioCooldown;

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 22;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            Time++;
            List<Color> eColors = new List<Color>()
            {
                Color.OrangeRed,
                Color.MediumTurquoise,
                Color.Orange,
                Color.LawnGreen
            };
            float rate = (Main.GlobalTimeWrappedHourly * 8);
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            sparkColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            if (targetDist < 1400)
            {
                Particle beam3 = new CustomSpark(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(0.01f, 0.1f), "CalamityMod/Particles/SmallBloom", false, 3, 0.4f, sparkColor, new Vector2(1f, 1), true, true);
                GeneralParticleHandler.SpawnParticle(beam3);

                float sine = MathHelper.Clamp((float)Math.Sin(Projectile.timeLeft * 0.875f / MathHelper.Pi), -0.7f, 0.7f);

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 22.5f;

                Particle beam33 = new CustomSpark(Projectile.Center + offset, (-offset.RotatedBy(MathHelper.PiOver2) * 0.3f) * Main.rand.NextFloat(0.01f, 0.1f), "CalamityMod/Particles/SmallBloom", false, 23, 0.3f, sparkColor, new Vector2(1f, 1), true, true, 0, false, false, 0.1f);
                GeneralParticleHandler.SpawnParticle(beam33);
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Doze - Flamethrowers in vanilla are long debuff infliction tools (20 seconds of their debuff).
            //I am applying this as the base for Cal flamethrowers, with shorter times being the exception instead of the rule
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 1200);
            Projectile.tileCollide = false;

            float numberOflines = 5;
            float rotFactorlines = 360f / numberOflines;
            for (int i = 0; i < numberOflines; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactorlines);
                Vector2 offset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                Vector2 velOffset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                Particle spark = new GlowSparkParticle(Projectile.Center + offset, velOffset * Main.rand.NextFloat(5.5f, 8.5f), true, 75, Main.rand.NextFloat(0.03f, 0.05f), sparkColor, new Vector2(0.3f, 1f));
                GeneralParticleHandler.SpawnParticle(spark);

                float rot2 = MathHelper.ToRadians(i * rotFactorlines);
                Vector2 offset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 + Main.rand.NextFloat(0.1f, 5.1f));
                Vector2 velOffset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 + Main.rand.NextFloat(0.1f, 5.1f));

                SquishyLightParticle exoEnergy = new(Projectile.Center + offset2, velOffset2 * Main.rand.NextFloat(0.5f, 2.5f), 0.5f, sparkColor, 35);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }

            Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, sparkColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1.2f, 1.8f * Main.rand.NextFloat(0.9f, 1.1f), 12, true);
            GeneralParticleHandler.SpawnParticle(blastRing);

            if (audioCooldown == 0)
            {
                SoundEngine.PlaySound(Photoviscerator.HitSound, target.Center);
                audioCooldown = 10;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!PostTileHit)
            {
                SoundEngine.PlaySound(DeadSunsWind.Ricochet with { Volume = 1.2f }, Projectile.Center);
                float numberOflines = 25;
                float rotFactorlines = 360f / numberOflines;
                for (int i = 0; i < numberOflines; i++)
                {
                    sparkColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };

                    float rot2 = MathHelper.ToRadians(i * rotFactorlines);
                    Vector2 offset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 * Main.rand.NextFloat(1.1f, 9.1f));
                    Vector2 velOffset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 * Main.rand.NextFloat(1.1f, 9.1f));

                    SquishyLightParticle exoEnergy = new(Projectile.Center + offset2, velOffset2 * Main.rand.NextFloat(0.2f, 1.9f), 0.5f, sparkColor, 40);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }
                PostTileHit = true;
            }

            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 1200);
        }
    }
}
