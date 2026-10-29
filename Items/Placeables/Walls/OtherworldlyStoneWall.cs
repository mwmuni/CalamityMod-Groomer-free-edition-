using CalamityMod.Items.Placeables.FurnitureOtherworldly;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("OccultStoneWall")]
    public class OtherworldlyStoneWall : PlaceableWallItem<WallTiles.OtherworldlyStoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<OtherworldlyStone>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
