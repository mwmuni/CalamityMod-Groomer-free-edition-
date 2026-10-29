using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public abstract class GhastlySoulBase : ModProjectile
    {
        private float HomingBuff = 1;

        protected abstract int TimeLeft { get; }
        protected abstract float SoulVelocityMultiplier { get; }
        protected abstract float HomingDistance { get; }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = TimeLeft;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (HomingBuff > 0)
                HomingBuff -= 0.01f;

            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);

            if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft < TimeLeft - 10)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.5f, 1.5f), false, Main.rand.Next(9, 12 + 1), 0.4f, Color.Plum);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            float inertia = MathHelper.Lerp(20, 90, HomingBuff) * Projectile.ai[1];
            float velocity = VoidEdge.ShootSpeed * SoulVelocityMultiplier * Projectile.ai[1];
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                float homingDistance = HomingDistance;
                NPC target = Projectile.FindTargetWithinRange(homingDistance);
                if (Projectile.timeLeft < TimeLeft - VoidEdge.ProjectileSpreadOutTime && target != null)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, homingDistance, velocity, inertia);
                }
                else if (Projectile.Distance(Main.player[Projectile.owner].Center) > homingDistance)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * velocity) / inertia;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > TimeLeft - 5)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
    }
}
