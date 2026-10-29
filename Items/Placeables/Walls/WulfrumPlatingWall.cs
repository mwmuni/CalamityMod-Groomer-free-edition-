using CalamityMod.Items.Placeables.FurnitureWulfrum;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class WulfrumPlatingWall : PlaceableWallItem<WallTiles.WulfrumPlatingWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<WulfrumPlating>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
