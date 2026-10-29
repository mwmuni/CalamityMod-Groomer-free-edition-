using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
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
    public class UndinesRetribution : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<CrushDepth>()];
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.damage = 44;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<UndinesRetributionSpear>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spearSpeed = Item.shootSpeed;
            for (int i = 0; i < 3; i++)
            {
                Vector2 realPlayerPos = GetRandomizedSpawnPosition(
                    player,
                    i,
                    directionalRandomMaxExclusive: 51,
                    horizontalSpread: 50,
                    spawnBelowPlayer: true,
                    verticalStep: 100);
                Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, spearSpeed);
                float speedX6 = aimedVelocity.X + (float)Main.rand.Next(-60, 61) * 0.02f;
                float speedY7 = aimedVelocity.Y + (float)Main.rand.Next(-60, 61) * 0.02f;
                float ai1 = Main.rand.NextFloat() + 0.5f;
                int bullet2 = Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX6, -speedY7, type, damage, knockback, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Voidstone>(30).
                AddIngredient<DepthCells>(30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
