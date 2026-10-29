using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("PlagueCellCluster")]
    public class PlagueCellCanister : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 20;
        protected override int SellValue => Item.sellPrice(silver: 15);
        protected override int MaterialRarity => ItemRarityID.Yellow;

    }
}
