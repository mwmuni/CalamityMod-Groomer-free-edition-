using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MolluskHusk : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 30;
        protected override int MaterialHeight => 28;
        protected override int SellValue => Item.sellPrice(silver: 8);
        protected override int MaterialRarity => ItemRarityID.Pink;

    }
}
