using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class RoundedAnodizedWulfrumPanelWall : PlaceableWallItem<WallTiles.RoundedAnodizedWulfrumPanelWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Items.Placeables.FurnitureWulfrum.RoundedAnodizedWulfrumPanels>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
