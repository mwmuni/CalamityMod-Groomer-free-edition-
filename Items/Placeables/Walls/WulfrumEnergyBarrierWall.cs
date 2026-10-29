using CalamityMod.Items.Placeables.FurnitureWulfrum;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class WulfrumEnergyBarrierWall : PlaceableWallItem<WallTiles.WulfrumEnergyBarrierWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<WulfrumEnergyBarrier>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
