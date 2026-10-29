using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardClub : BaseCardProjectile
    {
        public override void OnKill(int timeLeft)
        {
            for (int i = -8; i <= 8; i += 8)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CardClubSplit>(), Projectile.damage / 2, 0f, Projectile.owner, 0f, 0f);
            }
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);
        }

    }
}
