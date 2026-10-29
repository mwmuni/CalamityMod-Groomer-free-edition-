using CalamityMod.Items.Placeables.Astral;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AstralDirtWall : PlaceableWallItem<WallTiles.AstralDirtWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AstralDirt>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
