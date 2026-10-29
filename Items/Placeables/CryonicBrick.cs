using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class CryonicBrick : PlaceableTileItem<Tiles.CryonicBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<CryonicOre>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<CryonicBrickWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
