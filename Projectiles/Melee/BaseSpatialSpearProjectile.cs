using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public abstract class BaseSpatialSpearProjectile : ModProjectile
    {
        protected abstract int Lifetime { get; }
        protected abstract int? Penetration { get; }
        protected abstract Vector3 LightColor { get; }
        protected abstract Color AlphaColor { get; }
        protected abstract int DustType { get; }
        protected virtual int SplitCount => 0;
        protected virtual int SplitAngleDegrees => 0;
        protected virtual int NextProjectileType => 0;
        protected virtual bool PlayOnKillSound => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            if (Penetration is int penetration)
                Projectile.penetrate = penetration;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = !Main.zenithWorld;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, LightColor.X, LightColor.Y, LightColor.Z);

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver4;

            if (Projectile.timeLeft < (Lifetime / 2))
            {
                Projectile.velocity.Y += 0.16f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;

                Projectile.velocity.X *= 0.995f;
            }

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 125)
                {
                    Projectile.alpha = 130;
                    Projectile.localAI[1] = 1f;
                }
            }
            else if (Projectile.localAI[1] == 1f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[1] = 0f;
                }
            }

            int dust = Dust.NewDust(Projectile.oldPosition + Projectile.oldVelocity, Projectile.width, Projectile.height, DustType, 0f, 0f, 100, default, 1.25f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noLightEmittence = true;

            if (SplitCount > 0)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= Lifetime / SplitCount)
                {
                    Projectile.localAI[0] = 0f;
                    int numProj = 2;
                    float rotation = MathHelper.ToRadians(SplitAngleDegrees);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, NextProjectileType, (int)(Projectile.damage * 0.8), Projectile.knockBack * 0.5f, Projectile.owner);
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => AlphaColor;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > Lifetime - 5)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
        }

        public override void OnKill(int timeLeft)
        {
            if (PlayOnKillSound)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            for (int i = 4; i < 31; i++)
            {
                float projOldX = Projectile.oldVelocity.X * (30f / i);
                float projOldY = Projectile.oldVelocity.Y * (30f / i);
                int dust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - projOldX, Projectile.oldPosition.Y - projOldY), 8, 8, DustType, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLightEmittence = true;

                dust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - projOldX, Projectile.oldPosition.Y - projOldY), 8, 8, DustType, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].noLightEmittence = true;
            }
        }
    }
}
