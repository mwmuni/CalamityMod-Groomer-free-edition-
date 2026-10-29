using CalamityMod.NPCs.Astral;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class TwinklerItem : BaseCapturedCritterItem<Twinkler>
    {
        protected override int SellPrice => Item.sellPrice(silver: 10);
        protected override int? Bait => 40;
        protected override int? Rarity => ItemRarityID.Green;
    }
}
