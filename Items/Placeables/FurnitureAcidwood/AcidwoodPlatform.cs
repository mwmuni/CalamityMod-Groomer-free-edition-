using CalamityMod.Tiles.FurnitureAcidwood;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureAcidwood
{
    public class AcidwoodPlatform : PlaceableTileItem<AcidwoodPlatformTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<Acidwood>().
                Register();
        }
    }
}
