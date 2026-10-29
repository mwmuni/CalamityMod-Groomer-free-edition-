namespace CalamityMod.Projectiles.Boss
{
    public class HolyBlastFrags : BaseProvidenceHolyFireProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyFire2";

        protected override int TrailPositionCount => 12;
        protected override float MaxBodyWidth => 38f;

        public override void SetStaticDefaults()
        {
            SetTrailCache();
            base.SetStaticDefaults();
        }
    }
}
