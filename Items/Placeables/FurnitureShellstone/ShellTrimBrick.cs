using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureShellstone
{
    public class ShellTrimBrick : PlaceableTileItem<Tiles.FurnitureShellstone.ShellTrimBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        //public override void AddRecipes()
        //{
        //    CreateRecipe().
        //        AddIngredient<Shellstone>().
        //        AddTile(TileID.HeavyWorkBench).
        //        Register();
        //    CreateRecipe().
        //        AddIngredient<ShellstoneSlabWall>(4).
        //        AddTile(TileID.WorkBenches).
        //        DisableDecraft().
        //        Register();
        //}
    }
}
