using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    internal static class YoyoDefaults
    {
        internal static void ApplyUseDefaults(Item item)
        {
            item.useStyle = ItemUseStyleID.Shoot;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;
        }
    }
}
