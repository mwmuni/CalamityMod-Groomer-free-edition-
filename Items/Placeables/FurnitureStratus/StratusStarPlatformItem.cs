using CalamityMod.Tiles.FurnitureStratus;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusStarPlatformItem : PlaceableTileItem<StratusStarPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<StratusBricks>().
                Register();
        }
    }
}
