using CalamityMod.Tiles.FurnitureExo;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureExo
{
    public class ExoPlatform : PlaceableTileItem<ExoPlatformTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<ExoPlating>().
                Register();
        }
    }
}
