using CalamityMod.Items.Fishing;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class GleamingCucumber : BaseFishingCatch
    {
        protected override int CatchWidth => 32;
        protected override int CatchHeight => 36;
        protected override int CatchValue => Terraria.Item.sellPrice(silver: 5);
        protected override int CatchRarity => ItemRarityID.Blue;
    }
}
