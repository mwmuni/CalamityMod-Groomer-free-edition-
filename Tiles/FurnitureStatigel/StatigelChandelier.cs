using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStatigel
{
    public class StatigelChandelier : BaseBlueWhiteFrameLightTile
    {
        public Asset<Texture2D> FlameTexture;

        public override void Load() => FlameTexture = ModContent.Request<Texture2D>(Texture + "Flame");

        public override void SetStaticDefaults() => this.SetUpChandelier(ModContent.ItemType<Items.Placeables.FurnitureStatigel.StatigelChandelier>());

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.PinkSlime, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

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
            FurnitureCommon.LightHitWire(Type, i, j, 3, 3);
        }
    }
}
