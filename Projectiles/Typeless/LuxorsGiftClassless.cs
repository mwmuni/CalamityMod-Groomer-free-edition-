using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class LuxorsGiftClassless : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 6;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.35f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft < 294)
            { 
                Particle spark = new GlowSparkParticle(Projectile.Center - Projectile.velocity, -Projectile.velocity * 0.3f, false, 18, 0.03f * (1 + Utils.GetLerpValue(270, 300, Projectile.timeLeft, true)), Color.Gray * 0.45f, new Vector2(1.1f * (1 + Utils.GetLerpValue(270, 300, Projectile.timeLeft, true)), 1.3f), true, false, 0.3f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.1f;
            int hitsToMinMult = 7;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            for (int i = 0; i < Math.Max(6 - Projectile.numHits, 1); i++)
            {
                Particle spark = new GlowSparkParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f), false, 8, 0.02f, Color.White * 0.75f, new Vector2(0.8f, 1.3f), true, false, 1f);
                GeneralParticleHandler.SpawnParticle(spark);

                for (int y = 0; y < 2; y++)
                {
                    SparkParticle spark2 = new SparkParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f, 1.3f), false, 35, 0.5f, Color.White);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 20, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            Color drawColor = Color.Gray;
            float fade = Utils.GetLerpValue(0, 90, Projectile.timeLeft, true);


            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 2f;
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset, null, drawColor with { A = 0 } * 0.4f * (float)Math.Pow(fade, 3), Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            //CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
    }
}
