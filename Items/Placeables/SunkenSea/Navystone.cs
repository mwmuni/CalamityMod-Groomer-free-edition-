using CalamityMod.Items.Placeables.FurnitureNavystone;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class Navystone : PlaceableTileItem<Tiles.SunkenSea.Navystone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Runestone>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NavystoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();

            CreateRecipe().
                AddIngredient<NavystonePlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
