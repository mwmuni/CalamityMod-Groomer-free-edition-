using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumSiding : PlaceableTileItem<Tiles.FurnitureWulfrum.WulfrumSiding>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddRecipeGroup("AnyStoneBlock", 25).
                AddIngredient<AnodizedWulfrumMetal>().
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<WulfrumSidingWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
