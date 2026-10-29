using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class WaterSpout : BaseSpoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        protected override int HitCooldown => 10;
    }
}
