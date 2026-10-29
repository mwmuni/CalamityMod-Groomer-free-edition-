using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
using CalamityMod.Items.Placeables.FurnitureShellstone;

namespace CalamityMod.Items.Placeables.Walls
{
    public class ShellstoneSlabWall : PlaceableWallItem<WallTiles.ShellstoneSlabWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes() 
        {
            CreateRecipe(4).
                AddIngredient<ShellstoneSlab>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
