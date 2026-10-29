using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    public abstract class BaseBossMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Terraria.Item.sellPrice(silver: 75);
            Item.vanity = true;
        }
    }
}
