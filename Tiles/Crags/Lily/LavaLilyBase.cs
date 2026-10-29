using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Crags.Lily
{
    public abstract class LavaLilyBase : ModTile
    {
        private static readonly string[] GlowTexturePaths =
        {
            "CalamityMod/Tiles/Crags/Lily/LavaLily1Glow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily2Glow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily3Glow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily4Glow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily5Glow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily6Glow"
        };

        private static readonly string[] TopTexturePaths =
        {
            "CalamityMod/Tiles/Crags/Lily/LavaLily1Top",
            "CalamityMod/Tiles/Crags/Lily/LavaLily2Top",
            "CalamityMod/Tiles/Crags/Lily/LavaLily3Top",
            "CalamityMod/Tiles/Crags/Lily/LavaLily4Top",
            "CalamityMod/Tiles/Crags/Lily/LavaLily5Top",
            "CalamityMod/Tiles/Crags/Lily/LavaLily6Top"
        };

        private static readonly string[] TopGlowTexturePaths =
        {
            "CalamityMod/Tiles/Crags/Lily/LavaLily1TopGlow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily2TopGlow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily3TopGlow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily4TopGlow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily5TopGlow",
            "CalamityMod/Tiles/Crags/Lily/LavaLily6TopGlow"
        };

        public Asset<Texture2D> GlowTexture;
        public Asset<Texture2D> TopTexture;
        public Asset<Texture2D> TopGlowTexture;

        protected abstract int LilyNumber { get; }
        protected abstract Rectangle TopSource { get; }
        protected abstract Vector2 TopOrigin { get; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileAxe[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(3, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 3;
            TileObjectData.addTile(Type);
            MineResist = 3f;
            AddMapEntry(new Color(153, 100, 176));
            DustType = DustID.PurpleMoss;
            HitSound = SoundID.Grass;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = WorldGen.genRand.NextBool(2) ? DustID.PurpleMoss : DustID.YellowStarDust;
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 10;
        }

        internal static void DrawLilyTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //draw the glowmask on the lily base
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.IsTileActuallyInvisible())
                return;

            int textureIndex = LilyNumber - 1;
            GlowTexture ??= ModContent.Request<Texture2D>(GlowTexturePaths[textureIndex]);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);

            //draw in the middle of the tile so it doesnt draw more than once
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 && Framing.GetTileSafely(i, j).TileFrameY == 18)
            {
                TopTexture ??= ModContent.Request<Texture2D>(TopTexturePaths[textureIndex]);
                TopGlowTexture ??= ModContent.Request<Texture2D>(TopGlowTexturePaths[textureIndex]);

                DrawLilyTop(i, j, TopTexture.Value, TopSource, TileOffset.ToWorldCoordinates(), TopOrigin, false);
                DrawLilyTop(i, j, TopGlowTexture.Value, TopSource, TileOffset.ToWorldCoordinates(), TopOrigin, true);
            }
        }
    }
}
