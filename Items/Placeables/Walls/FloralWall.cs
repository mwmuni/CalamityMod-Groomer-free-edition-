using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class FloralWall : PlaceableWallItem<WallTiles.FloralWall>
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
            /*CreateRecipe(4).
                AddIngredient<PerennialPillar>().
                AddTile(TileID.WorkBenches).
                Register();*/
        }
    }
}
