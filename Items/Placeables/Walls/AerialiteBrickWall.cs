using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AerialiteBrickWall : PlaceableWallItem<WallTiles.AerialiteBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AerialiteBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
