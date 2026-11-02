using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    // Keep aim synchronization and its packet payload separate from Exalted's legacy aim handling.
    public abstract class BaseDemonSwordThrownBlade : BaseImpalingSwordThrownBlade
    {
        protected BaseDemonSwordThrownBlade(int lifetime, int fadeOutTime) : base(lifetime, fadeOutTime)
        {
        }

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
            PositionHeldBlade(chargeProgress);
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
