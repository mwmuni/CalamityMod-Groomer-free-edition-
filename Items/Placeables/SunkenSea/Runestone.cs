using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class Runestone : PlaceableTileItem<Tiles.SunkenSea.Runestone>
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Shellstone>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RunestoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }

    }
}
