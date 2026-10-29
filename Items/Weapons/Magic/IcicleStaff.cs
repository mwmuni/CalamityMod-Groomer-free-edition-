using CalamityMod.Items.Weapons;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IcicleStaff : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useTime = 7;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IcicleStaffProj>();
            Item.shootSpeed = 11f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int i = Main.myPlayer;
            float icicleSpeed = velocity.Length();
            float playerKnockback = knockback;
            playerKnockback = player.GetWeaponKnockback(Item, playerKnockback);
            player.itemTime = Item.useTime;
            Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, 0, verticalStep: 0);
            Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, icicleSpeed);
            float speedX4 = aimedVelocity.X;
            float speedY5 = aimedVelocity.Y + (float)Main.rand.Next(-40, 41) * 0.02f;
            Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<IcicleStaffProj>(), damage, playerKnockback, i, 0f, (float)Main.rand.Next(10));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyIceBlock", 25).
                AddIngredient(ItemID.Shiverthorn, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
