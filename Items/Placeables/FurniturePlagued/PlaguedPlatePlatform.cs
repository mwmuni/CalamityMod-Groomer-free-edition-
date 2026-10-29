using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurniturePlagued
{
    public class PlaguedPlatePlatform : PlaceableTileItem<Tiles.FurniturePlaguedPlate.PlaguedPlatePlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<PlaguedContainmentBrick>().
                Register();
        }
    }
}
