using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ButcherKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private static float RotationIncrement = 0.22f;
        private static float ReboundTime = 26f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            DrawOffsetX = -11;
            DrawOriginOffsetY = -10;
            DrawOriginOffsetX = 0;

            // ai[0] stores whether the knife is returning. If 0, it isn't. If 1, it is.
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= ReboundTime)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.tileCollide = false;
                float returnSpeed = 16f;
                float acceleration = 3.2f;
                Player owner = Main.player[Projectile.owner];

                // Delete the knife if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    Projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }

            // Rotate the knife as it flies.
            Projectile.rotation += RotationIncrement;
            return;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Laceration>(), 360);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Laceration>(), 360);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] += 0.1f;
            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
