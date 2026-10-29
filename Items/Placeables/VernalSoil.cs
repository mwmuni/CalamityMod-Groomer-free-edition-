using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class VernalSoil : PlaceableTileItem<Tiles.VernalSoil>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient(ItemID.MudBlock, 25).
                AddIngredient(ItemID.JungleSpores).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
