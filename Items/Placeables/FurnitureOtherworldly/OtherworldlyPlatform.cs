using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultPlatform")]
    public class OtherworldlyPlatform : PlaceableTileItem<Tiles.FurnitureOtherworldly.OtherworldlyPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<OtherworldlyStone>().
                Register();
        }
    }
}
