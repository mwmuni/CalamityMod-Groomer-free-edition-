using CalamityMod.Items.Placeables.FurnitureWulfrum;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AnodizedWulfrumTrimWall : PlaceableWallItem<WallTiles.AnodizedWulfrumTrimWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AnodizedWulfrumTrim>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
