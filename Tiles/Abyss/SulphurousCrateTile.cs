using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    [LegacyName("AbyssalCrateTile")]
    public class SulphurousCrateTile : BaseFishingCrateTile<SulphurousCrate>
    {
        protected override Color MapColor => new(47, 79, 79);
        protected override int CrateDustType => DustID.Water;

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool())
                type = 44;
            else
                type = 33;

            return true;
        }
    }
}
