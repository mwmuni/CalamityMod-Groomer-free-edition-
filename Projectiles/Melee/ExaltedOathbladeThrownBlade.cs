using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ExaltedOathbladeThrownBlade : BaseImpalingSwordThrownBlade
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ExaltedOathblade";
        protected override float HeldBladeOffset => -65f;
        protected override Color FadeDustSecondaryColor => Color.BlueViolet;
        protected override int DemonicFlamesDamage => 100;
        protected override int MaximumImpales => 4;

        public ExaltedOathbladeThrownBlade() : base(150, 120)
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            base.SetDefaults();
        }

        internal float ArmAnticipationMovement() => base.ArmAnticipationMovement(ChargeProgress);

        public override void AI()
        {
            if (time == 0)
                Projectile.scale = Owner.GetMeleeScale();
            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.spriteDirection = Projectile.direction;
            Vector3 Light = Color.MediumOrchid.ToVector3();
            Lighting.AddLight(Projectile.Center, Light * 0.85f);

            if (Projectile.timeLeft == Lifetime)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 toMouse = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                Projectile.velocity = toMouse * 14;
                Projectile.Center = Owner.MountedCenter + toMouse * 12f;
                Projectile.spriteDirection = Projectile.direction;
                thrown = true;
                time = 0;
                Projectile.extraUpdates = 2;
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if (thrown)
            {
                UpdateThrownMotion(0.35f, slowAfterExit: false, collideAfterExit: true, exitRotation: 1.5f);
                if (exitedTarget && !stuckInGround)
                {
                    Projectile.extraUpdates = 2;
                    if (Projectile.velocity.Y < 14 && Projectile.timeLeft < (bounces > 0 ? Lifetime : Lifetime - 50))
                        Projectile.velocity.Y += 0.35f;
                }

                if (fadingOut)
                    fadeOutEffect();
            }
            if (!fadingOut && !stuckInTarget && !stuckInGround && ChargeProgress >= 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 safeVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 dustVel = exitedTarget ? safeVel.RotatedBy(MathHelper.ToRadians(-90 * Projectile.direction) + Projectile.rotation) : safeVel.RotatedBy(MathHelper.ToRadians(70 * (i == 0 ? 1 : -1))) * 6;
                    if (!exitedTarget)
                    {
                        Particle spark = new VelChangingSpark(Projectile.Center + safeVel * 75 * Projectile.scale, -dustVel, -Projectile.velocity, "CalamityMod/Particles/BloomCircle", 9, 0.2f * Projectile.scale, (Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet) * 0.65f, new Vector2(1, 1f), true, false, 0, false, 1.0f, 0.2f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    if (exitedTarget && i == 0 && Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + safeVel.RotatedBy(Projectile.rotation - MathHelper.ToRadians(Projectile.direction * 45)) * 45 * Projectile.scale, ModContent.DustType<SquashDust>(), dustVel * Main.rand.NextFloat(1, 4), 0, default, Main.rand.NextFloat(1.2f, 1.4f) * Projectile.scale);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                        dust.fadeIn = Projectile.scale - 1;
                    }
                }
                
                if (Main.rand.NextBool(4))
                {
                    Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.6f), "CalamityMod/Particles/DemonSigilParticle", false, 17, Main.rand.NextFloat(0.2f, 0.3f) * Projectile.scale, Color.Lerp(Color.MediumOrchid, Color.BlueViolet, Main.rand.NextFloat(0, 0.7f)) * 0.6f, new Vector2(1, 1), true, false, Main.rand.NextFloat(-1f, 1f), false, false);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }

            //Anticipation animation. Make the player look like they're holding the item
            if (ChargeProgress < 1)
            {
                throwAnimation();
                if (ChargeProgress >= 0.6f && time == 0)
                {
                    SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordSwing", 2);
                    SoundEngine.PlaySound(swing with { Volume = 0.65f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                    time++;
                }
                return;
            }
            else
                time++;
        }
        public void fadeOutEffect() => FadeOutEffect();

        public void throwAnimation()
        {
            Owner.ChangeDir(MathF.Sign(Main.MouseWorld.X - Owner.Center.X));
            PositionHeldBlade(ChargeProgress);
        }

        protected override void OnImpaleEffects(NPC target, bool ejected)
        {
            if (ejected)
            {
                for (int i = 0; i < 12; i++)
                {
                    Particle spark2 = new SparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.4) * Main.rand.NextFloat(0.4f, 2.5f), false, 50, Main.rand.NextFloat(0.2f, 1.3f), Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.3) * Main.rand.NextFloat(0.5f, 2f), 0, default, Main.rand.NextFloat(1.3f, 1.6f));
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.45f;
            int hitsToMinMult = 9;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult * (exitedTarget ? 1.15f : 1f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => HandleTileCollision(oldVelocity, bounceSpread: 0.2f, syncOnImpale: false);
        public void impaleGround(Vector2 oldVelocity) => ImpaleGroundCore(oldVelocity, syncOnImpale: false);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => (Projectile.numHits > 0 && !exitedTarget) ? false : CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width / (exitedTarget ? 0.25f : 1) * Projectile.scale, targetHitbox); // After exiting a target, the hitbox is larger
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ExaltedOathblade").Value;
            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition;

            float fadeScale = (1 - Projectile.Opacity);
            for (int i = 0; i < 16; i++)
            {
                Color auraColor = Color.MediumOrchid with { A = 0 } * 0.4f * fadeScale * Projectile.Opacity;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 9 * fadeScale;
                Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset, null, auraColor, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (exitedTarget && !stuckInGround)
            {
                Asset<Texture2D> p = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearSmokey");
                Asset<Texture2D> p2 = ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmearSwipe");

                Main.EntitySpriteDraw(p2.Value, generalDrawPos, null, Color.BlueViolet with { A = 0 } * 0.45f, Projectile.rotation * Main.rand.NextFloat(1.6f, 1.7f), p2.Size() * 0.5f, 1.2f * Main.rand.NextFloat(0.8f, 1.15f) * Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(p.Value, generalDrawPos, null, Color.MediumOrchid with { A = 0 } * 0.65f, Projectile.rotation * Main.rand.NextFloat(1.2f, 1.3f), p.Size() * 0.5f, 0.95f * Projectile.scale, SpriteEffects.None);

            }
            Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.BlueViolet with { A = 0 }, lightColor, Projectile.Opacity) * Projectile.Opacity, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
