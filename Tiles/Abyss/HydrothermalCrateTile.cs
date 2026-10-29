using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class HydrothermalCrateTile : BaseFishingCrateTile<HydrothermalCrate>
    {
        protected override Color MapColor => new(150, 109, 26);
        protected override int CrateDustType => DustID.Flare;

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool())
                type = 44;
            else
                type = 127;

            return true;
        }
    }
}
