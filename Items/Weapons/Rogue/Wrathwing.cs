using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("ProfanedTrident")]
    public class Wrathwing : StealthStrikeRogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 152;
            Item.height = 148;
            Item.damage = 592;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 13;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.shoot = ModContent.ProjectileType<WrathwingSpear>();
            Item.shootSpeed = 28f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 0.9f;

    }
}
