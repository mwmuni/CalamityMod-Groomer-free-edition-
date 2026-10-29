using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureCosmilite
{
    public class CosmilitePlatform : PlaceableTileItem<Tiles.FurnitureCosmilite.CosmilitePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<CosmiliteBrick>().
                Register();
        }
    }
}
