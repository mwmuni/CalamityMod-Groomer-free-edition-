using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class Limestone : PlaceableTileItem<Tiles.SunkenSea.Limestone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MossyStone>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<LimestoneWall>().
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
