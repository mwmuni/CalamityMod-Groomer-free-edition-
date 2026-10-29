using System;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class KylieBoomerang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Kylie";

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        // Used for the stealth strike
        public int TileBounceDelay = 0;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            //Constant rotation
            Projectile.rotation += 0.2f;
            
            //Dust trail
            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0f);
                Main.dust[d].position = Projectile.Center;
            }
            //Constant sound effects
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 15;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // If stealth strike, stay close to the player's cursor.
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.tileCollide = true;
                if (TileBounceDelay > 0)
                    TileBounceDelay--;

                Vector2 mousePos = Owner.ClampedMouseWorld();
                if (Vector2.Distance(Projectile.Center, mousePos) > 115f && TileBounceDelay == 0)
                {
                    float accelerationFactor = 12f; // Higher number = Takes longer to turn around.
                    Projectile.velocity += (mousePos - Projectile.Center).SafeNormalize(Vector2.UnitX) * Kylie.Speed / accelerationFactor;
                    if (Projectile.velocity.Length() > Kylie.Speed)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= Kylie.Speed;
                    }
                }
                else // Accelerate to top speed if close enough to prevent it staying slow if you move in the cursor while it's slowing down.
                {
                    Projectile.velocity *= 1.1f;
                    if (Projectile.velocity.Length() > Kylie.Speed)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= Kylie.Speed;
                    }
                }
            }
            else
            {
                // State of 0 = Going out. State of 1 = Returning.
                if (State == 0f)
                {
                    Timer += 1f;

                    //Slopes REEEEEEEEEEEE
                    if (Timer == 3f)
                        Projectile.tileCollide = true;

                    if (Timer >= 35f) // Return to the player
                    {
                        State = 1f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.tileCollide = false;
                    float returnSpeed = Kylie.Speed * 1.5f;
                    float acceleration = 3.2f;
                    Player owner = Main.player[Projectile.owner];

                    // Delete the boomerang if it's excessively far away.
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
            }
        }

        // Return to player after hitting an enemy
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => State = 1f;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and return to player after hitting a tile
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.BounceOnTileCollision(oldVelocity);
            State = 1f;
            if (TileBounceDelay == 0)
                TileBounceDelay = 10;
            return false;
        }

        // Spawn splinter gores on stealth strike death
        public override void OnKill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Vector2 splinterVel = Projectile.velocity.RotatedByRandom(MathHelper.Pi / 12f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, splinterVel, Mod.Find<ModGore>("KylieGore1").Type);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, splinterVel, Mod.Find<ModGore>("KylieGore2").Type);
            }
        }
    }
}
