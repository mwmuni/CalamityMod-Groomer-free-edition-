using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    [LegacyName("ChaoticBrick")]
    public class ScoriaBrick : PlaceableTileItem<Tiles.ScoriaBrick>
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<ScoriaOre>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<ScoriaBrickWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
