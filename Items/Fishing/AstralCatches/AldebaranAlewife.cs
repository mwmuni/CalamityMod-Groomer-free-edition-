using CalamityMod.Items.Fishing;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class AldebaranAlewife : BaseFishingCatch
    {
        protected override int CatchWidth => 38;
        protected override int CatchHeight => 36;
        protected override int CatchValue => Item.sellPrice(silver: 15);
        protected override int CatchRarity => ItemRarityID.Blue;
    }
}
