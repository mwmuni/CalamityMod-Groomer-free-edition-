using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenLantern : BaseRedTorchStoneDustTile
    {
        public Asset<Texture2D> FlameTexture;

        public override void Load() => FlameTexture = ModContent.Request<Texture2D>(Texture + "Flame");

        public override void SetStaticDefaults() => this.SetUpLantern(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenLantern>(), true);
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 1f;
                g = 0.5f;
                b = 0.5f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
        {
            ulong flameSeed = Main.TileFrameSeed ^ (ulong)(((long)i << 32) | (uint)j);
            tileFlameData.flameSeed = flameSeed;
            tileFlameData.flameTexture = FlameTexture.Value;
            tileFlameData.flameColor = new Color(128, 64, 64, 0);
            tileFlameData.flameCount = 2;
            tileFlameData.flameRangeXMin = -10;
            tileFlameData.flameRangeXMax = 11;
            tileFlameData.flameRangeYMin = -10;
            tileFlameData.flameRangeYMax = 11;
            tileFlameData.flameRangeMultX = 0f;
            tileFlameData.flameRangeMultY = 0f;
        }

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 1, 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 18 && tile.TileFrameX < 18)
            {
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
            }
        }
    }
}
