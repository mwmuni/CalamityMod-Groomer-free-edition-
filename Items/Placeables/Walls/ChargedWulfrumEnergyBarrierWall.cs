using CalamityMod.Items.Placeables.FurnitureWulfrum;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class ChargedWulfrumEnergyBarrierWall : PlaceableWallItem<WallTiles.ChargedWulfrumEnergyBarrierWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<ChargedWulfrumEnergyBarrier>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
