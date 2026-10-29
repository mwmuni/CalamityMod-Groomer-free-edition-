using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalactiteSmall1 : CragSpikeBase
    {
        protected override int SpikeWidth => 2;
        protected override int SpikeHeight => 3;
        protected override Point16 SpikeOrigin => new Point16(1, 1);
        protected override int DrawOffset => -3;
        protected override bool HangsFromCeiling => true;
    }

    public class CragStalactiteSmall2 : CragStalactiteSmall1
    {
    }
}
