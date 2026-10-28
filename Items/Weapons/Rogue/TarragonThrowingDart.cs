using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TarragonThrowingDart : ConditionalStealthStrikeRogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.damage = 380;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 11;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.shoot = ModContent.ProjectileType<TarragonThrowingDartProjectile>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
