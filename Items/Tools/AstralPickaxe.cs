using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class AstralPickaxe : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                50, 60, 65, 5f, 6, 10, Item.sellPrice(gold: 6), ItemRarityID.Cyan,
                pickPower: 220, tileBoost: 3, crit: 25);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust d = CalamityUtils.MeleeDustHelper(player, Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(), 0.56f, 40, 65, -0.13f, 0.13f);
            if (d != null)
            {
                d.customData = 0.02f;
            }
        }
    }
}
