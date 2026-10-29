using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class GrandScale : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 28;
        protected override int MaterialHeight => 32;
        protected override int SellValue => Item.sellPrice(gold: 1);
        protected override int MaterialRarity => ItemRarityID.Lime;

    }
}
