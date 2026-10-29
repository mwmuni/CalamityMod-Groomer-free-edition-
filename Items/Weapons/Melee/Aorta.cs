using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Aorta : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static float Reach = 330f;
        public static float Speed = 25f;
        public static float Duration = 24f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed, Duration);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 31;
            Item.knockBack = 4.25f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            YoyoDefaults.ApplyUseDefaults(Item);

            Item.shoot = ModContent.ProjectileType<AortaYoyo>();
            Item.shootSpeed = 8f;

            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
    }
}
