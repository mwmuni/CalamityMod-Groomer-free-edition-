using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardSpade : BaseCardProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Web, Projectile.oldVelocity.X * 0.15f, Projectile.oldVelocity.Y * 0.15f);
            }
        }

    }
}
