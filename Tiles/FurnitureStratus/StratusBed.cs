using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStratus
{
    public class StratusBed : BaseBedTile
    {
        protected override int AssociatedItemType => ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusBed>();

        public override void SetStaticDefaults() => this.SetUpBed(ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusBed>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Firework_Blue, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
