using CalamityMod.Dusts.Furniture;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureBotanic
{
    public class BotanicBed : BaseBedTile
    {
        protected override int AssociatedItemType => ModContent.ItemType<Items.Placeables.FurnitureBotanic.BotanicBed>();

        public override void SetStaticDefaults() => this.SetUpBed(ModContent.ItemType<Items.Placeables.FurnitureBotanic.BotanicBed>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<BloomTileGold>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<BloomTileLeaves>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
