using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class UelibloomBrickWall : PlaceableWallItem<WallTiles.UelibloomBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<UelibloomBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
