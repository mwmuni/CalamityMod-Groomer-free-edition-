using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureVoid
{
    public class SmoothVoidstonePlatform : PlaceableTileItem<Tiles.FurnitureVoid.SmoothVoidstonePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<SmoothVoidstone>().
                Register();
        }
    }
}
