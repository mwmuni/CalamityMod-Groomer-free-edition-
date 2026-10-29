using CalamityMod.NPCs.SunkenSea;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyGhostBellItem : BaseCapturedCritterItem<BabyGhostBell>
    {
        protected override int SellPrice => Item.sellPrice(silver: 10);
        protected override int? Bait => 20;
        protected override int? Rarity => ItemRarityID.Green;
    }
}
