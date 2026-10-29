using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureMonolith
{
    public class MonolithPlatform : PlaceableTileItem<Tiles.FurnitureMonolith.MonolithPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AstralMonolith>().
                Register();
        }
    }
}
