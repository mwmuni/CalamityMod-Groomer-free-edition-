using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone
{
    public class AncientSmoothNavystone : PlaceableTileItem<Tiles.FurnitureNavystone.FurnitureAncientNavystone.AncientSmoothNavystone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        /*public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AncientSmoothNavystoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }*/
    }
}
