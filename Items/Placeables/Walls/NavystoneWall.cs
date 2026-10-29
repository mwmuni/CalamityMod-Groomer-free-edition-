using CalamityMod.Items.Placeables.SunkenSea;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("NavystoneWallSafe")]
    public class NavystoneWall : PlaceableWallItem<WallTiles.NavystoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<UnsafeNavystoneWall>();
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Navystone>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
