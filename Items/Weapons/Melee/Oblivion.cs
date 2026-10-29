using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TheEyeofCalamitas")]
    public class Oblivion : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static float Lifetime => 15; //Technically has infinite lifetime, but manually sets 15 seconds. See projectile file for details
        public static float Reach = 512f;
        public static float Speed = 36f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed, Lifetime);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 35;
            Item.knockBack = 4f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            YoyoDefaults.ApplyUseDefaults(Item);

            Item.shoot = ModContent.ProjectileType<OblivionYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Lime;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.oblivionCooldown == 0)
                return true;
            else
                return false;
        }
    }
}
