using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ArmoredShell : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 46;
        protected override int MaterialHeight => 34;
        protected override int SellValue => Item.sellPrice(gold: 1, silver: 40);
        protected override int MaterialRarity => ModContent.RarityType<Turquoise>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 107;
        }

    }
}
