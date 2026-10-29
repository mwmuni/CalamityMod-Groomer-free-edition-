using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.Typeless
{
    public class RedLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/RedSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.HellLabCenter;
        protected override Color DustColor => Color.Red;

        public static void Behavior(Projectile projectile, Vector2 destination, Color dustColor, ref float time)
        {
            projectile.ai[1] = 0f;

            if (time < 45f)
                projectile.velocity = Vector2.UnitY * Utils.GetLerpValue(0f, 25f, time, true) * Utils.GetLerpValue(30f, 25f, time, true) * -4.5f;
            else if (time < 80f)
                projectile.rotation = (projectile.AngleTo(destination) + MathHelper.PiOver2) * Utils.GetLerpValue(45f, 70f, time, true);
            else
            {
                if (projectile.WithinRange(destination, 420f))
                {
                    projectile.ai[1] = 1f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.UnitY * (float)Math.Sin((time - 80f) / 24f) * 2.5f, 0.15f);

                    if (!projectile.WithinRange(destination, 30f))
                        projectile.Center = projectile.Center.MoveTowards(destination, 10f);
                    projectile.rotation *= 0.95f;
                    projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.1f);
                }
                // Release lingering dust.
                else
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(8f, 8f), DustID.RainbowMk2);
                    dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    dust.noGravity = true;
                    dust.color = dustColor;
                    dust.scale = 1.6f;
                    dust.fadeIn = 1.45f;
                }
            }

            if (time > 80f && time < 120f)
                projectile.velocity = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Utils.GetLerpValue(80f, 105f, time, true) * 25f;

            // Reset afterimages before the charge.
            if (time < 80f)
                projectile.oldPos = new Vector2[projectile.oldPos.Length];

            Lighting.AddLight(projectile.Center, Vector3.One * 0.8f);
            time++;
        }
    }
}
