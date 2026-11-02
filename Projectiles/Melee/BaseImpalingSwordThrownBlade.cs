using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public abstract class BaseImpalingSwordThrownBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        protected BaseImpalingSwordThrownBlade(int lifetime, int fadeOutTime)
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

        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement(float progress) => PiecewiseAnimation(progress, new CurveSegment[] { pullback, throwout });

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

        protected void PositionHeldBlade(float chargeProgress)
        {
            float armRotation = ArmAnticipationMovement(chargeProgress) * Owner.direction;

            Owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Owner.direction;
            Projectile.direction = Owner.direction;

            Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * HeldBladeOffset * Owner.gravDir * Projectile.scale;
            Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);
        }

        protected void UpdateImpaledTarget(bool collideAfterExit, float exitRotation)
        {
            if (stuckInTarget)
            {
                Projectile.tileCollide = false;
                time--;
                Projectile.timeLeft++;
                if (stuckTimer > 0)
                    stuckTimer--;
                else
                {
                    Projectile.velocity *= 0.01f;
                    stabbedTarget.Calamity().demonSwordImpales--;
                    stuckInTarget = false;
                    FadeOutEffect();
                }

                Projectile.Center = stabbedTarget.Center + impalePos;
                if ((stabbedTarget.life <= 0) || stabbedTarget == null || Projectile.localAI[0] == 5 || !stabbedTarget.active)
                {
                    Projectile.timeLeft = Lifetime;
                    stabbedTarget.Calamity().demonSwordImpales--;
                    stuckInTarget = false;
                    exitedTarget = true;
                    if (collideAfterExit)
                        Projectile.tileCollide = true;
                    Projectile.rotation += Main.rand.NextFloat(-exitRotation, exitRotation);
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;
                    Projectile.numHits = 0;
                    SoundStyle unstuck = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
                    SoundEngine.PlaySound(unstuck with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.3f, 0.4f), MaxInstances = 3 }, Projectile.Center);
                }
            }
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
    }
}
