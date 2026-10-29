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
    public class BloodBath : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BurningBlood>()];
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 50;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.75f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodBeam>();
            Item.shootSpeed = 9f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float bloodSpeed = Item.shootSpeed;
            int bloodAmt = 2;
            if (Main.rand.NextBool(3))
            {
                bloodAmt++;
            }
            for (int i = 0; i < bloodAmt; i++)
            {
                Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, i);
                Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, bloodSpeed);
                float speedX4 = aimedVelocity.X + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = aimedVelocity.Y + (float)Main.rand.Next(-30, 31) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }
    }
}
