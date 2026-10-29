using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float LaserLength = 40f;
        private const float LaserLengthChangeRate = 1.5f;
        public bool HasBounced = false;
        public int bounceTimer = 0;

        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            // Very rapidly fade into existence.
            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0f, 0.7f, 0.1f);

            // Laser length shenanigans. If the laser is still growing, increase localAI 0 to indicate it is getting longer.
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += LaserLengthChangeRate;

                // Cap it at max length.
                if (Projectile.localAI[0] > LaserLength)
                    Projectile.localAI[0] = LaserLength;
            }

            // Otherwise it's shrinking. Once it reaches zero length it dies for good.
            else
            {
                Projectile.localAI[0] -= LaserLengthChangeRate;
                if (Projectile.localAI[0] <= 0f)
                    Projectile.Kill();
            }

            if (Projectile.timeLeft % 2 == 0)
            {
                Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark", false, 4, 0.02f, Color.Lime, new Vector2(0.6f, 1.4f), true, false, shrinkSpeed: 0.95f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            if (bounceTimer > 0)
                bounceTimer--;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounceTimer = 10;
            for (int i = 0; i < 2; i++)
            {
                Color color = i == 0 ? Color.White : Color.Lime;
                Particle orb2 = new CustomSpark(Projectile.Center, oldVelocity * 0.1f, "CalamityMod/Particles/BloomCircle", false, 15, Main.rand.NextFloat(0.15f, 0.25f) * (i == 0 ? 2 : 3), color, new Vector2(0.7f, 1.1f), true, false);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.8f, 2.5f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.45f, 1.1f);
                dust.color = Color.Lime;
                dust.noLightEmittence = true;
            }

            Player owner = Main.player[Projectile.owner];

            if (!HasBounced)
            {
                HasBounced = true;

                float npcDistCheck = 640f; // 40 tiles
                int index = -1;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.CanBeChasedBy(Projectile))
                        continue;

                    float currentNPCDist = Vector2.Distance(n.Center, owner.ClampedMouseWorld());
                    if (currentNPCDist < npcDistCheck)
                    {
                        npcDistCheck = currentNPCDist;
                        index = n.whoAmI;
                    }
                }
                // If the index is not default, smart bounce in the direction of that enemy.
                if (index != -1)
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTargetMaxUpdates(Projectile.Center, Main.npc[index], owner.HeldItem.shootSpeed, 3);
                }
                else // Otherwise, use standard bouncing behavior.
                {
                    Projectile.BounceOnTileCollision(oldVelocity);
                }

                // The laser loses 20% damage after bouncing.
                Projectile.damage = (int)(Projectile.damage * 0.8f);
                return false;
            }
            else
                return true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(96, 255, 96, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            if (bounceTimer == 0)
                Projectile.DrawBeam(LaserLength, 2f, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Particle spark2 = new CustomSpark(Projectile.Center, Projectile.velocity * 0.3f, "CalamityMod/Particles/GlowSpark", false, 14, 0.018f, Color.Lime, new Vector2(1f, 1f), true, false, shrinkSpeed: 0.65f);
            GeneralParticleHandler.SpawnParticle(spark2);
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.4f, 0.9f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.45f, 1.1f);
                dust.color = Color.Lime;
                dust.noLightEmittence = true;
            }
        }
    }
}
