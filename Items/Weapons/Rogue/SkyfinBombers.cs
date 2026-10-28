using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SkyfinBombers : ConditionalStealthStrikeRogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Irradiated>()];
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 46;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 35;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SkyfinNuke>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ContaminatedBile>().
                AddIngredient<CorrodedFossil>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
