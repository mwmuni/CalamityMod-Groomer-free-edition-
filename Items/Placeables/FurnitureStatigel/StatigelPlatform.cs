using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
    public class StatigelPlatform : PlaceableTileItem<Tiles.FurnitureStatigel.StatigelPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<StatigelBlock>().
                Register();
        }
    }
}
