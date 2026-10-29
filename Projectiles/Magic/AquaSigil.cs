using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class AquaSigil : MagicSigilBase
    {
        protected override void FireSigilProjectile()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/AquaSigilShot") { Volume = 1.5f, PitchVariance = 0.1f }, Projectile.Center);

            Player owner = Main.player[Projectile.owner];
            Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

            HasSpawnedProjectile = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 32f, ModContent.ProjectileType<AquaSigilWaterball>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
        }
    }
}
