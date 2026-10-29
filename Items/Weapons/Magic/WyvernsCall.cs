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
    public class WyvernsCall : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<WindChilled>()];
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 74;
            Item.damage = 65;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useAnimation = Item.useTime = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.75f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WyvernFeatherPurple>();
            Item.shootSpeed = 35f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float projSpeed = Item.shootSpeed;
            for (int i = 0; i < 3; i++)
            {
                float damageMult = 1f;
                float kbMult = 1f;
                if (Main.rand.NextBool(10))
                {
                    type = ModContent.ProjectileType<WyvernProjectile>();
                }
                else if (Main.rand.NextBool(3))
                {
                    type = ModContent.ProjectileType<WyvernFeatherGreen>();
                }
                else if (Main.rand.NextBool())
                {
                    type = ModContent.ProjectileType<WyvernFeatherPink>();
                }
                if (type == ModContent.ProjectileType<WyvernProjectile>())
                {
                    damageMult = 15f;
                    kbMult = 1.5f;
                }
                if (type == ModContent.ProjectileType<WyvernFeatherPink>())
                {
                    kbMult = 2f;
                }
                Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, i);
                Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, projSpeed);
                int feather = Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, aimedVelocity.X, aimedVelocity.Y, type, (int)(damage * damageMult), (int)(knockback * kbMult), player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkyGlaze>().
                AddRecipeGroup("AnyMythrilBar", 5).
                AddIngredient(ItemID.SoulofFlight, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
