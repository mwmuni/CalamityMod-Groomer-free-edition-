using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Crags
{
    public class BrimstoneSlag : PlaceableTileItem<Tiles.Crags.BrimstoneSlag>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneSlagWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
