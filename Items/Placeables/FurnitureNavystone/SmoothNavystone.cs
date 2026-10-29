using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureNavystone
{
    public class SmoothNavystone : PlaceableTileItem<Tiles.FurnitureNavystone.SmoothNavystone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AncientSmoothNavystone>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Navystone>().
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<SmoothNavystoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
