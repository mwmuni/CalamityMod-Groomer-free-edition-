using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalactiteLarge1 : CragSpikeBase
    {
        protected override int SpikeWidth => 2;
        protected override int SpikeHeight => 5;
        protected override Point16 SpikeOrigin => new Point16(1, 1);
        protected override int DrawOffset => -3;
        protected override bool HangsFromCeiling => true;
    }

    public class CragStalactiteLarge2 : CragStalactiteLarge1
    {
    }
}
