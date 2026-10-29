using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class EnergyCore : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 22;
        protected override int MaterialHeight => 22;
        protected override int SellValue => Item.sellPrice(copper: 40);
        protected override int MaterialRarity => ItemRarityID.Blue;

    }
}
