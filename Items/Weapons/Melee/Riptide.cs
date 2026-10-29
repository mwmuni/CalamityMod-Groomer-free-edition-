using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("Whirlpool")]
    public class Riptide : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static float Reach = 288f;
        public static float Speed = 25f;
        public static float Duration = 18f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed, Duration);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 48;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 12;
            Item.knockBack = 1f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            YoyoDefaults.ApplyUseDefaults(Item);

            Item.shoot = ModContent.ProjectileType<RiptideYoyo>();
            Item.shootSpeed = 18f;

            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PearlShard>(3).
                AddIngredient<SeaPrism>(7).
                AddIngredient<Navystone>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
