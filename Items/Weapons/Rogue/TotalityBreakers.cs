using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Systems.Collections;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TotalityBreakers : ConditionalStealthStrikeRogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3];
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
            Item.damage = 64;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<TotalityFlask>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 1.3f;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MolotovCocktail, 50).
                AddIngredient<SolarVeil>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
