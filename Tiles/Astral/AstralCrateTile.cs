using CalamityMod.Dusts;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralCrateTile : BaseFishingCrateTile<AstralCrate>
    {
        protected override Color MapColor => new(47, 66, 90);
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
