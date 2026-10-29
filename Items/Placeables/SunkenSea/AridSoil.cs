using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class AridSoil : PlaceableTileItem<Tiles.SunkenSea.AridSoil>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Basalt>();
        }

    }
}
