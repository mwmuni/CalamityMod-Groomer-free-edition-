using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("DemonicBoneAsh")]
    public class AncientBoneDust : BaseMaterial
    {
        protected override int ResearchCount => 5;
        protected override int MaterialWidth => 20;
        protected override int MaterialHeight => 20;
        protected override int SellValue => Item.sellPrice(copper: 10);
        protected override int MaterialRarity => ItemRarityID.Blue;

    }
}
