using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class DraedonLaserBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<DraedonLaser>();
    }
}
