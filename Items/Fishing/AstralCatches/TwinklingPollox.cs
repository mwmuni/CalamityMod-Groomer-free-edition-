using CalamityMod.Items.Fishing;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class TwinklingPollox : BaseFishingCatch
    {
        protected override int CatchWidth => 32;
        protected override int CatchHeight => 28;
        protected override int CatchValue => Item.sellPrice(silver: 5);
        protected override int CatchRarity => ItemRarityID.Blue;
    }
}
