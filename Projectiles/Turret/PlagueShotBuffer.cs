using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class PlagueShotBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<PlagueShot>();
    }
}
