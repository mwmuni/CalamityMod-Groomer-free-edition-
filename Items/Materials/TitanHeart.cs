using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class TitanHeart : BaseMaterial
    {
        protected override int ResearchCount => 3;
        protected override int MaterialWidth => 10;
        protected override int MaterialHeight => 10;
        protected override int SellValue => Item.sellPrice(silver: 30);
        protected override int MaterialRarity => ItemRarityID.LightRed;

    }
}
