using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class SmoothBrimstoneSlag : PlaceableTileItem<Tiles.FurnitureAshen.SmoothBrimstoneSlag>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneSlag>().
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<SmoothBrimstoneSlagWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<AshenPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
