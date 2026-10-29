using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumPanels : PlaceableTileItem<Tiles.FurnitureWulfrum.WulfrumPanels>
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
                AddIngredient<WulfrumPanelWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
