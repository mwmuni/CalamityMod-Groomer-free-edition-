using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YinYo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static float Reach = 400f;
        public static float Speed = 32f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 44;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 44;
            Item.knockBack = 3.5f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            YoyoDefaults.ApplyUseDefaults(Item);

            Item.shoot = ModContent.ProjectileType<YinYoyo>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Valor).
                AddIngredient(ItemID.DarkShard).
                AddIngredient(ItemID.LightShard).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
