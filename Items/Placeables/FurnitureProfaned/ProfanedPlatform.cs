using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedPlatform : PlaceableTileItem<Tiles.FurnitureProfaned.ProfanedPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<ProfanedRock>().
                Register();
        }
    }
}
