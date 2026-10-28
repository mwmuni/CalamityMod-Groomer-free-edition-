using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Icebreaker : ConditionalStealthStrikeRogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2, BuffID.Frozen];
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 60;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<IcebreakerHammer>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;
        }
    }
}
