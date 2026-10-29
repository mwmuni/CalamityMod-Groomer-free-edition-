using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Ranged;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class OrdoSigil : MagicSigilBase
    {
        protected override void FireSigilProjectile()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/OntologicalDespoilerLargeShot") { Volume = 0.5f, PitchVariance = 0.1f }, Projectile.Center);

            Player owner = Main.player[Projectile.owner];

            owner.SetScreenshake(3f);
            Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

            HasSpawnedProjectile = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 18f, ModContent.ProjectileType<OrdoSigilBeam>(), Projectile.damage * 6, Projectile.knockBack, Projectile.owner);
        }
    }
}
