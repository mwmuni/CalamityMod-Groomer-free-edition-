using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenPlatform : PlaceableTileItem<Tiles.FurnitureAshen.AshenPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<SmoothBrimstoneSlag>().
                Register();
        }
    }
}
