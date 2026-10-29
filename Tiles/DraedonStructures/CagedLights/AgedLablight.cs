using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class AgedLablight : DustCountingLightFixtureTile<CagedLablightItem>
    {
        public Asset<Texture2D> GlowTexture;

        protected override int ItemDropType => ModContent.ItemType<AgedLablightItem>();
        protected override int FixtureDustType => DustID.KryptonMoss;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 31f / 510f;
            g = 251f / 510f;
            b = 255f / 510f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].IsTileActuallyInvisible())
                return;

            int xFrameOffset = Main.tile[i, j].TileFrameX;
            int yFrameOffset = Main.tile[i, j].TileFrameY;

            GlowTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonStructures/CagedLights/AgedLablight_Glow");
            Texture2D glowmask = GlowTexture.Value;

            Vector2 drawOffest = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + drawOffest;
            Color drawColour = Color.White * 0.6f;
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
                spriteBatch.Draw(glowmask, drawPosition, new Rectangle(xFrameOffset, yFrameOffset, 18, 18), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (trackTile.IsHalfBlock)
                spriteBatch.Draw(glowmask, drawPosition + new Vector2(0f, 8f), new Rectangle(xFrameOffset, yFrameOffset, 18, 8), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
    }
}
