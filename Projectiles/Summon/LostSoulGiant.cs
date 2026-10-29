using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Summon
{
    public class LostSoulGiant : LostSoulBase
    {
        protected override int ProjectileSize => 52;
        protected override float? ProjectileScale => 0.8f;
        protected override float TrailOffset => 1.5f;
        protected override float DeathDustPositionRadius => 60f;
        protected override float DeathDustVelocityRadius => 2f;

        public static void DoSoulAI(Projectile projectile, ref float time)
        {
            if (time >= 15f)
            {
                NPC potentialTarget = projectile.Center.MinionHoming(1800f, Main.player[projectile.owner], false);
                if (potentialTarget != null)
                    HomeInOnTarget(projectile, potentialTarget);
            }

            projectile.Opacity = Utils.GetLerpValue(0f, 15f, time, true);
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 4f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

            projectile.rotation = projectile.velocity.ToRotation();
            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;

            time++;
        }

        public static void HomeInOnTarget(Projectile projectile, NPC target)
        {
            float speed = MathHelper.Lerp(21f, 31f, (float)Math.Sin(projectile.identity % 7f / 7f * MathHelper.TwoPi) * 0.5f + 0.5f);
            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(target.Center) * speed, 0.075f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.IndianRed, Color.DarkViolet, Projectile.identity % 5f / 5f * 0.5f);
            Color color = Color.Lerp(baseColor * 1.5f, baseColor, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.7f) * 0.04f + 0.45f);
            color.A = 0;
            return color * Projectile.Opacity;
        }
    }
}
