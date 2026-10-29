using CalamityMod.Items.Fishing;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class GreenwaveLoach : BaseFishingCatch
    {
        protected override int CatchWidth => 40;
        protected override int CatchHeight => 38;
        protected override int CatchValue => Terraria.Item.sellPrice(gold: 10);
        protected override int CatchRarity => ItemRarityID.Orange;
    }
}
