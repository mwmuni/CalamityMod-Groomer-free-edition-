using CalamityMod.Items.Placeables.FurnitureProfaned;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class ProfanedRockWall : PlaceableWallItem<WallTiles.ProfanedRockWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<ProfanedRock>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
