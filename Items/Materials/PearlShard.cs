using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("VictoryShard")]
    public class PearlShard : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 26;
        protected override int SellValue => Item.sellPrice(silver: 1);
        protected override int MaterialRarity => ItemRarityID.Green;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 60; // Meteorite
        }

    }
}
