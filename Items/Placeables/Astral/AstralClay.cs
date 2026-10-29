using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Astral
{
    public class AstralClay : PlaceableTileItem<Tiles.Astral.AstralClay>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.ClayBlock, 1);
        }

    }
}
