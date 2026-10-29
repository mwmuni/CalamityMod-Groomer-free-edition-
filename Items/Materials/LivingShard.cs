using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class LivingShard : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 14;
        protected override int MaterialHeight => 14;
        protected override int SellValue => Item.sellPrice(silver: 50);
        protected override int MaterialRarity => ItemRarityID.Lime;

    }
}
