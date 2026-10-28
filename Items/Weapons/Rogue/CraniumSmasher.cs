using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CraniumSmasher : StealthStrikeRogueWeapon
    {
        private bool throwExplosive = false;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 140;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<CraniumSmasherProj>();
            Item.shootSpeed = 20f;
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                type = ModContent.ProjectileType<CraniumSmasherStealth>();
            }
            else
            {
                type = throwExplosive ? ModContent.ProjectileType<CraniumSmasherExplosive>() : ModContent.ProjectileType<CraniumSmasherProj>();
                throwExplosive = !throwExplosive;
            }
        }

    }
}
