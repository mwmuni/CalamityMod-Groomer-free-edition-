using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ReaperTooth : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 26;
        protected override int MaterialHeight => 26;
        protected override int SellValue => Item.sellPrice(gold: 1, silver: 50);
        protected override int MaterialRarity => ModContent.RarityType<PureGreen>();

    }
}
