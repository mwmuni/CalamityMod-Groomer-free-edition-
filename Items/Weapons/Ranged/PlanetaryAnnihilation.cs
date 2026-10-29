using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PlanetaryAnnihilation : BaseMouseTargetedProjectileWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ElementalMix>()];
        }
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 102;
            Item.damage = 66;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item75;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PlanetaryAnnihilationProj>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float arrowSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = GetRandomizedSpawnPosition(player, 1);
            Vector2 aimedVelocity = GetMouseTargetedVelocity(realPlayerPos, arrowSpeed);
            if (CalamityUtils.CheckWoodenAmmo(type, player))
            {
                for (int i = 0; i < 7; i++)
                {
                    float speedX4 = aimedVelocity.X + Main.rand.Next(-120, 121) * 0.02f;
                    float speedY5 = aimedVelocity.Y + Main.rand.Next(-120, 121) * 0.02f;
                    Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<PlanetaryAnnihilationProj>(), damage, knockback, player.whoAmI, 0f, i);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    float speedX4 = aimedVelocity.X + Main.rand.Next(-120, 121) * 0.02f;
                    float speedY5 = aimedVelocity.Y + Main.rand.Next(-120, 121) * 0.02f;
                    Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VernalBolter>().
                AddIngredient(ItemID.DaedalusStormbow).
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
