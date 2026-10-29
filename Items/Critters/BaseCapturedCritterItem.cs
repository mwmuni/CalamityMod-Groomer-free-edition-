using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public abstract class BaseCapturedCritterItem<TCritter> : ModItem, ILocalizedModType where TCritter : ModNPC
    {
        public new string LocalizationCategory => "Items.Misc";

        protected abstract int SellPrice { get; }
        protected virtual int? Bait => null;
        protected virtual int? Rarity => null;

        protected virtual void SetAdditionalDefaults() { }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter(ModContent.NPCType<TCritter>());

            int? bait = Bait;
            if (bait.HasValue)
                Item.bait = bait.Value;

            Item.value = SellPrice;

            int? rarity = Rarity;
            if (rarity.HasValue)
                Item.rare = rarity.Value;

            SetAdditionalDefaults();
        }
    }
}
