using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class SmoothAbyssGravelPlatform : PlaceableTileItem<Tiles.FurnitureAbyss.SmoothAbyssGravelPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<SmoothAbyssGravel>().
                Register();
        }
    }
}
