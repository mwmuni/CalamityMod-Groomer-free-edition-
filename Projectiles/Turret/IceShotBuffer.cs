using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class IceShotBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<IceShot>();
    }
}
