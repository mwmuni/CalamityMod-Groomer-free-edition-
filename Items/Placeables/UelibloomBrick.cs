using CalamityMod.Items.Placeables.FurnitureBotanic;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class UelibloomBrick : PlaceableTileItem<Tiles.UelibloomBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<UelibloomOre>().
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<UelibloomBrickWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<BotanicPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
