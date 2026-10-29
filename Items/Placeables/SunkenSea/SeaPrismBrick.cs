using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class SeaPrismBrick : PlaceableTileItem<Tiles.SunkenSea.SeaPrismBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                    AddRecipeGroup("AnyStoneBlock", 25).
                    AddIngredient<SeaPrism>().
                    AddTile(TileID.Furnaces).
                    Register();
        }
    }
}
