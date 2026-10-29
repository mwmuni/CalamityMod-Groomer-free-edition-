using CalamityMod.NPCs.NormalNPCs;
using Terraria;

namespace CalamityMod.Items.Critters
{
    public class ShroombleItem : BaseCapturedCritterItem<Shroomble>
    {
        protected override int SellPrice => Item.sellPrice(silver: 1);
    }
}
