using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("Stardust")]
    public class StarblightSoot : BaseMaterial
    {
        protected override int ResearchCount => 100;
        protected override int MaterialWidth => 30;
        protected override int MaterialHeight => 26;
        protected override int SellValue => Item.sellPrice(silver: 2);
        protected override int MaterialRarity => ItemRarityID.LightRed;

    }
}
