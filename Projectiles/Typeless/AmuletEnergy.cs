using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Projectiles.Typeless
{
    public class AmuletEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Color bColor = Color.White;
        public bool canDamage = false;
        public bool visuals => Owner.Calamity().sSpiritAmuletVisual; // Enables/disables visuals and sounds based on accessory visibility
        public ref float time => ref Projectile.ai[0];
        public ref float energyNumber => ref Projectile.ai[1]; // The number assigned to this energy, ranging from 1 to the max number of energies
        public bool idle => Projectile.ai[2] == 0; // Floating around the player
        public bool healing = false; // If the energies should heal the player. If false they will attack
        public NPC targeted;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 20;
        }

        public override void AI()
        {
            int startTime = 100; // Time after launch when it will begin attacking/healing
            int endTime = 300; // Time after launch when it's movement code reaches it's cap on strength

            // Color is in blues if idle or healing, but becomes orange if they are attacking
            float colorShift = (idle || healing ? 0 : Utils.GetLerpValue(0, startTime, time));
            float rate = Main.GlobalTimeWrappedHourly * 5;
            List<Color> eColors = new List<Color>()
            {
                Color.Lerp(Color.Aquamarine, Color.LightSalmon, colorShift),
                Color.Lerp(Color.MediumTurquoise, Color.Coral, colorShift)
            };

            bColor = CommonProjectileAI.CycleEnergyColor(rate, eColors);

            if (idle || time < startTime) // If not attacking/healing keep it alive
                Projectile.timeLeft++;

            if (Owner.dead || !Owner.Calamity().sSpiritAmulet)
                Projectile.Kill();

            if (Utils.Distance(Owner.Center, Projectile.Center) > 1100) // If it's too far away either teleport to the player if idle (or kill it if needed)
            {
                if (idle)
                    Projectile.Center = Owner.Center;
                else if (targeted == null && !healing)
                    Projectile.Kill();
            }

            // Emit some light
            Lighting.AddLight(Projectile.Center, bColor.ToVector3() * 0.65f);

            if (idle) // Following Ai
            {
                if (time == 0 && visuals) // On spawn fx
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                        Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 4).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * 2;
                        float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f;

                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), vel);
                        dust2.scale = scale * 3;
                        dust2.noGravity = false;
                        dust2.alpha = 180;
                        dust2.color = Main.rand.NextBool(4) ? Color.Lerp(Color.Yellow, bColor, 0.5f) : bColor;
                        dust2.noLight = true;
                        dust2.noLightEmittence = true;
                    }
                }
                if (time > 80) // Track the player in a natural motion simular to swimming
                {
                    Projectile.FollowEnergyOwner(Owner, ref time, ref energyNumber, 0.005f, 60);
                }
                else
                    Projectile.velocity *= 0.99f;
            }
            else // Attack/Healing Ai
            {
                if (Projectile.ai[2] == 5) // Effects for the moment they stop being idle
                {
                    Projectile.netUpdate = true;
                    time = 0;
                    healing = (Owner.statLife < Owner.statLifeMax2 * 0.5f);
                    Projectile.ai[2]++;
                    Projectile.velocity = Vector2.Lerp(Utils.DirectionTo(Owner.Center, Projectile.Center), Owner.velocity.SafeNormalize(Vector2.UnitX), 0.6f) * Main.rand.NextFloat(4.5f, 5.5f);
                }
                if (time <= startTime)
                    Projectile.velocity *= 0.99f;

                if (healing) // I wonder what this does :clueless:
                {
                    float sine = (float)Math.Sin((time * 0.3f) / MathHelper.Pi);
                    Projectile.extraUpdates = 6;
                    
                    if (time > startTime)
                    {
                        float homingSpeed = Utils.Remap(time, startTime, endTime, 0.01f, 0.1f);

                        Vector2 goalPosition = Owner.Center;

                        if (Projectile.velocity.Length() < 5)
                            Projectile.velocity = Projectile.velocity.RotatedBy(0.02f * sine) * 0.99f + Utils.DirectionTo(Projectile.Center, goalPosition) * homingSpeed;
                        else
                            Projectile.velocity *= 0.985f;

                        if (Utils.Distance(goalPosition, Projectile.Center) < 50)
                        {
                            Owner.HealPlayer((energyNumber % 2 == 0 ? 3 : 4));
                            Projectile.netUpdate = true;
                            Projectile.Kill();
                        }
                    }
                }
                else // Attacking
                {
                    canDamage = true;
                    Projectile.extraUpdates = 6;

                    if (time > startTime)
                    {
                        float homingSpeed = Utils.Remap(time, startTime, endTime, 0.01f, 0.1f);

                        targeted = Projectile.Center.ClosestNPCAt(1200);
                        CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, homingSpeed, 25, 0.99f, accelerate: true);

                        if (targeted == null)
                        {
                            Projectile.extraUpdates = 2;
                            if (Projectile.velocity.Y > -5)
                            {
                                Projectile.velocity.Y -= 0.8f * homingSpeed;
                                Projectile.velocity.X *= 0.997f;
                            }
                        }
                        else
                            Projectile.timeLeft++;
                    }
                }
            }

            float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
            if (squash > 0.15f && visuals)
            {
                Particle fadeInfx = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 15, 0.4f * Projectile.scale, bColor * 0.3f * squash, new Vector2(1 - 0.15f * squash, 1f), true, false, shrinkSpeed: 0.3f * squash);
                GeneralParticleHandler.SpawnParticle(fadeInfx);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.5f, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 180);
            Projectile.netUpdate = true;
        }
        public override void OnKill(int timeLeft)
        {
            if (!healing && visuals)
            {
                for (int i = 0; i <= 4; i++)
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 4).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * 4;
                    float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f * Projectile.scale;

                    Particle sparks = new CustomSpark(Projectile.Center + vel, vel, "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(13, 16 + 1), scale, bColor * 0.7f, new Vector2(1f, 1), true, false, 0, false, shrinkSpeed: 0.25f);
                    GeneralParticleHandler.SpawnParticle(sparks);

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), vel);
                    dust2.scale = scale * 3;
                    dust2.noGravity = false;
                    dust2.alpha = 180;
                    dust2.color = Main.rand.NextBool(4) ? Color.Lerp(Color.Yellow, bColor, 0.5f) : bColor;
                    dust2.noLight = true;
                    dust2.noLightEmittence = true;
                }
            }
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> orb = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            float sine = (float)Math.Sin((Main.GlobalTimeWrappedHourly * 10) / MathHelper.Pi);
            Vector2 squash = new Vector2(Utils.Remap(Projectile.velocity.Length(), 1, 5, 1, 0.6f), Utils.Remap(Projectile.velocity.Length(), 1, 5, 1, 2f));

            for (int i = 0; i < 6; i++)
            {
                Color orbColor = Color.Lerp(bColor, Color.Yellow, (i + 1) / 6) with { A = 0 } * 0.4f * (visuals ? 1 : 0.1f);
                Vector2 scale = Projectile.scale * squash * (0.05f + i * 0.01f) * 3;
                Main.EntitySpriteDraw(orb.Value, Projectile.Center - Main.screenPosition, null, orbColor, Projectile.rotation, orb.Size() * 0.5f, scale, SpriteEffects.None);
            }

            return false;
        }
        public override bool? CanDamage() => canDamage ? null : false;
        public override bool? CanCutTiles() => false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteFlags(canDamage, healing);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reader.ReadFlags(out canDamage, out healing);
        }
    }
}
