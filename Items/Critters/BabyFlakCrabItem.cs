using CalamityMod.NPCs.AcidRain;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyFlakCrabItem : BaseCapturedCritterItem<BabyFlakCrab>
    {
        protected override int SellPrice => Item.sellPrice(silver: 5);
        protected override int? Rarity => ItemRarityID.White;
    }
}
