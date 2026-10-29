using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("PurityAxe")]
    public class AxeofPurity : BaseMeleeTool
    {
        private static int AxePower = 125 / 5;
        private static float PowderSpeed = 21f;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.shoot = ProjectileID.PurificationPowder;
            SetMeleeToolDefaults(
                58, 54, 55, 5f, 15, 15, CalamityGlobalItem.RarityLightRedBuyPrice, ItemRarityID.LightRed,
                axePower: AxePower);
            Item.shootSpeed = PowderSpeed;
        }

        public override bool CanShoot(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                return false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int powderDamage = (int)(0.85f * damage);
            int idx = Projectile.NewProjectile(source, position, velocity, type, powderDamage, knockback, player.whoAmI);
            Main.projectile[idx].DamageType = DamageClass.Melee;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void UseAnimation(Player player)
        {
            //Done here because no other override fits neatly enough in the pipeline.
            Item.axe = AxePower;

            if (player.altFunctionUse == 2)
                Item.axe = 0;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Enchanted_Pink);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FellerofEvergreens>().
                AddIngredient(ItemID.PurificationPowder, 20).
                AddIngredient(ItemID.PixieDust, 20).
                AddIngredient(ItemID.CrystalShard, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
