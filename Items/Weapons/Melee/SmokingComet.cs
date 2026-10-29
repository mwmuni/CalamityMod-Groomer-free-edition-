using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SmokingComet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static float Reach = 320f;
        public static float Speed = 20f;
        public static float Duration = 21f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed, Duration);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 40;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 17;
            Item.knockBack = 1.5f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            YoyoDefaults.ApplyUseDefaults(Item);

            Item.shoot = ModContent.ProjectileType<SmokingCometYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddIngredient(ItemID.PinkGel, 10).
                AddIngredient(ItemID.FallenStar, 15).
                AddIngredient(ItemID.MeteoriteBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
