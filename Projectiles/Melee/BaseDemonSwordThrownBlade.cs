using System;
using System.IO;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public abstract class BaseDemonSwordThrownBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        protected BaseDemonSwordThrownBlade(int lifetime, int fadeOutTime)
        {
            Lifetime = lifetime;
            this.fadeOutTime = fadeOutTime;
        }

        protected abstract float HeldBladeOffset { get; }
        protected abstract Color FadeDustSecondaryColor { get; }

        public int ChargeupTime => (int)Projectile.localAI[2];
        public int Lifetime;
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);
        public Player Owner => Main.player[Projectile.owner];

        // Real variables here
        public ref float time => ref Projectile.ai[0];
        public ref float stabOrder => ref Projectile.ai[1];
        public ref NPC stabbedTarget => ref Main.npc[(int)Projectile.ai[2]];

        public bool thrown = false;
        public int fadeOutTime;
        public bool stuckInTarget = false;
        public int stuckTimer = 0;
        public bool exitedTarget = false;
        public bool stuckInGround = false;
        public Vector2 impalePos;
        public int bounces = 0;
        public Vector2 tipPosition = Vector2.Zero;
        public bool fadingOut => Projectile.timeLeft <= (Lifetime - fadeOutTime);
        private bool visualChargeInitialized = false;
        private int visualChargeTimer = 0;
        private int visualChargeTime = 0;

        protected Vector2 GetAimDirection()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 toMouse = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                float aimAngle = toMouse.ToRotation();
                if (Math.Abs(MathHelper.WrapAngle(aimAngle - Projectile.localAI[1])) > 0.02f)
                {
                    Projectile.localAI[1] = aimAngle;
                    if (Projectile.timeLeft % 6 == 0)
                        Projectile.netUpdate = true;
                }
                return toMouse;
            }
            return Projectile.localAI[1].ToRotationVector2();
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return (ChargeProgress >= 1 && !stuckInGround && !stuckInTarget);
        }

        //Swing animation keys
        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement(float progress) => PiecewiseAnimation(progress, new CurveSegment[] { pullback, throwout });

        // Called at AI entry; the animation update stays after each blade's flight and visual logic.
        protected void InitializeVisualCharge(bool isOwner)
        {
            if (!visualChargeInitialized && !isOwner)
            {
                int fallbackCharge = (int)MathHelper.Clamp((Owner.HeldItem.useTime / 2.8f), 1, 100);
                visualChargeTime = (int)(Projectile.localAI[2] > 0f ? Projectile.localAI[2] : fallbackCharge);
                visualChargeTimer = visualChargeTime;
                visualChargeInitialized = true;
            }
        }

        protected void UpdateChargeAnimation(bool isOwner)
        {
            //Anticipation animation. Make the player look like they're holding the item
            float chargeProgress = ChargeProgress;
            bool canVisualCharge = !isOwner && Projectile.velocity.LengthSquared() < 0.01f && visualChargeTimer > 0;
            if (canVisualCharge)
            {
                chargeProgress = 1f - (visualChargeTimer / (float)visualChargeTime);
                visualChargeTimer--;
            }
            if (chargeProgress < 1f)
            {
                AnimateThrow(chargeProgress);
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

        protected void AnimateThrow(float chargeProgress)
        {
            Vector2 aimDirection = GetAimDirection();
            Owner.ChangeDir(MathF.Sign(aimDirection.X));

            float armRotation = ArmAnticipationMovement(chargeProgress) * Owner.direction;

            Owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Owner.direction;
            Projectile.direction = Owner.direction;

            Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * HeldBladeOffset * Owner.gravDir * Projectile.scale;
            Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);
        }

        public void FadeOutEffect()
        {
            Projectile.tileCollide = false;
            if (Projectile.timeLeft > Lifetime - fadeOutTime)
                Projectile.timeLeft = Lifetime - fadeOutTime;
            Projectile.Opacity = Utils.GetLerpValue(0, Lifetime - fadeOutTime, Projectile.timeLeft, true);

            Vector2 dustVel = new Vector2(10, 10).RotatedByRandom(Math.PI) * Main.rand.NextFloat(0.2f, 1f) * Projectile.Opacity;
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), dustVel, 0, default, Main.rand.NextFloat(1.1f, 1.4f) * Projectile.scale);
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.MediumOrchid : FadeDustSecondaryColor;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.alpha = 100;
                dust.velocity += Projectile.velocity;
                dust.fadeIn = Projectile.scale - 1;
            }
        }

        public override bool? CanDamage()
        {
            if (ChargeProgress < 1 || fadingOut || stuckInGround || stuckInTarget || (Projectile.numHits > 0 && !exitedTarget))
                return false;

            return base.CanDamage();
        }

        // Keep the shared state flags and payload order identical for both projectile types.
        public override void SendExtraAI(BinaryWriter writer)
        {
            byte state = 0;
            if (thrown)
                state |= 1;
            if (stuckInTarget)
                state |= 2;
            if (exitedTarget)
                state |= 4;
            if (stuckInGround)
                state |= 8;

            writer.Write(state);
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
            writer.Write(impalePos.X);
            writer.Write(impalePos.Y);
            writer.Write(stuckTimer);
            writer.Write((short)bounces);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            byte state = reader.ReadByte();
            thrown = (state & 1) != 0;
            stuckInTarget = (state & 2) != 0;
            exitedTarget = (state & 4) != 0;
            stuckInGround = (state & 8) != 0;

            Projectile.rotation = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
            impalePos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            stuckTimer = reader.ReadInt32();
            bounces = reader.ReadInt16();
        }
    }
}
