using System;
using CalamityMod.Skies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityMod.Backgrounds
{
    public abstract class AstralSurfaceBackgroundStyleBase : SurfaceBackgroundStyleBase
    {
        private protected readonly struct BackgroundLayer
        {
            public BackgroundLayer(
                Func<Texture2D> textureGetter,
                int yOffset,
                float scale,
                double parallax,
                double topPositionMultiplier,
                double topPositionOffset,
                float drawDepth,
                int menuTopY,
                int menuOffsetX = 0)
            {
                TextureGetter = textureGetter;
                GlowTextureGetter = textureGetter;
                GlowIntensity = 0f;
                YOffset = yOffset;
                Scale = scale;
                Parallax = parallax;
                TopPositionMultiplier = topPositionMultiplier;
                TopPositionOffset = topPositionOffset;
                DrawDepth = drawDepth;
                MenuTopY = menuTopY;
                MenuOffsetX = menuOffsetX;
            }

            public BackgroundLayer(
                Func<Texture2D> textureGetter,
                Func<Texture2D> glowTextureGetter,
                float glowIntensity,
                int yOffset,
                float scale,
                double parallax,
                double topPositionMultiplier,
                double topPositionOffset,
                float drawDepth,
                int menuTopY,
                int menuOffsetX = 0)
            {
                TextureGetter = textureGetter;
                GlowTextureGetter = glowTextureGetter;
                GlowIntensity = glowIntensity;
                YOffset = yOffset;
                Scale = scale;
                Parallax = parallax;
                TopPositionMultiplier = topPositionMultiplier;
                TopPositionOffset = topPositionOffset;
                DrawDepth = drawDepth;
                MenuTopY = menuTopY;
                MenuOffsetX = menuOffsetX;
            }

            public Func<Texture2D> TextureGetter { get; }
            public Func<Texture2D> GlowTextureGetter { get; }
            public float GlowIntensity { get; }
            public int YOffset { get; }
            public float Scale { get; }
            public double Parallax { get; }
            public double TopPositionMultiplier { get; }
            public double TopPositionOffset { get; }
            public float DrawDepth { get; }
            public int MenuTopY { get; }
            public int MenuOffsetX { get; }
        }

        private protected abstract BackgroundLayer[] BackgroundLayers { get; }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            float screenOff = Main.instance.screenOff;
            float scAdj = Main.instance.scAdj;
            Color surfaceBackgroundAlpha = Main.ColorOfSurfaceBackgroundsModified;
            Color surfaceBackgroundColor = new Color(63, 51, 90, surfaceBackgroundAlpha.A);
            bool canBGDraw = false;
            if ((!Main.remixWorld || (Main.gameMenu && !WorldGen.remixWorldGen)) && (!WorldGen.remixWorldGen || !WorldGen.drunkWorldGen))
            {
                canBGDraw = true;
            }
            if (Main.mapFullscreen)
            {
                canBGDraw = false;
            }

            int offset = 30;
            if (Main.gameMenu)
            {
                offset = 0;
            }
            if (WorldGen.drunkWorldGen)
            {
                offset = -180;
            }

            float surfacePosition = (float)Main.worldSurface;
            if (surfacePosition == 0f)
            {
                surfacePosition = 1f;
            }

            float screenPosition = Main.screenPosition.Y + (float)(Main.screenHeight / 2) - 600f;
            double backgroundTopMagicNumber = (0f - screenPosition + screenOff / 2f) / (surfacePosition * 16f);
            float bgGlobalScaleMultiplier = 2f;
            int offset2 = -180;
            int menuOffset = 0;
            if (Main.gameMenu)
            {
                menuOffset -= offset2;
            }
            int pushBGTopHack = menuOffset;
            pushBGTopHack += offset;
            pushBGTopHack += offset2;

            if (canBGDraw)
            {
                foreach (BackgroundLayer layer in BackgroundLayers)
                {
                    DrawLayer(layer, backgroundTopMagicNumber, scAdj, pushBGTopHack, bgGlobalScaleMultiplier, surfaceBackgroundColor, surfaceBackgroundAlpha.A);
                }
            }

            return false;
        }

        private static void DrawLayer(
            BackgroundLayer layer,
            double backgroundTopMagicNumber,
            float scAdj,
            int pushBGTopHack,
            float bgGlobalScaleMultiplier,
            Color surfaceBackgroundColor,
            byte surfaceBackgroundAlpha)
        {
            Texture2D texture = layer.TextureGetter();
            float bgScale = layer.Scale * bgGlobalScaleMultiplier;
            double bgParallax = layer.Parallax;
            int bgTopY = (int)(backgroundTopMagicNumber * layer.TopPositionMultiplier + layer.TopPositionOffset) + (int)scAdj + pushBGTopHack;
            int bgWidthScaled = (int)((float)texture.Width * bgScale);
            SkyManager.Instance.DrawToDepth(Main.spriteBatch, layer.DrawDepth / (float)bgParallax);
            int bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));
            if (Main.gameMenu)
            {
                bgTopY = layer.MenuTopY + pushBGTopHack;
                bgStartX -= layer.MenuOffsetX;
            }

            int bgLoops = Main.screenWidth / bgWidthScaled + 2;
            if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
            {
                for (int i = 0; i < bgLoops; i++)
                {
                    Vector2 drawPosition = new Vector2(bgStartX + bgWidthScaled * i, bgTopY + layer.YOffset);
                    Main.spriteBatch.Draw(texture, drawPosition, new Rectangle(0, 0, texture.Width, texture.Height), surfaceBackgroundColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    if (layer.GlowIntensity > 0f)
                    {
                        Texture2D glowTexture = layer.GlowTextureGetter();
                        Color glowColor = new Color(Color.White.R * layer.GlowIntensity, Color.White.G * layer.GlowIntensity, Color.White.B * layer.GlowIntensity, surfaceBackgroundAlpha);
                        Main.spriteBatch.Draw(glowTexture, drawPosition, new Rectangle(0, 0, glowTexture.Width, glowTexture.Height), glowColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
