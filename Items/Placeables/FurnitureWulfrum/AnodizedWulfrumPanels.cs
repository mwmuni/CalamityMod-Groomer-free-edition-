using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class AnodizedWulfrumPanels : PlaceableTileItem<Tiles.FurnitureWulfrum.AnodizedWulfrumPanels>
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
                AddTile(TileID.HeavyWorkBench).
                Register();
        }
    }
}
