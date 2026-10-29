using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class FireShotBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<FireShot>();
    }
}
