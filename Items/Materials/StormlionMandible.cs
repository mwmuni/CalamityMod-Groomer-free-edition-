using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class StormlionMandible : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 36;
        protected override int MaterialHeight => 38;
        protected override int SellValue => Item.sellPrice(copper: 40);
        protected override int MaterialRarity => ItemRarityID.Blue;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

    }
}
