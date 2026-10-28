using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureWulfrum
{
    public class WulfrumChandelier : BaseBlueWhiteFrameLightTile
    {
        public Asset<Texture2D> FlameTexture;
        public Asset<Texture2D> GlowTexture;

        public override void Load()
        {
            FlameTexture = ModContent.Request<Texture2D>(Texture + "Flame");
            GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.MultiTileSway[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(235, 166, 135), Language.GetText("MapObject.Chandelier"));
            AdjTiles = new int[] { TileID.Chandeliers };
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.TerraBlade, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor) => glowTexture = GlowTexture.Value;

        public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
        {
            ulong flameSeed = Main.TileFrameSeed ^ (ulong)(((long)i << 32) | (uint)j);
            tileFlameData.flameSeed = flameSeed;
            tileFlameData.flameTexture = FlameTexture.Value;
            tileFlameData.flameColor = new Color(102, 115, 128, 0);
            tileFlameData.flameCount = 3;
            tileFlameData.flameRangeXMin = -10;
            tileFlameData.flameRangeXMax = 11;
            tileFlameData.flameRangeYMin = -10;
            tileFlameData.flameRangeYMax = 11;
            tileFlameData.flameRangeMultX = 0.1f;
            tileFlameData.flameRangeMultY = 0.1f;
        }

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 3, 2);
        }
    }
}
