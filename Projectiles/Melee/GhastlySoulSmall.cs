using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Projectiles.Melee
{
    public class GhastlySoulSmall : GhastlySoulBase, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        protected override int TimeLeft => 540;
        protected override float SoulVelocityMultiplier => VoidEdge.SmallSoulStatMultiplier;
        protected override float HomingDistance => 600f;

        public override void OnKill(int timeLeft)
        {
            Projectile.damage = (int)(Projectile.damage / VoidEdge.TotalProjectilesPerSwing);
            Projectile.penetrate = -1;
            Projectile.ExpandHitboxBy(80);
            Projectile.Damage();
            SoundEngine.PlaySound(VoidEdge.ProjectileDeathSound with { Pitch = 0.3f }, Projectile.Center);

            int points = 15;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
            float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);
                LineParticle subTrail = new LineParticle(Projectile.Center + velocity * 10.5f, velocity * 5, false, 20, 0.55f, Color.Plum);
                GeneralParticleHandler.SpawnParticle(subTrail);
            }
            for (int k = 0; k < 7; k++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch, new Vector2(6, 6).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.8f));
                dust2.scale = Main.rand.NextFloat(0.65f, 0.85f);
                dust2.noGravity = true;
                dust2.color = Color.Plum;
            }
        }
    }
}
