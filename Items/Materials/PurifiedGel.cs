using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class PurifiedGel : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 36;
        protected override int SellValue => Item.sellPrice(silver: 5);
        protected override int MaterialRarity => ItemRarityID.LightRed;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 71; // Soul of Light
        }

    }
}
