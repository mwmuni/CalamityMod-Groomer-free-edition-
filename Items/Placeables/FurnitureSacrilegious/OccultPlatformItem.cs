using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class OccultPlatformItem : PlaceableTileItem<OccultPlatformTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<OccultBrickItem>().
                Register();
        }
    }
}
