using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class RuinousSoul : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 26;
        protected override int MaterialHeight => 42;
        protected override int SellValue => Item.sellPrice(gold: 1, silver: 50);
        protected override int MaterialRarity => ModContent.RarityType<PureGreen>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.SortingPriorityMaterials[Type] = 111;
        }

    }
}
