using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class TerraSigil : MagicSigilBase
    {
        protected override bool IsTimeToFire => Projectile.localAI[0] >= FireTime;

        protected override void FireSigilProjectile()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot") { Volume = 0.45f, PitchVariance = 0.1f }, Projectile.Center);

            Player owner = Main.player[Projectile.owner];
            Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

            HasSpawnedProjectile = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 32f, ModContent.ProjectileType<TerraSigilLargeRock>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
        }
    }
}
