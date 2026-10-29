using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class ChargedWulfrumEnergyBarrier : PlaceableTileItem<Tiles.FurnitureWulfrum.ChargedWulfrumEnergyBarrier>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient(ItemID.Glass, 25).
                AddIngredient<EnergyCore>().
                AddTile(TileID.HeavyWorkBench).
                Register();
            CreateRecipe().
                AddIngredient<ChargedWulfrumEnergyBarrierWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
