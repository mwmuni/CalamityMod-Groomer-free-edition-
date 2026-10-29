using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuxorsGiftRogue : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Magenta.ToVector3() * 0.35f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.numHits == 0)
                Projectile.velocity.Y += 0.04f;
            else
                Projectile.velocity.Y += 0.06f;

            if (Projectile.velocity.Y > 0)
                Projectile.velocity.X *= 0.99f;

            if (Projectile.timeLeft > 60 && Projectile.timeLeft % 2 == 0)
            {
                Vector2 dustVel = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.05f) * Main.rand.NextFloat(0.1f, 0.3f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), dustVel);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.4f, 0.5f);
                dust.color = Color.Magenta;
                dust.noLightEmittence = true;
            }
            if (!Collision.SolidCollision(Projectile.Center, 35, 35) || Projectile.timeLeft <= 500)
                Projectile.tileCollide = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.4f;
            int hitsToMinMult = 3;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            Projectile.velocity = Utils.DirectionTo(target.Center, Projectile.Center).RotatedByRandom(0.2f) * 7;
            Projectile.velocity.X *= 0.25f;
            Projectile.velocity.Y *= 1.2f;

            for (int i = 0; i < 8; i++)
            {
                Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>());
                c.velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2().RotatedBy(MathHelper.PiOver4) * 4.5f * (i % 2 == 0 ? 0.78f : 1f);
                c.scale = Main.rand.NextFloat(0.6f, 0.7f) * (i % 2 == 0 ? 2.2f : 1.8f);
                c.noGravity = true;
                c.color = Color.Magenta;
                c.noLightEmittence = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.numHits == 0)
                Projectile.numHits = 1;
            Projectile.BounceOnTileCollision(oldVelocity);
            Projectile.velocity *= 0.92f;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 30, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            Color drawColor = Color.Magenta;
            float fade = Utils.GetLerpValue(0, 90, Projectile.timeLeft, true);

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 1f;
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset, null, drawColor with { A = 0 } * 0.4f * (float)Math.Pow(fade, 3), Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
