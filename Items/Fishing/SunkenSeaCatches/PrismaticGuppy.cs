using CalamityMod.Items.Fishing;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class PrismaticGuppy : BaseFishingCatch
    {
        protected override int CatchWidth => 30;
        protected override int CatchHeight => 28;
        protected override int CatchMaxStack => 9999;
        protected override int CatchValue => Terraria.Item.sellPrice(silver: 5);
        protected override int CatchRarity => ItemRarityID.Green;
    }
}