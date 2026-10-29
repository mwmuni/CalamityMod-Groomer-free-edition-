using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureBotanic
{
    public class BotanicPlatform : PlaceableTileItem<Tiles.FurnitureBotanic.BotanicPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<UelibloomBrick>().
                Register();
        }
    }
}
