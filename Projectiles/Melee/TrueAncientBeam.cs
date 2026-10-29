using System;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Projectiles.Melee
{
    public class TrueAncientBeam : BaseEnergyNeedleProjectile //The boring plain one
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueArkoftheAncientsGlow";

        const float MaxTime = 75;
        public float Timer => MaxTime - Projectile.timeLeft;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < MaxTime - 5)
                Projectile.tileCollide = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.scale = 2.4f;
            Projectile.Opacity = 0.6f;
            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);

            Projectile.velocity *= (1 - (float)Math.Pow(Timer / MaxTime, 3));

            if (Main.rand.NextBool(3))
            {
                int dustTrail = Dust.NewDust(Projectile.Center, 14, 14, DustID.RainbowTorch, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f, 150, new Color(Main.DiscoR, 100, 255), 1.2f);
                Main.dust[dustTrail].noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(3);
                dustType = dustType == 0 ? 15 : dustType == 1 ? 57 : 58;

                Dust.NewDust(Projectile.Center, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1.3f);
            }



            if (Projectile.velocity.Length() < 1.0f)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 70)
                return false;

            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D starTexture = Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            Texture2D bloomTexture = Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            //Adjust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;

            Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.85f);
            float rotation = Main.GlobalTimeWrappedHourly * 8f;

            Vector2 sparkCenter = Projectile.Center - Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * 30.5f - Main.screenPosition;

            Main.EntitySpriteDraw(bloomTexture, sparkCenter, null, color * 0.5f, 0, bloomTexture.Size() / 2f, 4 * properBloomSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, color * 0.5f, rotation + MathHelper.PiOver4, starTexture.Size() / 2f, 2 * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, Color.White, rotation, starTexture.Size() / 2f, 2, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

    }
}
