using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public abstract class BaseRubblemakerGlowEcho : ModTile
    {
        public Asset<Texture2D> GlowTexture;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.IsTileActuallyInvisible())
                return;

            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
            Texture2D tex = GlowTexture.Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}
