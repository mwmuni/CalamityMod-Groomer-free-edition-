using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Magic
{
    public class PerditoSigil : MagicSigilBase
    {
        protected override void FireSigilProjectile()
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<PerditoSigilShotCreator>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            HasSpawnedProjectile = true;
        }
    }
}
