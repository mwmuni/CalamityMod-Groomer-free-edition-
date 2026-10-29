using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Crags
{
    public class ScorchedRemains : PlaceableTileItem<Tiles.Crags.ScorchedRemains>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

    }
}
