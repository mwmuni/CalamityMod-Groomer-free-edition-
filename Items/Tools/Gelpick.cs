using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Gelpick : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                46, 48, 19, 2.5f, 9, 12, Item.sellPrice(silver: 75), ItemRarityID.LightRed,
                pickPower: 105, tileBoost: 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(18).
                AddIngredient<BlightedGel>(18).
                AddTile(TileID.Solidifier).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurificationPowder);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}
