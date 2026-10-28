using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.MainMenu
{
    internal static class MainMenuRenderingHelper
    {
        internal static void DrawBackgroundAndCinders<TCinder>(
            SpriteBatch spriteBatch,
            Texture2D backgroundTexture,
            List<TCinder> cinders,
            Func<int, int, float, Color, Vector2, Vector2, TCinder> createCinder,
            Func<TCinder, MainMenuCinderState> getState,
            Action<TCinder, MainMenuCinderState> setState,
            Func<Color> selectCinderColor)
        {
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / backgroundTexture.Width;
            float yScale = (float)Main.screenHeight / backgroundTexture.Height;
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (backgroundTexture.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                    drawOffset.Y -= (backgroundTexture.Height * scale - Main.screenHeight) * 0.5f;
            }

            spriteBatch.Draw(backgroundTexture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            for (int i = 0; i < 5; i++)
            {
                if (Main.rand.NextBool(4))
                {
                    int lifetime = Main.rand.Next(200, 300);
                    float depth = Main.rand.NextFloat(1.8f, 5f);
                    Vector2 startingPosition = new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                    Vector2 startingVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f)) * 4f;
                    Color cinderColor = selectCinderColor();
                    cinders.Add(createCinder(lifetime, cinders.Count, depth, cinderColor, startingPosition, startingVelocity));
                }
            }

            for (int i = 0; i < cinders.Count; i++)
            {
                MainMenuCinderState state = getState(cinders[i]);
                state.Scale = Utils.GetLerpValue(state.Lifetime, state.Lifetime / 3, state.Time, true);
                state.Scale *= MathHelper.Lerp(0.6f, 0.9f, state.IdentityIndex % 6f / 6f);
                if (state.IdentityIndex % 13 == 12)
                    state.Scale *= 2f;

                float flySpeed = MathHelper.Lerp(3.2f, 14f, state.IdentityIndex % 21f / 21f);
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.44f, 0.44f, (float)Math.Sin(state.Time / 16f + state.IdentityIndex) * 0.5f + 0.5f));
                idealVelocity = (idealVelocity + Vector2.UnitX).SafeNormalize(Vector2.UnitY) * flySpeed;

                float movementInterpolant = MathHelper.Lerp(0.01f, 0.08f, Utils.GetLerpValue(45f, 145f, state.Time, true));
                state.Velocity = Vector2.Lerp(state.Velocity, idealVelocity, movementInterpolant);

                state.Time++;
                state.Center += state.Velocity;
                setState(cinders[i], state);
            }

            cinders.RemoveAll(cinder =>
            {
                MainMenuCinderState state = getState(cinder);
                return state.Time >= state.Lifetime;
            });

            Texture2D cinderTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/CalamitasCinder").Value;
            for (int i = 0; i < cinders.Count; i++)
            {
                MainMenuCinderState state = getState(cinders[i]);
                Vector2 drawPosition = state.Center;
                spriteBatch.Draw(cinderTexture, drawPosition, null, state.DrawColor, 0f, cinderTexture.Size() * 0.5f, state.Scale, 0, 0f);
            }
        }
    }

    internal struct MainMenuCinderState
    {
        internal int Time;
        internal int Lifetime;
        internal int IdentityIndex;
        internal float Scale;
        internal float Depth;
        internal Color DrawColor;
        internal Vector2 Velocity;
        internal Vector2 Center;

        internal MainMenuCinderState(int time, int lifetime, int identityIndex, float scale, float depth, Color drawColor, Vector2 velocity, Vector2 center)
        {
            Time = time;
            Lifetime = lifetime;
            IdentityIndex = identityIndex;
            Scale = scale;
            Depth = depth;
            DrawColor = drawColor;
            Velocity = velocity;
            Center = center;
        }
    }
}
