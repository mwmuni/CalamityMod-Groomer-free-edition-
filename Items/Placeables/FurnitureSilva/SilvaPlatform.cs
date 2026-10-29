using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaPlatform : PlaceableTileItem<Tiles.FurnitureSilva.SilvaPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<SilvaCrystal>().
                Register();
        }
    }
}
