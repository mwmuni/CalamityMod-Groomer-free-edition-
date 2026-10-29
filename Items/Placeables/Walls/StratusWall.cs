using CalamityMod.Items.Placeables.FurnitureStratus;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class StratusWall : PlaceableWallItem<WallTiles.StratusWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<StratusBricks>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
