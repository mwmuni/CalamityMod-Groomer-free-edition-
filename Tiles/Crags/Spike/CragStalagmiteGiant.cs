using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalagmiteGiant1 : CragSpikeBase
    {
        protected override int SpikeWidth => 4;
        protected override int SpikeHeight => 8;
        protected override Point16 SpikeOrigin => new Point16(2, 7);
        protected override int DrawOffset => 3;
        protected override bool HangsFromCeiling => false;
    }

    public class CragStalagmiteGiant2 : CragStalagmiteGiant1
    {
    }

    public class CragStalagmiteGiant3 : CragStalagmiteGiant1
    {
    }
}
