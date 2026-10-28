using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RegulusRiot : ConditionalStealthStrikeRogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 34;
            Item.damage = 100;
            Item.useAnimation = Item.useTime = 24;
            Item.knockBack = 4.5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Cyan;
            Item.DamageType = RogueDamageClass.Instance;

            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<RegulusRiotProj>();
        }
    }
}
