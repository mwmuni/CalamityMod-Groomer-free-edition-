using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureVoid
{
    public class VoidstoneSlab : PlaceableTileItem<Tiles.FurnitureVoid.VoidstoneSlab>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SmoothVoidstone>().
                AddTile<VoidCondenser>().
                Register();
            CreateRecipe().
                AddIngredient<VoidstoneSlabWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
