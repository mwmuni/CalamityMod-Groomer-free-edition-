using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureRunestone
{
    public class RunestoneTerracotta : PlaceableTileItem<Tiles.FurnitureRunestone.RunestoneTerracotta>
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        //public override void AddRecipes()
        //{
        //    CreateRecipe().
        //        AddIngredient<RunestoneWall>(4).
        //        AddTile(TileID.WorkBenches).
        //        DisableDecraft().
        //        Register();
        //}

    }
}
