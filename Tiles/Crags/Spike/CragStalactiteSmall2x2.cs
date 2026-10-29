using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalactiteSmall3 : CragSpikeBase
    {
        protected override int SpikeWidth => 2;
        protected override int SpikeHeight => 2;
        protected override Point16 SpikeOrigin => new Point16(1, 1);
        protected override int DrawOffset => -3;
        protected override bool HangsFromCeiling => true;
        protected override bool UsesStyle2x2 => true;
    }
}
