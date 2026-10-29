using CalamityMod.Tiles.Crags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public abstract class BaseAshenSlab : BaseRedTorchStoneDustTile
    {
        public Asset<Texture2D> GlowTexture;

        protected virtual bool ShouldMergeWithBrimstoneSlag => true;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            if (ShouldMergeWithBrimstoneSlag)
                CalamityUtils.SetMerge(Type, ModContent.TileType<BrimstoneSlag>());

            HitSound = SoundID.Tink;
            MineResist = 2f;
            MinPick = 100;
            AddMapEntry(new Color(40, 24, 48));
            AnimationFrameHeight = 90;
        }
        int animationFrameWidth = 234;

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = Main.tileFrame[Type] + i;
            int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
            int xPos = i % 2;
            int yPos = j % 3;
            int xPattern = i % 20 / 2;
            int yPattern = j % 30 / 3;
            int xOffset;
            switch (xPattern)
            {
                case 0:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 1;
                            break;
                        case 5:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 2;
                            break;
                        case 9:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 1:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 3:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 2:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        case 8:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 3:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 2;
                            break;
                        case 5:
                            xOffset = 2;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 4:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 5:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 1;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 6:
                    switch (yPattern)
                    {
                        case 5:
                            xOffset = 2;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        case 12:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 7:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 6:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 8:
                    switch (yPattern)
                    {
                        case 3:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 9:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                default:
                    xOffset = -1;
                    break;
            }
            if (yPos < 2)
            {
                if (xOffset != -1)
                {
                    if (j % 3 < 2)
                    {
                        uniqueAnimationFrameX = Main.tile[i - (i % 2), j - (j % 3)].TileFrameNumber;
                    }
                    if (uniqueAnimationFrameX != 0)
                    {
                        uniqueAnimationFrameX += xOffset;
                    }
                }
                else
                {
                    uniqueAnimationFrameX = 0;
                }
            }
            switch (yPos)
            {
                case 0:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 3;
                            break;
                    }
                    break;
                case 2:
                    uniqueAnimationFrameY = 4 + xPos;
                    uniqueAnimationFrameX = 0;
                    break;
                default:
                    uniqueAnimationFrameY = 0;
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
            frameYOffset = uniqueAnimationFrameY * AnimationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].IsTileActuallyInvisible())
                return;

            int frameXOffset = 0;
            int frameYOffset = 0;
            AnimateIndividualTile(Type, i, j, ref frameXOffset, ref frameYOffset);
            int xDrawPos = Main.tile[i, j].TileFrameX + frameXOffset;
            int yDrawPos = Main.tile[i, j].TileFrameY + frameYOffset;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;

            GlowTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureAshen/AshenSlabGlow");
            Texture2D glowmask = GlowTexture.Value;

            Color drawColour = CalamityUtils.ApplyPaint(Main.tile[i, j].TileColor, new Color(64, 64, 64, 64));
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xDrawPos, yDrawPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xDrawPos, yDrawPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }

    }

    public class AshenSlab : BaseAshenSlab
    {
    }
}
