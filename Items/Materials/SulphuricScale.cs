using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("SulfuricScale")]
    public class SulphuricScale : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 20;
        protected override int SellValue => Item.sellPrice(copper: 60);
        protected override int MaterialRarity => ItemRarityID.Green;

    }
}
