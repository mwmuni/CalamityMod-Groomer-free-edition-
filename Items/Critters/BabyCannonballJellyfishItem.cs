using CalamityMod.NPCs.Abyss;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyCannonballJellyfishItem : BaseCapturedCritterItem<BabyCannonballJellyfish>
    {
        // Intentionally not bait because their bestiary entry uses them as ammunition.
        protected override int SellPrice => Item.sellPrice(silver: 10);
        protected override int? Rarity => ItemRarityID.Green;

        protected override void SetAdditionalDefaults()
        {
            Item.damage = 300;
        }
    }
}
