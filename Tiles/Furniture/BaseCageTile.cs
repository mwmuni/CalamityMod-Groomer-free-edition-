using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public abstract class BaseCageTile<TItem> : ModTile where TItem : ModItem
    {
        public static Asset<Texture2D> topTexture;

        protected virtual string TopTexturePath => Texture + "_Top";
        protected abstract int AnimationFrameCount { get; }
        protected virtual int AnimationFrameTime => 6;

        protected virtual int GetAnimationFrameTime(int frame) => AnimationFrameTime;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                topTexture = ModContent.Request<Texture2D>(TopTexturePath);

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.addTile(Type);
            AnimationFrameHeight = 54;
            AddMapEntry(new Color(122, 217, 232), CalamityUtils.GetItemName<TItem>());
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Glass, 0f, 0f, 0, new Color(), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= GetAnimationFrameTime(frame))
            {
                frame++;
                frameCounter = 0;
            }
            if (frame >= AnimationFrameCount)
                frame = 0;
        }

        // The transparent cage top is a separate sprite and must be drawn over the tile's lower half.
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].IsTileActuallyInvisible())
                return false;

            Tile tile = Main.tile[i, j];
            var zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange, Main.offScreenRange);
            var drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero + CalamityUtils.TileDrawOffset + Vector2.UnitY * 2;
            var animateFrameOffset = Main.tileFrame[Type] * AnimationFrameHeight;
            var rect = new Rectangle(tile.TileFrameX, tile.TileFrameY + animateFrameOffset, 16, 16);
            var finalColor = CalamityUtils.ApplyPaint(tile.TileColor, Lighting.GetColor(i, j), false);

            if (rect.Y % 54 == 0)
            {
                Vector2 position = drawPos;
                position.Y += 8f;
                Rectangle drawRectangle = rect;
                drawRectangle.Y += 8;
                drawRectangle.Height -= 8;
                Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, position, drawRectangle, finalColor, 0f, zero, 1f, SpriteEffects.None, 0f);

                position = drawPos;
                position.Y -= 2f;
                drawRectangle = rect;
                drawRectangle.Y = 0;
                drawRectangle.Height = 10;
                spriteBatch.Draw(topTexture.Value, position, drawRectangle, finalColor, 0f, zero, 1f, SpriteEffects.None, 0f);
            }
            else
                spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawPos, rect, finalColor, 0f, zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
