using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BallisticPoisonBomb : StealthStrikeRogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.damage = 57;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<BallisticPoisonBombProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeafoamBomb>().
                AddIngredient<SulphurousSand>(10).
                AddIngredient<Voidstone>(10).
                AddIngredient<DepthCells>(20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
