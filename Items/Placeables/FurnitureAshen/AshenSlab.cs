using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenSlab : PlaceableTileItem<Tiles.FurnitureAshen.AshenSlab>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SmoothBrimstoneSlag>().
                AddTile<AshenAltar>().
                Register();
            CreateRecipe().
                AddIngredient<AshenSlabWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
