namespace CalamityMod.NPCs.SunkenSea
{
    public class SeaSerpent2 : SeaSerpentSegment
    {
        public float speed = 6f;
        public float turnSpeed = 0.125f;

        public override void SetDefaults()
        {
            SetSegmentDefaults(40, 24, 16, 20);
        }

        protected override string GoreName => "SeaSerpentGore2";
    }
}
