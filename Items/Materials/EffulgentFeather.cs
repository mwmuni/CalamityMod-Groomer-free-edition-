using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class EffulgentFeather : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 24;
        protected override int MaterialHeight => 24;
        protected override int SellValue => Item.sellPrice(gold: 1, silver: 30);
        protected override int MaterialRarity => ItemRarityID.Purple;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 11));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.SortingPriorityMaterials[Type] = 102;
        }

    }
}
