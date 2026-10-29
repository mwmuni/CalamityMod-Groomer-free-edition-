using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("FlamebeakHampick")]
    public class SeismicHampick : BaseMeleeTool
    {
        private const int PickPower = 210;
        private const int HammerPower = 95;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                52, 50, 58, 8f, 6, 15, Item.sellPrice(gold: 4, silver: 35), ItemRarityID.Yellow,
                pickPower: PickPower, hammerPower: HammerPower, tileBoost: 2);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.pick = 0;
                Item.hammer = HammerPower;
            }
            else
            {
                Item.pick = PickPower;
                Item.hammer = 0;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(15).
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
