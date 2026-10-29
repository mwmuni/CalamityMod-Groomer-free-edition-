using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("ChaoswarpedSlashaxe")]
    public class TectonicTruncator : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                50, 50, 68, 7f, 6, 17, Item.sellPrice(gold: 4, silver: 35), ItemRarityID.Yellow,
                axePower: 190 / 5, tileBoost: 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 16 : 127);
            }
            if (Main.rand.NextBool(5) && !Main.dedServ)
            {
                int smoke = Gore.NewGore(player.GetSource_ItemUse(Item), new Vector2(hitbox.X, hitbox.Y), default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}
