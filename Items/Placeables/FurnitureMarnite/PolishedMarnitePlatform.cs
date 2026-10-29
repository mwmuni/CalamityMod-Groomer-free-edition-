using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureMarnite
{
    public class PolishedMarnitePlatform : PlaceableTileItem<Tiles.FurnitureMarnite.PolishedMarnitePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<PolishedMarniteBlock>().
                Register();
        }
    }
}
