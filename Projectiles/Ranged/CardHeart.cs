using System;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardHeart : BaseCardProjectile
    {
        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Web, Projectile.oldVelocity.X * 0.15f, Projectile.oldVelocity.Y * 0.15f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ProjectileID.VampireHeal, (int)Math.Round(hit.Damage * 0.01), 0.4f);

    }
}
