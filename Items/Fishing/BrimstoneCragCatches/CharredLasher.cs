using CalamityMod.Items.Fishing;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CharredLasher : BaseFishingCatch
    {
        protected override int CatchWidth => 44;
        protected override int CatchHeight => 36;
        protected override int CatchValue => Item.sellPrice(gold: 10);
        protected override int CatchRarity => ItemRarityID.Orange;
    }
}
