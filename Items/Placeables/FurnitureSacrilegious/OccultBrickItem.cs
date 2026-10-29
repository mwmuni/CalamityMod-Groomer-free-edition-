using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class OccultBrickItem : PlaceableTileItem<OccultBrickTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddRecipeGroup("AnyStoneBlock", 400).
                AddIngredient<AshesofAnnihilation>().
                AddTile<SCalAltar>().
                Register();
            CreateRecipe().
                AddIngredient<OccultPlatformItem>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<OccultBrickWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
