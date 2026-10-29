using CalamityMod.Items.Placeables.FurnitureVoid;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class VoidstoneSlabWall : PlaceableWallItem<WallTiles.VoidstoneSlabWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<VoidstoneSlab>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
