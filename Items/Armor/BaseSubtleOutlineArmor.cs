using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    public abstract class BaseSubtleOutlineArmor : ModItem
    {
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }
    }
}
