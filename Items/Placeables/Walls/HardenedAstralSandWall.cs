using CalamityMod.Items.Placeables.Astral;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class HardenedAstralSandWall : PlaceableWallItem<WallTiles.HardenedAstralSandWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddTile(TileID.WorkBenches).
                AddIngredient<HardenedAstralSand>().
                Register();
        }
    }
}
