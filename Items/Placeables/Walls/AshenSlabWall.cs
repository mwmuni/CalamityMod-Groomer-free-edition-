using CalamityMod.Items.Placeables.FurnitureAshen;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AshenSlabWall : PlaceableWallItem<WallTiles.AshenSlabWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AshenSlab>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
