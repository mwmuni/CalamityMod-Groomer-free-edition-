using CalamityMod.Items.Fishing;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CragBullhead : BaseFishingCatch
    {
        protected override int CatchWidth => 32;
        protected override int CatchHeight => 36;
        protected override int CatchValue => Item.sellPrice(silver: 7, copper: 50);
        protected override int CatchRarity => ItemRarityID.Blue;
    }
}
