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
    public class MangroveChakram : StealthStrikeRogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Venom];
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 84;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<MangroveChakramProjectile>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 1.25f;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
