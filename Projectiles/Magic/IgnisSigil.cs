using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.Projectiles.Magic
{
    public class IgnisSigil : MagicSigilBase
    {
        protected override void FireSigilProjectile()
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

            Player owner = Main.player[Projectile.owner];
            Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

            HasSpawnedProjectile = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 50f, ModContent.ProjectileType<IgnisSigilFireball>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner);
        }
    }
}
