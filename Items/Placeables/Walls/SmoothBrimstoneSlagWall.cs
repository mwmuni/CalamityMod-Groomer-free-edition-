using CalamityMod.Items.Placeables.FurnitureAshen;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SmoothBrimstoneSlagWall : PlaceableWallItem<WallTiles.SmoothBrimstoneSlagWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SmoothBrimstoneSlag>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
