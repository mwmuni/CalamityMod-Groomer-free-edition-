using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class WaterShotBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<WaterShot>();
        protected override Vector2 SpawnOffset => new Vector2(3f, 0f);
    }
}
