using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class InfectedArmorPlating : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 20;
        protected override int SellValue => Item.sellPrice(silver: 80);
        protected override int MaterialRarity => ItemRarityID.Yellow;

    }
}
