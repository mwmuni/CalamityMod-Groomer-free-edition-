namespace CalamityMod.Projectiles.Boss
{
    public class HolyFire2 : BaseProvidenceHolyFireProjectile
    {
        protected override int TrailPositionCount => 8;
        protected override float MaxBodyWidth => 16f;
        protected override float RenderTargetOpacity => 0.75f;
        protected override bool SetTrailCacheInDefaultsAndAI => true;
    }
}
