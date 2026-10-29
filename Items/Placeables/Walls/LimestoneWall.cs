using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
using CalamityMod.Items.Placeables.SunkenSea;

namespace CalamityMod.Items.Placeables.Walls
{
    public class LimestoneWall : PlaceableWallItem<WallTiles.LimestoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Limestone>().
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
