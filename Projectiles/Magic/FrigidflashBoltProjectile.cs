using System;
using System.Net;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FrigidflashBoltProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float time => ref Projectile.ai[0];
        public bool bigMagic => Projectile.ai[1] == 5;
        public int wallBounces = 0;
        public float fadeIn = 0;
        public bool launch = true;
        public Color fireColor = Color.OrangeRed;
        public Color iceColor = Color.DeepSkyBlue;
        public SlotId chargeSound;
        public int launchTime = 0;
        public Vector2 endPoint;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 480;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Color lightColor = Color.Lerp(fireColor, iceColor, Utils.GetLerpValue(300, 0, Projectile.timeLeft, true));
            Lighting.AddLight(Projectile.Center, lightColor.ToVector3());
            Vector2 mouse = Owner.Calamity().mouseWorld;
            fadeIn = launch ? Utils.GetLerpValue(0, Owner.itemAnimationMax * 0.5f * Projectile.MaxUpdates, time, true) : 1;
            Vector2 velocity = Utils.DirectionTo(Owner.Center, mouse) * 10;
            Projectile.scale = fadeIn * (bigMagic ? 1.3f : 1);
            if (fadeIn < 1) // Hold the projectile in front of the player while they case the spell
            {
                Projectile.Center = Owner.Center + velocity.SafeNormalize(Vector2.UnitX) * 48;
                if (bigMagic && time % 7 == 0)
                {
                    if (time == 0)
                    {
                        chargeSound = SoundEngine.PlaySound(FrigidflashBolt.ChargeSound with { Volume = 0.6f, Pitch = 0.75f }, Projectile.Center);
                    }
                    Vector2 vel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1f, 3f) * fadeIn;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vel * 3, ModContent.DustType<LightDust>(), vel);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.75f, 1.1f) * fadeIn;
                    dust.color = Main.rand.NextBool() ? iceColor : fireColor;
                    dust.noLightEmittence = true;
                }
                if (SoundEngine.TryGetActiveSound(chargeSound, out var ChargeSound) && ChargeSound.IsPlaying)
                    ChargeSound.Position = Projectile.Center;
            }
            else
            {
                if (launch) // On launch, spawns some effects and launch the projectile at the mouse
                {
                    Projectile.tileCollide = true;
                    Vector2 staticSpeed = Utils.DirectionTo(Owner.Center, mouse) * Utils.Distance(Owner.Center, Owner.ClampedMouseWorld()) * 0.01f;
                    Projectile.velocity = (bigMagic ? staticSpeed : velocity);
                    endPoint = Owner.ClampedMouseWorld();

                    SoundEngine.PlaySound(FrigidflashBolt.UseSound with { Volume = 1f, Pitch = (bigMagic ? -0.15f : 0.15f) }, Projectile.Center);
                    if (bigMagic)
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HalleysInfernoShoot") with { Volume = 0.65f, Pitch = 0.45f }, Projectile.Center);
                    for (int i = 0; i < (bigMagic ? 14 : 8); i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1.1f, 1.9f) * (bigMagic ? 2 : 1));
                        dust.noGravity = false;
                        dust.scale = Main.rand.NextFloat(0.65f, 1f) * (bigMagic ? 1.5f : 1);
                        dust.color = Main.rand.NextBool() ? fireColor : iceColor;
                        dust.noLightEmittence = true;

                        if (i % 2 == 0)
                        {
                            bool type = Main.rand.NextBool();
                            float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                            float fxScale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.4f;
                            Vector2 fxVelocity = (Projectile.velocity * 2).RotatedBy(variance) * Main.rand.NextFloat(0.6f, 1f) * (1 - Math.Abs(variance)) * (bigMagic ? 2 : 1);

                            Particle fx = new VelChangingSpark(Projectile.Center, fxVelocity * 1.3f, fxVelocity.RotatedBy(variance * 1.3f), "CalamityMod/Particles/FullStar", 22, fxScale, type ? fireColor : iceColor, new Vector2(2f, 1f), true, false, shrinkSpeed: 0.3f, lerpRate: 0.2f);
                            GeneralParticleHandler.SpawnParticle(fx);
                        }
                    }
                    launch = false;
                }

                if (Main.rand.NextBool(bigMagic ? 3 : 8) && launchTime < 56)
                {
                    bool type = Main.rand.NextBool();
                    Particle fx = new CustomSpark(Projectile.Center + Main.rand.NextVector2Circular(12, 12), -Projectile.velocity * 0.3f, type ? "CalamityMod/Particles/FireTypeParticle" : "CalamityMod/Particles/IceTypeParticle", false, 32, 0.9f, type ? fireColor : iceColor, new Vector2(0.8f, 1f), true, false, shrinkSpeed: type ? 0.2f : 0);
                    GeneralParticleHandler.SpawnParticle(fx);
                }

                float velFade = Utils.GetLerpValue(1.5f, 6.5f, Projectile.velocity.Length(), true);
                Particle iceTrail = new CustomSpark(Projectile.Center + (Vector2.One * 3 * Projectile.scale).RotatedBy(Projectile.rotation), -Projectile.velocity * 0.3f, "CalamityMod/Particles/BloomCircle", false, 7, 0.2f, iceColor * 0.9f, new Vector2(1, 1f), true, false, shrinkSpeed: 0.8f * velFade);
                GeneralParticleHandler.SpawnParticle(iceTrail);
                Particle fireTrail = new CustomSpark(Projectile.Center - (Vector2.One * 3 * Projectile.scale).RotatedBy(Projectile.rotation), -Projectile.velocity * 0.3f, "CalamityMod/Particles/BloomCircle", false, 7, 0.2f, fireColor * 0.9f, new Vector2(1, 1f), true, false, shrinkSpeed: 0.8f * velFade);
                GeneralParticleHandler.SpawnParticle(fireTrail);
            }
            if (bigMagic)
            {
                if (!launch)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, endPoint, 0.035f);
                    Projectile.velocity *= 0.9f;
                    launchTime++;
                    if (launchTime >= (Owner.itemAnimationMax * 2.5f + 28))
                        Projectile.Kill();
                }
            }
            else
            {
                // Gain gravity after a while
                // This makes it more useful on the surface where you otherwise have less tiles to work with
                if (Projectile.timeLeft < 260)
                {
                    Projectile.velocity.X *= 0.9711f;
                    if (Projectile.velocity.Y < 15)
                        Projectile.velocity.Y += 0.19f;
                    if (Projectile.velocity.Y < 5)
                        Projectile.velocity.Y *= 0.977f;
                    wallBounces = 2; // Will always expire on the next tile collide
                }
                else if (wallBounces > 1) // If you only have one bounce before death, reduce lifetime to start falling faster
                    Projectile.timeLeft--;
            }
            
            Projectile.rotation += (0.7f * Projectile.direction / Projectile.MaxUpdates) * Projectile.scale;
            time++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bigMagic)
            {
                Projectile.Kill();
                return false;
            }

            // Allow bounced bolts to hit enemies already hit before the bounce
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;

            Projectile.BounceOnTileCollision(oldVelocity);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            smallMagicExplosion();
            
            if (wallBounces >= 2)
            {
                Projectile.Kill();
            }
            wallBounces++;

            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(BuffID.OnFire3, 60);
            target.AddBuff(BuffID.Frostburn2, 60);

            float minMult = 0.2f;
            int hitsToMinMult = 3;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnKill(int timeLeft)
        {
            if (bigMagic) // Collapsing explosion for the big attack
            {
                // This explosion gets full damage because it spawns another itself, it actually only deals a portion of the projectile damage
                Projectile bigBlast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FireImplosion>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, 0, 0, 5);
                bigBlast.ArmorPenetration = 20; // Hits rapidly enough that it needs some
                float rot = Main.rand.NextFloat(-2, 2);
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 6f).ToRotationVector2().RotatedBy(rot - MathHelper.ToRadians(90)) * 8f;
                    Particle trail = new VelChangingSpark(Projectile.Center + velocity * 20, -velocity * 0.2f, -velocity * 5f, "CalamityMod/Particles/IceTypeParticle", 55, 1.8f, Color.Lerp(iceColor, Color.White, 0.5f), new Vector2(1.1f, 1f), true, false, shrinkSpeed: 0.15f, lerpRate: 0.01f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
                for (int n = 0; n < 15; n++)
                {
                    Vector2 vel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(3, 15);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vel * 14, ModContent.DustType<LightDust>(), -vel);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.2f, 1.7f);
                    dust.color = iceColor;
                    dust.noLightEmittence = true;
                }
            }
            else
            {
                if (wallBounces < 2)
                {
                    smallMagicExplosion();
                }
                SoundEngine.PlaySound(FrigidflashBolt.ProjDeathSound, Projectile.Center);
            }
        }
        public void smallMagicExplosion()
        {
            // Create Blast
            float blastSize = 80;
            float minMultiplier = 0.25f;
            int hitsToMinMult = 4;
            int debuff1 = BuffID.Frostburn2;
            int debuff2 = BuffID.OnFire3;
            int debuffTime = 90;
            Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
            blast.localAI[0] = debuff1;
            blast.localAI[2] = debuff2;
            blast.localAI[1] = debuffTime;
            blast.timeLeft = 15;
            blast.DamageType = DamageClass.Magic;

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f, Pitch = 0.3f, MaxInstances = -1 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f, Pitch = -0.3f, MaxInstances = -1 }, Projectile.Center);

            // "Snowflake" visual effect, but now with fire too
            float rot = Main.rand.NextFloat(-2, 2);
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 6f).ToRotationVector2().RotatedBy(rot) * 6f;
                Particle trail = new CustomSpark(Projectile.Center + velocity * 3, velocity * 0.5f, "CalamityMod/Particles/IceTypeParticle", false, 25, 1.3f, iceColor, new Vector2(1, 1.8f), true, false, shrinkSpeed: -0.45f);
                GeneralParticleHandler.SpawnParticle(trail);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), velocity * 1.5f);
                dust.noGravity = true;
                dust.scale = 1.45f;
                dust.color = fireColor;
                dust.noLightEmittence = true;
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 6f).ToRotationVector2().RotatedBy(rot - MathHelper.ToRadians(90)) * 6f;
                Particle trail = new CustomSpark(Projectile.Center + velocity * 6, velocity * 0.5f, "CalamityMod/Particles/FireTypeParticle", false, 25, 1.5f, fireColor, new Vector2(1.8f, 1f), true, false, shrinkSpeed: 0.25f);
                GeneralParticleHandler.SpawnParticle(trail);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), velocity * 2f);
                dust.noGravity = true;
                dust.scale = 1.25f;
                dust.color = iceColor;
                dust.noLightEmittence = true;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = Color.Lerp(fireColor, iceColor, Utils.GetLerpValue(300, 0, Projectile.timeLeft, true));
            Projectile.DrawProjectileWithBackglow(drawColor with { A = 0 }, Color.White, 3f * fadeIn);
            return false;
        }
    }
}
