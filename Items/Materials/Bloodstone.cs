using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("BloodstoneCore")]
    public class Bloodstone : BaseMaterial
    {
        protected override int ResearchCount => 100;
        protected override int MaterialWidth => 13;
        protected override int MaterialHeight => 10;
        protected override int SellValue => Item.sellPrice(silver: 60);
        protected override int MaterialRarity => ModContent.RarityType<Turquoise>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 112;
        }

    }
}
