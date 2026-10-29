using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public abstract class BaseFishingCatch : ModItem, ILocalizedModType
    {
        protected abstract int CatchWidth { get; }
        protected abstract int CatchHeight { get; }
        protected abstract int CatchValue { get; }
        protected abstract int CatchRarity { get; }
        protected virtual int CatchMaxStack => Item.CommonMaxStack;

        public new string LocalizationCategory => "Items.Fishing";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = CatchWidth;
            Item.height = CatchHeight;
            Item.maxStack = CatchMaxStack;
            Item.value = CatchValue;
            Item.rare = CatchRarity;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Fish;
        }
    }
}
