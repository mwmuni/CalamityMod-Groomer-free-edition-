using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DepthCells : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 26;
        protected override int MaterialHeight => 26;
        protected override int SellValue => Item.sellPrice(silver: 12);
        protected override int MaterialRarity => ItemRarityID.Lime;

    }
}
