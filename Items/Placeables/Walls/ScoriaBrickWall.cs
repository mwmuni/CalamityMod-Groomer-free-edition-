using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("ChaoticBrickWall")]
    public class ScoriaBrickWall : PlaceableWallItem<WallTiles.ScoriaBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<ScoriaBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
