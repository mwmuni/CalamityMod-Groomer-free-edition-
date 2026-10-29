using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class PerennialBrickWall : PlaceableWallItem<WallTiles.PerennialBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<PerennialBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
