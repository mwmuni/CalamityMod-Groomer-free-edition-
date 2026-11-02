using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class ForbiddenOathbladeThrownBlade : BaseDemonSwordThrownBlade
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ForbiddenOathblade";
        protected override float HeldBladeOffset => -55f;
        protected override Color FadeDustSecondaryColor => Color.BlueViolet;
        protected override int DemonicFlamesDamage => 60;
        protected override int MaximumImpales => 3;

        public ForbiddenOathbladeThrownBlade() : base(120, 80)
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            base.SetDefaults();
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 toMouse = (Owner.Calamity().mouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                Projectile.scale = Owner.GetMeleeScale();
                Projectile.localAI[1] = toMouse.ToRotation();
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            bool isOwner = Main.myPlayer == Projectile.owner;
            InitializeVisualCharge(isOwner);

            if (!thrown && ChargeProgress >= 1f)
                thrown = true;

            if (thrown && !stuckInGround && !stuckInTarget)
                Projectile.extraUpdates = 1;

            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.spriteDirection = Projectile.direction;
            Vector3 Light = Color.MediumOrchid.ToVector3();
            Lighting.AddLight(Projectile.Center, Light * 0.5f);

            if (Projectile.timeLeft == Lifetime && Main.myPlayer == Projectile.owner)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 toMouse = GetAimDirection();
                Projectile.velocity = toMouse * 14;
                Projectile.Center = Owner.MountedCenter + toMouse * 12f * Projectile.scale;
                Projectile.spriteDirection = Projectile.direction;
                thrown = true;
                time = 0;
                Projectile.extraUpdates = 1;
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if (thrown)
            {
                UpdateThrownMotion(0.35f, slowAfterExit: true, collideAfterExit: true, exitRotation: 1.5f);
                if (exitedTarget && !stuckInGround)
                {
                    if (Projectile.velocity.Y < 14 && Projectile.timeLeft < (bounces == 1 ? Lifetime - 20 : Lifetime))
                        Projectile.velocity.Y += 0.1f * (bounces * 3);
                }

                if (fadingOut)
                    FadeOutEffect();
            }
            if (!fadingOut && !stuckInTarget && !stuckInGround && ChargeProgress >= 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 safeVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 dustVel = exitedTarget ? safeVel.RotatedBy(MathHelper.ToRadians(-90 * Projectile.direction) + Projectile.rotation) : safeVel.RotatedBy(MathHelper.ToRadians(90 * (i == 0 ? 1 : -1))).RotatedByRandom(0.1f) * Main.rand.NextFloat(3, 4);
                    if (!exitedTarget && Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + safeVel * 60 * Projectile.scale, ModContent.DustType<SquashDust>(), dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.1f) * Projectile.scale);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                        dust.alpha = 50;
                        dust.fadeIn = Projectile.scale - 1;
                    }
                    if (exitedTarget && i == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + safeVel.RotatedBy(Projectile.rotation - MathHelper.ToRadians(Projectile.direction * 45)) * 45 * Projectile.scale, DustID.FireworksRGB, dustVel * Main.rand.NextFloat(1, 4), 0, default, Main.rand.NextFloat(0.4f, 0.6f));
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                    }
                }
                
                if (Main.rand.NextBool(4))
                {
                    Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.6f), "CalamityMod/Particles/DemonSigilParticle", false, 17, Main.rand.NextFloat(0.2f, 0.3f) * Projectile.scale, Color.Lerp(Color.MediumOrchid, Color.BlueViolet, Main.rand.NextFloat(0, 0.7f)) * 0.6f, new Vector2(1, 1), true, false, Main.rand.NextFloat(-1f, 1f), false, false);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }

            UpdateChargeAnimation(isOwner);
        }

        public void throwAnimation(float chargeProgress) => AnimateThrow(chargeProgress);

        protected override void OnImpaleEffects(NPC target, bool ejected)
        {
            if (ejected)
            {
                for (int i = 0; i < 10; i++)
                {
                    Particle spark2 = new SparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.2f, 1.5f), false, 45, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.3f, 1f), 0, default, Main.rand.NextFloat(1.3f, 1.6f));
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.4f;
            int hitsToMinMult = 8;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult * (exitedTarget ? 1.15f : 1f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => HandleTileCollision(oldVelocity, bounceSpread: 0f, syncOnImpale: true);
        public void ImpaleGround(Vector2 oldVelocity) => ImpaleGroundCore(oldVelocity, syncOnImpale: true);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => (Projectile.numHits > 0 && !exitedTarget) ? false : CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width / (exitedTarget ? 0.5f : 1) * Projectile.scale, targetHitbox); // After exiting a target, the hitbox is larger
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ForbiddenOathblade").Value;
            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition;

            float fadeScale = (1 - Projectile.Opacity);
            for (int i = 0; i < 16; i++)
            {
                Color auraColor = Color.MediumOrchid with { A = 0 } * 0.4f * fadeScale * Projectile.Opacity;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 9 * fadeScale;
                Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset, null, auraColor, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }

            Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.BlueViolet with { A = 0 }, lightColor, Projectile.Opacity) * Projectile.Opacity, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
