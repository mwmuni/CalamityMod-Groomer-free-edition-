using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenCandle : BaseRedTorchStoneDustTile
    {
        public Asset<Texture2D> FlameTexture;

        public override void SetStaticDefaults() => this.SetUpCandle(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenCandle>(), true);

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

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 1, 1);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenCandle>();
        }

        public override bool RightClick(int i, int j)
        {
            FurnitureCommon.RightClickBreak(i, j);
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FlameTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureAshen/AshenCandleFlame");
            CalamityUtils.DrawFlameEffect(FlameTexture.Value, i, j);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 18)
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
        }
    }
}
