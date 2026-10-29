using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public abstract class BasePumpkaboomProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        protected abstract int ProjectileWidth { get; }
        protected abstract int ProjectileHeight { get; }
        protected abstract float MidAirRotationSpeed { get; }
        protected abstract string StuckTickSoundPath { get; }
        protected abstract float CollisionRadius { get; }
        protected abstract float GlowPulseSpeed { get; }
        protected abstract int GlowCopyCount { get; }

        public float time = 0;
        public ref float stuckNPC => ref Projectile.ai[2];
        public ref float stuckState => ref Projectile.ai[1];
        public ref float flungState => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public int tileHits = 0;
        public float charge = 0;
        public bool hasReachedFullCharge = false;
        public bool hasStoppedHolding = false;
        public float midAirRot = 0;
        public int initialCastDirection = 0;

        private Vector2 placementDistance;
        private Vector2 placementVelocity;

        private bool beginStretchAnim = false;
        private float progress = -1;

        public Color mainColor = Color.White;
        public Color c1 = new Color(255, 117, 24);
        public Color c2 = new Color(168, 47, 57);
        private bool hasDealtDamage = false;

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool ShouldUpdatePosition() => flungState != 0f && stuckState == 0f;
        public override bool? CanDamage() => flungState != 0f && tileHits == 0 ? null : false;

        public override bool? CanHitNPC(NPC target)
        {
            if (hasDealtDamage)
                return false;

            return null;
        }

        public override void AI()
        {
            float rate = Main.GlobalTimeWrappedHourly * 6;
            List<Color> eColors = new List<Color>() { c1, c2 };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            mainColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            // Progress through squash n stretch anim on hit.
            if (progress >= 0f && progress < 1f)
                progress += 0.08f;

            if (stuckState == 1f)
            {
                if (stuckNPC >= 0 && Main.npc[(int)stuckNPC].active)
                    Projectile.Center = Main.npc[(int)stuckNPC].Center + placementVelocity * placementDistance;
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.Kill();

                if (Projectile.timeLeft == 109)
                    SoundEngine.PlaySound(new SoundStyle(StuckTickSoundPath) with { Pitch = 0f, Volume = 1f, MaxInstances = 4 }, Projectile.Center);

                if (Projectile.timeLeft < 110)
                    UpdateStuckEffects();

                return;
            }

            if (flungState != 0f)
            {
                if (initialCastDirection == 0)
                    initialCastDirection = Owner.direction;

                // stuckNPC index
                Projectile.localAI[0]++;
                Projectile.localAI[1] = 5; // The item can be used again once flung.

                Projectile.velocity.Y += 0.22f;

                if (stuckState == 0f)
                    midAirRot += MidAirRotationSpeed * initialCastDirection;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + midAirRot;
            }
            else
            {
                Projectile.velocity = Owner.velocity;
                int useAnim = Owner.HeldItem.useAnimation > 0 ? Owner.HeldItem.useAnimation : 30;
                float completion = time / (useAnim * 0.7f);

                if (Main.myPlayer == Projectile.owner)
                {
                    if (completion >= 1)
                    {
                        time = -1;
                        Projectile.Center = Owner.Center;
                        Vector2 velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                        Projectile.velocity = velocity * 16;
                        Projectile.tileCollide = true;
                        flungState = 1f;
                        Projectile.netUpdate = true;
                    }

                    Owner.direction = Math.Sign(Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).X);
                }

                float grenadeRot = 0;
                if (completion <= 0.75f)
                {
                    float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0f, 0.75f, completion, true), 2);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp) * Owner.direction);
                }
                else
                {
                    float releaseLerp = (float)Math.Pow(Utils.GetLerpValue(0.75f, 1f, completion, true), 3);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(-75f, 30f, releaseLerp) * Owner.direction);
                }

                Vector2 mouseDir = Main.myPlayer == Projectile.owner
                    ? Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld)
                    : Projectile.velocity.SafeNormalize(Vector2.UnitX);
                grenadeRot += mouseDir.ToRotation();
                Vector2 grenadePos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, grenadeRot) +
                    new Vector2(Owner.direction == 1 ? 5 : -3, Owner.direction == 1 ? -24 : -4).RotatedBy(grenadeRot);
                float completionLerp2 = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                float grenadeHalfRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp2) * Owner.direction);

                Projectile.Center = grenadePos;
                Projectile.rotation = grenadeRot - MathHelper.ToRadians(25 * grenadeHalfRot) +
                    (Owner.direction == 1 ? MathHelper.ToRadians(180) : 0);

                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, mouseDir.ToRotation() - MathHelper.ToRadians(90));
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, grenadeRot -
                    (Owner.direction == 1 ? MathHelper.ToRadians(180) : MathHelper.ToRadians(0)));
            }

            time++;
        }

        protected virtual void UpdateStuckEffects()
        {
        }

        protected void StickToTarget(NPC target)
        {
            stuckState = 1f;
            stuckNPC = target.whoAmI;
            placementDistance = new(-Vector2.Distance(target.Center, Projectile.Center));
            placementVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.oldVelocity.ToRotation() + MathHelper.PiOver2 + midAirRot;
            flungState = Projectile.rotation;

            hasDealtDamage = true;
            Projectile.localAI[1] = 5;
            Projectile.timeLeft = 110;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            stuckState = 1f;
            stuckNPC = target.whoAmI;
            Projectile.localAI[1] = 5;
            beginStretchAnim = true;
            Projectile.netUpdate = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            CalamityUtils.CircularHitboxCollision(Projectile.Center, CollisionRadius, targetHitbox);

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beginStretchAnim)
            {
                progress = 0f;
                beginStretchAnim = false;
            }

            if (progress >= 0f)
            {
                float stretchFactorX, stretchFactorY;

                if (progress < 0.5f)
                {
                    float completion = progress / 0.5f;
                    stretchFactorX = MathHelper.Lerp(1.7f, 0.7f, completion);
                    stretchFactorY = MathHelper.Lerp(0.7f, 1.7f, completion);
                }
                else
                {
                    float completion = (progress - 0.5f) / 0.5f;
                    stretchFactorX = MathHelper.Lerp(0.7f, 1f, completion);
                    stretchFactorY = MathHelper.Lerp(1.7f, 1f, completion);
                }

                if (progress >= 1f)
                {
                    stretchFactorX = 1f;
                    stretchFactorY = 1f;
                    progress = -1f;
                }

                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                Vector2 finalSyringeScale = new Vector2(stretchFactorX, stretchFactorY);
                Vector2 finalPos = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.Draw(texture, finalPos, null, lightColor, Projectile.rotation, origin, finalSyringeScale, SpriteEffects.None, 0f);

                return false;
            }

            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = mainTexture.Size() / 2f;
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            Vector2 drawPosition;

            if (stuckState == 1f && Projectile.timeLeft <= 110)
            {
                float glowSine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * GlowPulseSpeed);
                float pulse = MathHelper.Lerp(0.7f, 1f, glowSine);
                float lifeFadeIn = Utils.GetLerpValue(110, 0, Projectile.timeLeft, true);
                float finalGlowIntensity = pulse * lifeFadeIn;
                drawPosition = Projectile.Center - Main.screenPosition;

                for (int i = 0; i < GlowCopyCount; i++)
                {
                    float rotationOffset = MathHelper.TwoPi * i / 15;
                    Vector2 glowOffset = rotationOffset.ToRotationVector2() * (3f + glowSine * 1f) * finalGlowIntensity;

                    Main.spriteBatch.Draw(mainTexture, drawPosition + glowOffset, null, mainColor with { A = 0 } * finalGlowIntensity * 0.4f, flungState, drawOrigin, drawScale, spriteEffects, 0f);
                }
            }

            if (flungState == 0f)
            {
                spriteEffects = SpriteEffects.FlipVertically;

                int useAnim = Owner.HeldItem.useAnimation > 0 ? Owner.HeldItem.useAnimation : 30;
                float completion = time / (useAnim * 0.7f);
                float grenadeRot;

                if (completion <= 0.75f)
                {
                    float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0f, 0.75f, completion, true), 2);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp) * Owner.direction);
                }
                else
                {
                    float releaseLerp = (float)Math.Pow(Utils.GetLerpValue(0.75f, 1f, completion, true), 3);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(-75f, 30f, releaseLerp) * Owner.direction);
                }

                Vector2 mouseDir = Main.myPlayer == Projectile.owner
                    ? Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld)
                    : Projectile.velocity.SafeNormalize(Vector2.UnitX);
                grenadeRot += mouseDir.ToRotation();

                Vector2 grenadePos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, grenadeRot) +
                    new Vector2(Owner.direction == 1 ? 5 : 5, Owner.direction == 1 ? -38 : 22).RotatedBy(grenadeRot);

                drawPosition = grenadePos - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                drawRotation = grenadeRot + (Owner.direction == 1 ? MathHelper.ToRadians(180) : 0);
            }
            else
            {
                if (stuckState == 1f)
                    drawRotation = flungState;
                drawPosition = Projectile.Center - Main.screenPosition;
            }

            Main.spriteBatch.Draw(mainTexture, drawPosition, null, lightColor * (1f - Projectile.alpha / 255f), drawRotation, drawOrigin, drawScale, spriteEffects, 0f);

            return false;
        }
    }
}
