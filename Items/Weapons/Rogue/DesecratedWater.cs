using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DesecratedWater : ConditionalStealthStrikeRogueWeapon
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.damage = 55;
            Item.useAnimation = 29;
            Item.useTime = 29;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DesecratedWaterProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilWater", 100).
                AddRecipeGroup("AnyAdamantiteBar", 5).
                AddRecipeGroup("CursedFlameIchor", 5).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
