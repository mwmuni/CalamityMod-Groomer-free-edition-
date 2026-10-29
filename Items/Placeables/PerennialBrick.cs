using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class PerennialBrick : PlaceableTileItem<Tiles.PerennialBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<PerennialOre>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<PerennialBrickWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<FloralWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
