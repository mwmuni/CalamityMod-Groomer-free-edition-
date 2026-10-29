using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class PiggyGoldItem : BaseCapturedCritterItem<PiggyGold>
    {
        protected override int SellPrice => Item.sellPrice(gold: 10);
        protected override int? Rarity => ItemRarityID.Orange;
    }
}
