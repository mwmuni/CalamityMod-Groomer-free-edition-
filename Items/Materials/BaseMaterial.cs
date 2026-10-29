using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public abstract class BaseMaterial : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        protected abstract int ResearchCount { get; }
        protected abstract int MaterialWidth { get; }
        protected abstract int MaterialHeight { get; }
        protected abstract int SellValue { get; }
        protected abstract int MaterialRarity { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = ResearchCount;

        public override void SetDefaults()
        {
            Item.width = MaterialWidth;
            Item.height = MaterialHeight;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = SellValue;
            Item.rare = MaterialRarity;
        }
    }
}
