using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseManaChargingGunHoldoutProjectile : BaseGunHoldoutProjectile
    {
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 1.5f * Projectile.spriteDirection;
        public override float WeaponTurnSpeed => 0.35f;
        public override float RecoilResolveSpeed => shootingTimer >= 0 ? 0.3f : 0.05f;

        public bool firing => Projectile.ai[2] == 0;
        public ref float shootingTimer => ref Projectile.ai[0];
        public ref float manaPower => ref Projectile.ai[1];
        public Color effectsColor { get; set; } = Color.Crimson;
        public int fireRate => Owner.HeldItem.useAnimation;
        public bool fullyCharged = false;
        public int time = 0;
        public int shotNum = 0;
        public SlotId ionHum { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (Owner.dead)
            {
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                Projectile.Kill();
            }
        }

        public override void HoldoutAI()
        {
            bool isAtMaxMana = ((float)Owner.statMana / (float)Owner.statManaMax2) >= 1;
            Projectile.timeLeft++;

            if (time == 0)
                shootingTimer = fireRate;

            if (shootingTimer < 0)
            {
                OnCooldown();
                shootingTimer++;
                if (shootingTimer == 0)
                {
                    shootingTimer = fireRate;
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum?.Stop();
                    }
                }
                return;
            }

            if (HeldItem.type != Owner.HeldItem.type)
            {
                if (manaPower > 0)
                    shootingTimer = -75;
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                Projectile.Kill();
            }

            if (shootingTimer == 0)
            {
                if (firing)
                {
                    if (Owner.Calamity().mouseRight)
                    {
                        Projectile.ai[2] = 5;
                        return;
                    }
                    if (Owner.CheckMana(HeldItem, -1, true, false))
                    {
                        Shoot(false);
                        shootingTimer = fireRate;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                        shootingTimer = -40;
                    }
                }
                else // Charging mode
                {
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum.Position = Projectile.Center;
                        hum.Pitch = MathHelper.Lerp(-0.5f, 0.1f, manaPower);
                    }
                    else
                    {
                        SoundStyle charge = new("CalamityMod/Sounds/Item/IonChargeLoop");
                        ionHum = SoundEngine.PlaySound(charge with { Volume = 0.7f, IsLooped = true }, Projectile.Center);
                    }

                    if (Owner.Calamity().mouseRight)
                    {
                        if (!isAtMaxMana)
                        {
                            // This gives mana recharge a consistent rate rather than being variable by a frame
                            // Without it the mana recharge rate would change if you started charging on an even value rather than an odd one
                            if (manaPower == 0)
                                time = 100;

                            if (time % 2 == 0)
                            {
                                Owner.statMana += (int)(Owner.statManaMax2 * 0.025f); // 80 frames to reach max mana
                                manaPower += 0.0275f; // 72 frames to reach max power
                                // Mana power reaches max at 90% mana regained

                                if (Owner.statMana > Owner.statManaMax2)
                                    Owner.statMana = Owner.statManaMax2;
                            }
                            if (manaPower > 1)
                                manaPower = 1;

                            Vector2 vel = Projectile.velocity.RotateRandom(0.7f) * Main.rand.NextFloat(7, 14);
                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition + vel * 10, ModContent.DustType<LightDust>(), -vel);
                            dust2.scale = Main.rand.NextFloat(0.35f, 0.55f);
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor;
                        }

                        if (isAtMaxMana && !fullyCharged && manaPower >= 0.25f)
                        {
                            SoundStyle fullCharge = new("CalamityMod/Sounds/Item/DudFire");
                            for (int i = 0; i < 2; i++)
                                SoundEngine.PlaySound(fullCharge with { Volume = .6f, Pitch = -0.7f + i * 1.5f, MaxInstances = 2 }, Projectile.Center);
                            fullyCharged = true;
                            OffsetLengthFromArm -= 7f * manaPower;
                        }

                        OffsetLengthFromArm += 1.3f * manaPower;
                    }
                    else
                    {
                        if (fullyCharged)
                        {
                            Shoot(true);
                            fullyCharged = false;
                        }
                        else // Failure to full charge
                        {
                            SoundEngine.PlaySound(SoundID.Item109 with { Volume = 0.7f, Pitch = 1f }, Projectile.Center);
                            for (int i = 0; i < 10; i++)
                            {
                                Vector2 vel = Projectile.velocity.RotateRandom(Math.PI);
                                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), -vel);
                                dust2.scale = Main.rand.NextFloat(0.95f, 1.35f) * (manaPower + 0.3f);
                                dust2.noGravity = false;
                                dust2.color = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor;
                            }
                        }
                        manaPower = 0;
                        shootingTimer = -75;
                        Projectile.ai[2] = 0; // Return to regular firing mode
                    }
                }
            }
            else
            {
                if (!Owner.Calamity().mouseRight && !Main.mouseLeft)
                {
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum?.Stop();
                    }
                    Projectile.Kill();
                }
            }

            if (shootingTimer > 0)
                shootingTimer--;

            time++;
        }

        public void OnCooldown()
        {
            Projectile.timeLeft++;

            if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
            {
                float lerp = Utils.GetLerpValue(-60, 0, shootingTimer, true);
                hum.Position = Projectile.Center;
                hum.Pitch = MathHelper.Lerp(0f, -0.6f, lerp);
                hum.Volume = 1 - lerp;
            }

            Vector2 smokeVel = new Vector2(0, -6) * Main.rand.NextFloat(0.1f, 1.1f);
            Particle smoke = new HeavySmokeParticle(GunTipPosition, smokeVel, Color.SlateGray, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool());
            GeneralParticleHandler.SpawnParticle(smoke);
        }

        public abstract void Shoot(bool big);
    }
}
