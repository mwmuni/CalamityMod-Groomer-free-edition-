using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.MainMenu
{
    public class CalamityMainMenu_Classic : ModMenu
    {
        public class Cinder
        {
            public int Time;
            public int Lifetime;
            public int IdentityIndex;
            public float Scale;
            public float Depth;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;

            public Cinder(int lifetime, int identity, float depth, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                IdentityIndex = identity;
                Depth = depth;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
            }
        }

        public static List<Cinder> Cinders
        {
            get;
            internal set;
        } = new();

        public float remixLogoRotation = 0f;
        public override string DisplayName => CalamityUtils.GetTextValue("UI.MainMenuClassic");

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("CalamityMod/MainMenu/Logo");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("CalamityMod/Backgrounds/BlankPixel");
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("CalamityMod/Backgrounds/BlankPixel");

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("CalamityTitle") ?? MusicID.Title;

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NullSurfaceBackground>();

        // Before drawing the logo, draw the entire Calamity background. This way, the typical parallax background is skipped entirely.
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/MainMenu/ClassicMenuBackground").Value;

            static Color selectCinderColor()
            {
                if (Main.rand.NextBool(3))
                    return Color.Lerp(Color.DarkGray, Color.LightGray, Main.rand.NextFloat());

                return Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.9f));
            }

            MainMenuRenderingHelper.DrawBackgroundAndCinders(
                spriteBatch,
                texture,
                Cinders,
                static (lifetime, identity, depth, color, position, velocity) => new Cinder(lifetime, identity, depth, color, position, velocity),
                static cinder => new MainMenuCinderState(cinder.Time, cinder.Lifetime, cinder.IdentityIndex, cinder.Scale, cinder.Depth, cinder.DrawColor, cinder.Velocity, cinder.Center),
                static (cinder, state) =>
                {
                    cinder.Time = state.Time;
                    cinder.Scale = state.Scale;
                    cinder.Velocity = state.Velocity;
                    cinder.Center = state.Center;
                },
                selectCinderColor);

            // Set the logo draw color to be white and the time to be noon
            // This is because there is not a day/night cycle in this menu, and changing colors would look bad
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            // Adjust rotation based on secret seeds; only Drunk world and Remix touch this, with GFB leeching off those two
            // Standard rotation is none; Drunk world makes it spin out, so it can use the vanilla rotation due to disappearing
            // Remix makes it flip upside down, and in GFB it will spin forever 
            if (WorldGen.remixWorldGen)
            {
                remixLogoRotation += MathHelper.Pi / 50f;
                if (remixLogoRotation >= MathHelper.Pi && !WorldGen.everythingWorldGen)
                    remixLogoRotation = MathHelper.Pi;
            }
            else
                remixLogoRotation = 0f;
            float rotationSecretSeedAdjusted = WorldGen.remixWorldGen ? remixLogoRotation : WorldGen.drunkWorldGen ? logoRotation : 0f;

            // Draw the logo using a different spritebatch blending setting so it doesn't have a horrible yellow glow
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(Logo.Value, drawPos, null, drawColor, rotationSecretSeedAdjusted, Logo.Value.Size() * 0.5f, WorldGen.drunkWorldGen ? logoScale : 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            return false;
        }
    }
}
