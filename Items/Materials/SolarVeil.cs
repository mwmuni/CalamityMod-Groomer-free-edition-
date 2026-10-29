using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class SolarVeil : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 46;
        protected override int MaterialHeight => 32;
        protected override int SellValue => Item.sellPrice(silver: 15);
        protected override int MaterialRarity => ItemRarityID.Lime;

    }
}
