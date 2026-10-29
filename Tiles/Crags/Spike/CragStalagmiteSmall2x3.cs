using Terraria.DataStructures;

namespace CalamityMod.Tiles.Crags.Spike
{
    public class CragStalagmiteSmall1 : CragSpikeBase
    {
        protected override int SpikeWidth => 2;
        protected override int SpikeHeight => 3;
        protected override Point16 SpikeOrigin => new Point16(1, 2);
        protected override int DrawOffset => 3;
        protected override bool HangsFromCeiling => false;
    }

    public class CragStalagmiteSmall2 : CragStalagmiteSmall1
    {
    }
}
