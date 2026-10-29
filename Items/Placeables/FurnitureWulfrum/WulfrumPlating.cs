using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumPlating : PlaceableTileItem<Tiles.FurnitureWulfrum.WulfrumPlating>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddRecipeGroup("AnyStoneBlock", 25).
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<WulfrumPlatingWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<WulfrumComplexPanelWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
