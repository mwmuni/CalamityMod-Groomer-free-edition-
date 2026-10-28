using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientChandelier : BaseRedTorchStoneDustTile
    {
        public Asset<Texture2D> FlameTexture;

        public override void Load() => FlameTexture = ModContent.Request<Texture2D>(Texture + "Flame");

        public override void SetStaticDefaults() => this.SetUpChandelier(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientChandelier>(), true);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 3, 3);
        }

        public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
        {
            ulong flameSeed = Main.TileFrameSeed ^ (ulong)(((long)i << 32) | (uint)j);
            tileFlameData.flameSeed = flameSeed;
            tileFlameData.flameTexture = FlameTexture.Value;
            tileFlameData.flameColor = new Color(128, 64, 64, 0);
            tileFlameData.flameCount = 3;
            tileFlameData.flameRangeXMin = -10;
            tileFlameData.flameRangeXMax = 11;
            tileFlameData.flameRangeYMin = -10;
            tileFlameData.flameRangeYMax = 11;
            tileFlameData.flameRangeMultX = 0.1f;
            tileFlameData.flameRangeMultY = 0f;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 54 && tile.TileFrameY == 36)
            {
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
            }
        }
    }
}
