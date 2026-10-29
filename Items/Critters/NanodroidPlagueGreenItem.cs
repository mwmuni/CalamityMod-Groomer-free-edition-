using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class NanodroidPlagueGreenItem : BaseCapturedCritterItem<NanodroidPlagueGreen>
    {
        protected override int SellPrice => Item.sellPrice(silver: 10);
        protected override int? Rarity => ModContent.RarityType<DarkOrange>();
    }
}
