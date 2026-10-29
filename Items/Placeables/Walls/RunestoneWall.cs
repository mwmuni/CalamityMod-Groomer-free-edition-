using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
using CalamityMod.Items.Placeables.SunkenSea;

namespace CalamityMod.Items.Placeables.Walls
{
    public class RunestoneWall : PlaceableWallItem<WallTiles.RunestoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Runestone>().
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
