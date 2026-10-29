using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class PiggyItem : BaseCapturedCritterItem<Piggy>
    {
        protected override int SellPrice => Item.sellPrice(silver: 5);
        protected override int? Rarity => ItemRarityID.Blue;

        protected override void SetAdditionalDefaults()
        {
            Item.Calamity().donorItem = true;
        }
    }
}
