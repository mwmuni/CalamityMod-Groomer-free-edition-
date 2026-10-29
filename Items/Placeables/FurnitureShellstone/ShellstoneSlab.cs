using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureShellstone
{
    public class ShellstoneSlab : PlaceableTileItem<Tiles.FurnitureShellstone.ShellstoneSlab>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Shellstone>().
                AddTile(TileID.HeavyWorkBench).
                Register();
            CreateRecipe().
                AddIngredient<ShellstoneSlabWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
