using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureNavystone
{
    public class NavystonePlatform : PlaceableTileItem<Tiles.FurnitureNavystone.NavystonePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<SmoothNavystone>().
                Register();
        }
    }
}
