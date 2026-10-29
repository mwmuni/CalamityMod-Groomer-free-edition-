using CalamityMod.Tiles.FurnitureAuric;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureAuric
{
    public class AuricPlatform : PlaceableTileItem<AuricPlatformTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AuricPanel>().
                Register();
        }
    }
}
