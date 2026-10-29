using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    [LegacyName("SunkenCrateTile")]
    public class EutrophicCrateTile : BaseFishingCrateTile<EutrophicCrate>
    {
        protected override Color MapColor => new(106, 218, 230);
        protected override int CrateDustType => DustID.TsunamiInABottle;
    }
}
