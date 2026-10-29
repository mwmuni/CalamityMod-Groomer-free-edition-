using CalamityMod.Items.Placeables.Crags;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class BrimstoneSlagWall : PlaceableWallItem<WallTiles.BrimstoneSlagWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<BrimstoneSlag>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
