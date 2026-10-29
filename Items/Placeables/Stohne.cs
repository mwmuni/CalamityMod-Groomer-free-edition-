using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class Stohne : PlaceableTileItem<Tiles.Stohne>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

    }
}
