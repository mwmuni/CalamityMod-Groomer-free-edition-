using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Magic
{
    public class WarpSigil : MagicSigilBase
    {
        private bool spawnedIntroParticle = false;

        protected override float FireTime => 25f;
        protected override float KillTime => 70f;
        protected override float MaskRevealTime => 18f;
        protected override float FadeStartTime => 55f;
        protected override float FadeEndTime => 70f;
        protected override bool CopiesParentScale => false;
        protected override bool IsTimeToFire => Projectile.localAI[0] >= FireTime;

        protected override void FireSigilProjectile()
        {
            // ai[1] and ai[2] will inherit this sigil's position and timeLeft as its own
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<WarpSigilShotCreator>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 0, Projectile.whoAmI, Projectile.timeLeft);
            HasSpawnedProjectile = true;
        }

        protected override void DoAdditionalAI()
        {
            if (!spawnedIntroParticle)
                for (int j = 0; j < 13; j++)
                {
                    spawnedIntroParticle = true;
                    GeneralParticleHandler.SpawnParticle(new SquishyLightParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(3f, 7f), 0f).RotatedByRandom(MathHelper.TwoPi), Projectile.scale * Main.rand.NextFloat(0.2f, 0.475f), Main.rand.NextBool() ? Color.Magenta : Color.White, Main.rand.Next(12, 24), 1f, 0f, 1f));
                }
        }
    }
}
