using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class PrismCrateTile : BaseFishingCrateTile<PrismCrate>
    {
        protected override Color MapColor => new(145, 240, 255);
        protected override int CrateDustType => DustID.TsunamiInABottle;
    }
}
