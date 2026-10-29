using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    internal static class RangedAmmoDefaults
    {
        internal static void SetDefaults(
            Item item,
            int width,
            int height,
            int damage,
            float knockBack,
            int value,
            int rarity,
            int projectileType,
            float shootSpeed,
            int ammoType)
        {
            item.width = width;
            item.height = height;
            item.damage = damage;
            item.DamageType = DamageClass.Ranged;
            item.maxStack = Item.CommonMaxStack;
            item.consumable = true;
            item.knockBack = knockBack;
            item.value = value;
            item.rare = rarity;
            item.shoot = projectileType;
            item.shootSpeed = shootSpeed;
            item.ammo = ammoType;
        }
    }
}
