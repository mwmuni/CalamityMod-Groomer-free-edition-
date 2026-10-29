using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class HardenedEutrophicSand : PlaceableTileItem<Tiles.SunkenSea.HardenedEutrophicSand>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<EutrophicSand>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EutrophicSand>().
                AddIngredient(ItemID.DirtBlock).
                AddTile(TileID.Solidifier).
                Register();
        }
    }
}
