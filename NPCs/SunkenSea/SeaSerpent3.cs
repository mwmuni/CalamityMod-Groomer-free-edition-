namespace CalamityMod.NPCs.SunkenSea
{
    public class SeaSerpent3 : SeaSerpentSegment
    {
        public override void SetDefaults()
        {
            SetSegmentDefaults(40, 24, 16, 20);
        }

        protected override string GoreName => "SeaSerpentGore3";
    }
}
