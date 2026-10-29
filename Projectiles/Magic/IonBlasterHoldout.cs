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
    public class IonBlasterHoldout : BaseManaChargingGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<IonBlaster>();
        public override float MaxOffsetLengthFromArm => 30f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -9f;
        public override float OffsetYDownwards => 5f;
        public override void Shoot(bool big)
        {
            float manaPercent = ((float)Owner.statMana / (float)Owner.statManaMax2);
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 firingVelocity = (shootDirection * 4);
            float damageBoost = (manaPower >= 1 ? 1.9f : 1.6f);

            if (big)
            {
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                if (manaPower >= 1)
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/ImpalerLaunch");
                    SoundEngine.PlaySound(fire with { Volume = 0.8f, Pitch = -0.6f }, Projectile.Center);
                }
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/LanceofDestinyStrong");
                SoundEngine.PlaySound(fire2 with { Volume = 0.4f, Pitch = (manaPower >= 1 ? 0.8f : 0.6f) }, Projectile.Center);

                Owner.SetScreenshake(6f * manaPower);
                int maxProj = (int)(manaPower * 24 * (manaPower >= 1 ? 1 : 0.5f));
                for (int i = 0; i < maxProj; i++)
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 vel = firingVelocity.RotatedBy(variance * 0.3f) * (1 - Math.Abs(variance)) * (i % 2 == 0 ? 0.7f : 1) * Main.rand.NextFloat(5f, 5.5f);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, vel, ModContent.ProjectileType<IonBlast>(), (int)(Projectile.damage * damageBoost), Projectile.knockBack * 2, Projectile.owner, 0, 0, 5);
                }
                OffsetLengthFromArm -= 24f * manaPower;
                Owner.velocity -= Projectile.velocity * 12f * manaPower;

                for (int i = 0; i < Math.Max(3, 30 * manaPower); i++)
                {
                    Particle sparks = new SparkParticle(GunTipPosition, Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(7, 25), false, 35, Main.rand.NextFloat(0.5f, 0.9f), Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }
                Particle pulse3 = new CustomSpark(GunTipPosition, shootDirection * 9, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 17, 0.06f, effectsColor, new Vector2(2f, 0.7f), shrinkSpeed: 0.1f);
                GeneralParticleHandler.SpawnParticle(pulse3);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item91 with { Volume = 0.5f, Pitch = 0.8f - 1.3f * manaPercent }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity, ModContent.ProjectileType<IonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                OffsetLengthFromArm -= 5f;

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
                shotNum++;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/IonBlasterGlow").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.Pi * Owner.direction : 0f);
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 shake = Main.rand.NextVector2Circular(2, 2) * manaPower;

            Main.EntitySpriteDraw(tex, drawPosition + shake, null, Projectile.GetAlpha(lightColor), drawRotation, tex.Size() * 0.5f, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(texGlow, drawPosition + shake, null, Color.White, drawRotation, texGlow.Size() * 0.5f, Projectile.scale, flipSprite);

            if (manaPower > 0 && shootingTimer >= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    float iMult = (1 - 0.1f * i);
                    Main.EntitySpriteDraw(tex2, GunTipPosition - Main.screenPosition + shake, null, Color.Lerp(effectsColor, Color.White, i * 0.1f) with { A = 0 }, Main.rand.NextFloat(-5f, 5f), tex2.Size() * 0.5f, new Vector2(1f, 0.55f) * 0.5f * manaPower * Main.rand.NextFloat(0.7f, 1.3f) * iMult, flipSprite);
                }
            }
            return false;
        }
    }
}
