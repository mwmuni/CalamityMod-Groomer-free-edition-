using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TitaniumShurikenProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TitaniumShuriken";

        private static float RotationIncrement = 0.22f;
        private static float ReboundTime = 26f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
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
                        Projectile.ResetLocalNPCHitImmunity(); // Stealth uses -1 local and resets it when returning to guarantee it can hit again on the way back
                    }
                }
                else
                {
                    Projectile.tileCollide = false;
                    float returnSpeed = 16f;
                    float acceleration = 3.2f;
                    Player owner = Main.player[Projectile.owner];

                    // Delete the shuriken if it's excessively far away.
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
                        {
                            Projectile.Kill();
                        }
                }

                // Rotate the shuriken as it flies.
                Projectile.rotation += RotationIncrement;
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.BounceOnTileCollision(oldVelocity);
                return false;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            if (Projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            }
            else
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => SpawnStealthClones();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => SpawnStealthClones();
        private void SpawnStealthClones()
        {
            if (Projectile.Calamity().stealthStrike && Projectile.numHits < 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 randSpeed = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * Main.rand.NextFloat(0.4f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, randSpeed, ModContent.ProjectileType<TitaniumClone>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
