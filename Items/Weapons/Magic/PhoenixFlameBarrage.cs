using CalamityMod.Items.Weapons;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PhoenixFlameBarrage : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 106;
            Item.height = 140;
            Item.damage = 140;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HolyFlame>();
            Item.shootSpeed = 30f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float flameSpeed = Item.shootSpeed;
            player.itemTime = Item.useTime;
            int flameAmt = 30;
            if (Main.rand.NextBool(3))
            {
                flameAmt++;
            }
            if (Main.rand.NextBool(3))
            {
                flameAmt++;
            }
            for (int i = 0; i < flameAmt; i++)
            {
                Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, i);
                Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, flameSpeed);
                float speedX4 = aimedVelocity.X + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = aimedVelocity.Y + (float)Main.rand.Next(-30, 31) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }
    }
}
