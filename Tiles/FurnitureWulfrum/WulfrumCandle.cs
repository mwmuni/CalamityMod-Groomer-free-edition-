using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureWulfrum
{
    public class WulfrumCandle : BaseBlueWhiteFrameLightTile
    {
        public Asset<Texture2D> FlameTexture;

        public override void SetStaticDefaults() => this.SetUpCandle(ModContent.ItemType<Items.Placeables.FurnitureWulfrum.WulfrumCandle>());

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.WoodFurniture, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 1, 1);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnitureWulfrum.WulfrumCandle>();
        }

        public override bool RightClick(int i, int j)
        {
            FurnitureCommon.RightClickBreak(i, j);
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FlameTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureWulfrum/WulfrumCandleFlame");
            CalamityUtils.DrawFlameEffect(FlameTexture.Value, i, j);
        }
    }
}
