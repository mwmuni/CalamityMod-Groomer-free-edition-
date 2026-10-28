using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("SeashellBoomerang")]
    public class FishboneBoomerang : StealthStrikeRogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.damage = 27;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 5.5f;
            Item.UseSound = null;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<FishboneBoomerangProjectile>();
            Item.shootSpeed = 3f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
