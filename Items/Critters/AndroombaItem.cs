using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class AndroombaItem : BaseCapturedCritterItem<AndroombaFriendly>
    {
        protected override int SellPrice => Item.sellPrice(silver: 10);
        protected override int? Rarity => ModContent.RarityType<DarkOrange>();
    }
}
