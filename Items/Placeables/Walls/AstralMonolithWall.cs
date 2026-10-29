using CalamityMod.Items.Placeables.FurnitureMonolith;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AstralMonolithWall : PlaceableWallItem<WallTiles.AstralMonolithWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AstralMonolith>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
