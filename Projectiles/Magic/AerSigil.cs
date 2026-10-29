using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class AerSigil : MagicSigilBase
    {
        protected override void FireSigilProjectile()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/VividClarityBeamAppear") { Volume = 0.5f, PitchVariance = 0.1f }, Projectile.Center);

            Player owner = Main.player[Projectile.owner];
            Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

            HasSpawnedProjectile = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 36f, ModContent.ProjectileType<AerSigilMissile>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
        }
    }
}
