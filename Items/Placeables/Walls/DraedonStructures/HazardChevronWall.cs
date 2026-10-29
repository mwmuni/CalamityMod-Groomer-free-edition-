using Terraria.ID;
using Terraria.ModLoader;
using TileItems = CalamityMod.Items.Placeables.DraedonStructures;
using WallTiles = CalamityMod.Walls.DraedonStructures;
using CalamityMod.Items.Placeables.Walls;

namespace CalamityMod.Items.Placeables.Walls.DraedonStructures
{
    public class HazardChevronWall : PlaceableWallItem<WallTiles.HazardChevronWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<TileItems.HazardChevronPanels>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
