using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalactiteGiant1 : CragSpikeBase
    {
        protected override int SpikeWidth => 4;
        protected override int SpikeHeight => 8;
        protected override Point16 SpikeOrigin => new Point16(2, 1);
        protected override int DrawOffset => -3;
        protected override bool HangsFromCeiling => true;
    }

    public class CragStalactiteGiant2 : CragStalactiteGiant1
    {
    }

    public class CragStalactiteGiant3 : CragStalactiteGiant1
    {
    }
}
