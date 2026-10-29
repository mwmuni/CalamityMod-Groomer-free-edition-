using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ApoctosisArrayHoldout : BaseManaChargingGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<ApoctosisArray>();
        public override float MaxOffsetLengthFromArm => 47f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -13f;
        public override float OffsetYDownwards => 5f;
        public override void Shoot(bool big)
        {
            float manaPercent = ((float)Owner.statMana / (float)Owner.statManaMax2);
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 firingVelocity = (shootDirection * 4);
            float damageBoost = Math.Max((manaPower * 60) * (manaPower >= 1 ? 1.5f : 1f), 1);

            if (big)
            {
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                
                SoundStyle fire = new("CalamityMod/Sounds/Item/ImpalerLaunch");
                for (int i = 0; i < (manaPower >= 1 ? 2 : 1); i++)
                    SoundEngine.PlaySound(fire with { Volume = 0.9f, Pitch = -0.7f, MaxInstances = 2 }, Projectile.Center);
                
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/PulseRifleFire");
                SoundEngine.PlaySound(fire2 with { Volume = 0.75f, Pitch = (manaPower >= 1 ? 0.7f : 0.5f) }, Projectile.Center);

                Owner.SetScreenshake(12f * manaPower);
                float projSpeedMult = (manaPower + 0.4f) * (manaPower >= 1 ? 3f : 2f);

                float variance = Main.rand.NextFloat(-0.8f, 0.8f);
                Vector2 vel = firingVelocity * projSpeedMult;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, vel, ModContent.ProjectileType<ApoctosisBlast>(), (int)(Projectile.damage * damageBoost), Projectile.knockBack * 4, Projectile.owner, 0, 0, 5);
                
                OffsetLengthFromArm -= 24f * manaPower;
                Owner.velocity -= Projectile.velocity * 14f * manaPower;
                Projectile.rotation += 0.5f * Projectile.direction;

                for (int i = 0; i < Math.Max(3, 30 * manaPower); i++)
                {
                    Particle sparks = new SparkParticle(GunTipPosition, Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(7, 25), false, 35, Main.rand.NextFloat(0.5f, 0.9f), Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }
                Particle pulse3 = new CustomSpark(GunTipPosition, shootDirection * 7, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 23, 0.085f * manaPower, effectsColor, new Vector2(2f, 0.7f), shrinkSpeed: 0.1f);
                GeneralParticleHandler.SpawnParticle(pulse3);
                Particle pulse4 = new CustomSpark(GunTipPosition, shootDirection * 13, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 18, 0.06f * manaPower, effectsColor, new Vector2(2f, 0.7f), shrinkSpeed: 0.1f);
                GeneralParticleHandler.SpawnParticle(pulse4);
            }
            else
            {
                SoundStyle shoot = new("CalamityMod/Sounds/Item/ApoctosisShoot");
                SoundEngine.PlaySound(shoot with { Volume = 0.25f, Pitch = 0.5f - 1.1f * manaPercent, MaxInstances = -1 }, Projectile.Center);
                if (shotNum % 2 == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity, ModContent.ProjectileType<IonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
                    for (int k = 1; k <= 10; k++)
                    {
                        Color useColor = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.3f) : effectsColor;
                        Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), shootVel);
                        dust2.scale = Main.rand.NextFloat(0.85f, 1f);
                        dust2.noGravity = true;
                        dust2.color = useColor;

                        if (k % 2 == 0)
                        {
                            Particle blast = new CustomSpark(GunTipPosition, Projectile.velocity * 3 * k, "CalamityMod/Particles/BloomLineSoftEdge", false, 11, 0.04f, useColor, new Vector2(1f, 0.8f), shrinkSpeed: 0.8f);
                            GeneralParticleHandler.SpawnParticle(blast);
                        }
                    }
                }
                else
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 vel = firingVelocity.RotatedBy(0.23f * i);
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, vel, ModContent.ProjectileType<IonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, i);

                        shootDirection = shootDirection.RotatedBy(0.23f * i);
                        for (int k = 1; k <= 10; k++)
                        {
                            Color useColor = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.3f) : effectsColor;
                            Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), shootVel);
                            dust2.scale = Main.rand.NextFloat(0.85f, 1f);
                            dust2.noGravity = true;
                            dust2.color = useColor;

                            if (k % 2 == 0)
                            {
                                Particle blast = new CustomSpark(GunTipPosition, vel.SafeNormalize(Vector2.UnitX) * 3 * k, "CalamityMod/Particles/BloomLineSoftEdge", false, 11, 0.04f, useColor, new Vector2(1f, 0.8f), shrinkSpeed: 0.8f);
                                GeneralParticleHandler.SpawnParticle(blast);
                            }
                        }
                    }
                }
                OffsetLengthFromArm -= 5f;

                shotNum++;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float manaPercent = ((float)Owner.statMana / (float)Owner.statManaMax2);
            if (time < 2)
                return false;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/ApoctosisArrayGlow").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 shake = Main.rand.NextVector2Circular(2, 2) * manaPower;

            Main.EntitySpriteDraw(tex, drawPosition + shake, null, Projectile.GetAlpha(lightColor), drawRotation, tex.Size() * 0.5f, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(texGlow, drawPosition + shake, null, Color.White, drawRotation, texGlow.Size() * 0.5f, Projectile.scale, flipSprite);

            if (shootingTimer >= 0)
            {
                float reverseManaPower = MathHelper.Lerp(0.7f, 0.1f, manaPower > 0 ? (1 - manaPower) : manaPercent);
                for (int i = 0; i < 5; i++)
                {
                    float iMult = (1 - 0.1f * i);
                    if (manaPower > 0)
                        Main.EntitySpriteDraw(tex2, GunTipPosition - Main.screenPosition + shake, null, Color.Lerp(effectsColor, Color.White, i * 0.1f) with { A = 0 }, Main.rand.NextFloat(-5f, 5f), tex2.Size() * 0.5f, new Vector2(1f, 0.35f) * 0.75f * manaPower * Main.rand.NextFloat(0.7f, 1.3f) * iMult, flipSprite);

                    for (int b = -1; b <= 1; b += 2)
                    {
                        float sine = MathHelper.Lerp(((float)Math.Sin(Main.GlobalTimeWrappedHourly * (firing ? 20f : 35f) / MathHelper.Pi)), reverseManaPower * b, 0.75f);
                        Vector2 scale = new Vector2(0.3f, 1f * sine * b) * (Main.rand.NextFloat(3f, 4.5f) * iMult + manaPower * 1.2f);
                        float rotation = Projectile.rotation + (time * manaPower * Math.Max(i - 2, 0) * 0.2f) + MathHelper.PiOver4 * b;
                        Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition + Projectile.velocity.RotatedBy(0.44f * Projectile.direction) * 11.7f, null, Color.Lerp(effectsColor, Color.White, i * 0.1f) with { A = 0 }, rotation, sparkle.Size() * 0.5f, scale, flipSprite);
                    }
                }
            }
            return false;
        }
    }
}
