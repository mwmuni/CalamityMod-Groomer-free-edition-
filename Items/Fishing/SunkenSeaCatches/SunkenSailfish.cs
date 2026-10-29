using CalamityMod.Items.Fishing;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SunkenSailfish : BaseFishingCatch
    {
        protected override int CatchWidth => 58;
        protected override int CatchHeight => 52;
        protected override int CatchValue => Terraria.Item.sellPrice(silver: 15);
        protected override int CatchRarity => ItemRarityID.Green;
    }
}
