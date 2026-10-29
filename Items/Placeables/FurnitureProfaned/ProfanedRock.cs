using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedRock : PlaceableTileItem<Tiles.FurnitureProfaned.ProfanedRock>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedRockWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<ProfanedPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
