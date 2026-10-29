using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Crags
{
    public class BrimstoneSlab : PlaceableTileItem<Tiles.Crags.BrimstoneSlab>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneSlag>().
                AddTile(TileID.HeavyWorkBench).
                Register();
            CreateRecipe().
                AddIngredient<BrimstoneSlabWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
