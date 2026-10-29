using CalamityMod.Items.Fishing;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class ProcyonidPrawn : BaseFishingCatch
    {
        protected override int CatchWidth => 30;
        protected override int CatchHeight => 26;
        protected override int CatchValue => Item.sellPrice(silver: 15);
        protected override int CatchRarity => ItemRarityID.Blue;
    }
}
