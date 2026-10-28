using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BloodsoakedCrasher : StealthStrikeRogueWeapon //This weapon has been coded by Ben || Termi
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Laceration>()];
        }
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 64;
            Item.damage = 245;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useAnimation = Item.useTime = 24;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<BloodsoakedCrashax>();

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrushsawCrasher>().
                AddIngredient<Bloodstone>(25).
                AddIngredient<BloodOrb>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
