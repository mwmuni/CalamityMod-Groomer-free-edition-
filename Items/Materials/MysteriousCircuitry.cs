using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MysteriousCircuitry : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 30;
        protected override int MaterialHeight => 24;
        protected override int SellValue => Item.sellPrice(silver: 3);
        protected override int MaterialRarity => ModContent.RarityType<DarkOrange>();

    }
}
