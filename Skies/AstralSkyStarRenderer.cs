using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalamityMod.Skies
{
    internal static class AstralSkyStarRenderer
    {
        internal static void Draw(SpriteBatch spriteBatch, float opacity)
        {
            int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
            float colorMult = 0.952f * opacity;
            Color astralcyan = new Color(100, 183, 255);
            Color purple = new Color(201, 148, 255);
            Color yellow = new Color(255, 146, 73);
            float width1 = Main.screenWidth / 500f;
            float height1 = Main.screenHeight / 600f;
            float width2 = Main.screenWidth / 600f;
            float height2 = Main.screenHeight / 800f;
            float width3 = Main.screenWidth / 200f;
            float height3 = Main.screenHeight / 900f;
            float width4 = Main.screenWidth / 1000f;
            float height4 = Main.screenHeight / 200f;
            for (int i = 0; i < Main.star.Length; i++)
            {
                Star star = Main.star[i];
                if (star == null)
                    continue;

                Texture2D t2D = TextureAssets.Star[star.type].Value;
                Vector2 origin = new Vector2(t2D.Width * 0.5f, t2D.Height * 0.5f);
                float posX = star.position.X * width1;
                float posY = star.position.Y * height1;
                Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);
                spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), astralcyan * star.twinkle * colorMult, star.rotation, origin, (star.scale * star.twinkle) - 0.2f, SpriteEffects.None, 0f);

                origin = new Vector2(t2D.Width * 0.2f, t2D.Height * 0.2f);
                posX = star.position.X * width2;
                posY = star.position.Y * height2;
                position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);
                spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), purple * star.twinkle * colorMult, star.rotation, origin, (star.scale * star.twinkle) + 0.2f, SpriteEffects.None, 0f);

                origin = new Vector2(t2D.Width * 0.8f, t2D.Height * 0.8f);
                posX = star.position.X * width3;
                posY = star.position.Y * height3;
                position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);
                spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), yellow * star.twinkle * colorMult, star.rotation, origin, star.scale * star.twinkle, SpriteEffects.None, 0f);

                origin = new Vector2(t2D.Width * 0.5f, t2D.Height * 0.5f);
                posX = star.position.X * width4;
                posY = star.position.Y * height4;
                position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);
                spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), Color.White * star.twinkle * colorMult, star.rotation, origin, star.scale * star.twinkle, SpriteEffects.None, 0f);
            }
        }
    }
}
