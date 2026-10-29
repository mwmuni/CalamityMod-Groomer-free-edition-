using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class AstralHamaxe : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                60, 70, 80, 5f, 5, 20, Item.sellPrice(gold: 4, silver: 50), ItemRarityID.Cyan,
                axePower: 165 / 5, hammerPower: 100, tileBoost: 3, crit: 25);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust d = CalamityUtils.MeleeDustHelper(player, Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(), 0.48f, 50, 78, -0.1f, 0.1f);
            if (d != null)
            {
                d.customData = 0.02f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
