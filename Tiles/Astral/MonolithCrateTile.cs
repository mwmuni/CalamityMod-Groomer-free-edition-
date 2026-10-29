using CalamityMod.Dusts;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class MonolithCrateTile : BaseFishingCrateTile<MonolithCrate>
    {
        protected override Color MapColor => new(110, 45, 117);
        protected override int CrateDustType => ModContent.DustType<AstralBlue>();

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool())
            {
                type = ModContent.DustType<AstralOrange>();
            }
            else
            {
                type = ModContent.DustType<AstralBlue>();
            }
            return true;
        }
    }
}
