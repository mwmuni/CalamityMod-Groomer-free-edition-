using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone
{
    [LegacyName("EutrophicPlatform")]
    public class AncientNavystonePlatform : PlaceableTileItem<Tiles.FurnitureNavystone.FurnitureAncientNavystone.AncientNavystonePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AncientSmoothNavystone>().
                Register();
        }
    }
}
