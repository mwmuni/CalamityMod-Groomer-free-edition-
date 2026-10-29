using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
using CalamityMod.Items.Placeables.FurnitureShellstone;

namespace CalamityMod.Items.Placeables.Walls
{
    public class ShellTrimBrickWall : PlaceableWallItem<WallTiles.ShellTrimBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes() 
        {
            CreateRecipe(4).
                AddIngredient<ShellTrimBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
