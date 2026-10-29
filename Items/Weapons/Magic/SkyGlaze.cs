using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SkyGlaze : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 74;
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StickyFeather>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float featherSpeed = Item.shootSpeed;
            int featherAmt = 3;
            for (int i = 0; i < featherAmt; i++)
            {
                Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, i);
                Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, featherSpeed);
                float speedX4 = aimedVelocity.X + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = aimedVelocity.Y + (float)Main.rand.Next(-30, 31) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }
    }
}
