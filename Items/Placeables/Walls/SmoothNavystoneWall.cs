using CalamityMod.Items.Placeables.FurnitureNavystone;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SmoothNavystoneWall : PlaceableWallItem<WallTiles.SmoothNavystoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SmoothNavystone>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
