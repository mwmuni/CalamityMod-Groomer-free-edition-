using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public class OnyxShotBuffer : BaseTurretShotBuffer
    {
        protected override int ShotType => ModContent.ProjectileType<OnyxShot>();
        protected override Vector2 SpawnOffset => new Vector2(3f, 0f);

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item36 with { Volume = 0.65f }, Projectile.Center);

                Projectile.localAI[0] = 1f;
            }
            base.AI();
        }

        protected override void SpawnShots(IEntitySource source)
        {
            for (int i = -1; i < 2; i++)
                Projectile.NewProjectile(source, Projectile.Center + SpawnOffset, Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.035f, 0.11f) * i), ShotType, Projectile.damage, Projectile.knockBack, Main.myPlayer);
        }
    }
}
