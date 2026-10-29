using System;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Projectiles.Melee
{
    public class RendingNeedle : BaseEnergyNeedleProjectile
    {
        protected override int TrailCacheLength => 10;

        const float MaxTime = 30;
        public float Timer => MaxTime - Projectile.timeLeft;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.scale = 2.4f;
            Projectile.Opacity = 0.6f;
            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.velocity *= (1 - (float)Math.Pow(Timer / MaxTime, 3) * 0.3f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * Projectile.scale * 0.6f, 0.28f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Main.hslToRgb(Main.rand.NextFloat(), 1, 0.7f), 20, Main.rand.NextFloat(0.4f, 0.7f) * Projectile.scale * 0.6f, 0.8f, 0, true, 0.05f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }

            if (Projectile.velocity.Length() < 1.0f)
                Projectile.Kill();
        }

        internal Color ColorFunction(float completionRatio, Vector2 vertexPos)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.GetLerpValue(1f, 0.64f, completionRatio, true) * Projectile.Opacity;

            Color endColor = Color.Lerp(Color.Cyan, Color.Crimson, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio, Vector2 vertexPos)
        {
            float expansionCompletion = (float)Math.Pow(1 - completionRatio, 2);
            return MathHelper.Lerp(0f, 14f * Projectile.scale, expansionCompletion);
        }

        public Vector2 OffsetFunction(float completionRatio, Vector2 vertexPos) => Projectile.Size * 0.5f - Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * 30.5f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 35)
                return false;

            Texture2D texture = Request<Texture2D>("CalamityMod/Projectiles/Melee/RendingNeedle").Value;

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, OffsetFunction, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D starTexture = Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            Texture2D bloomTexture = Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;

            Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.85f);
            float rotation = Main.GlobalTimeWrappedHourly * 8f;

            Vector2 sparkCenter = Projectile.Center - Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * 30.5f - Main.screenPosition;

            Main.EntitySpriteDraw(bloomTexture, sparkCenter, null, color* 0.5f, 0, bloomTexture.Size() / 2f, 4 * properBloomSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, color * 0.5f, rotation + MathHelper.PiOver4, starTexture.Size() / 2f, 2 * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, Color.White, rotation, starTexture.Size() / 2f, 2, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

    }
}
