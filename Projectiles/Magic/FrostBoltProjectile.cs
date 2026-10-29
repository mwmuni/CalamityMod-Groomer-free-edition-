using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FrostBoltProjectile : ModProjectile, ILocalizedModType
    {
        public ref float time => ref Projectile.ai[0];
        public int wallBounces = 0;
        public float fadeIn = 0;
        public bool launch = true;
        public Color bColor = Color.DeepSkyBlue;
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.coldDamage = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, bColor.ToVector3());
            fadeIn = Utils.GetLerpValue(0, Owner.itemAnimationMax * 0.5f * Projectile.MaxUpdates, time, true);
            Vector2 velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * Owner.HeldItem.shootSpeed;
            Projectile.scale = fadeIn;
            if (fadeIn < 1) // Hold the projectile in front of the player while they case the spell
            {
                Projectile.Center = Owner.Center + velocity.SafeNormalize(Vector2.UnitX) * 48;
            }
            else
            {
                if (launch) // On launch, spawns some effects and launch the projectile at the mouse
                {
                    Projectile.tileCollide = true;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3) with { Volume = 0.45f, Pitch = 0.3f }, Projectile.Center);
                    for (int i = 0; i < 8; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.5f));
                        dust.noGravity = false;
                        dust.scale = Main.rand.NextFloat(0.65f, 1f);
                        dust.color = bColor;
                        dust.noLightEmittence = true;
                    }
                    Projectile.velocity = velocity;
                    launch = false;
                }
                
                if (Main.rand.NextBool(15))
                {
                    Particle fx = new CustomSpark(Projectile.Center + Main.rand.NextVector2Circular(8, 8), -Projectile.velocity * 0.3f, "CalamityMod/Particles/IceTypeParticle", false, 32, 0.9f, Color.Lerp(bColor, Color.White, 0.5f), new Vector2(0.8f, 1f), true, false);
                    GeneralParticleHandler.SpawnParticle(fx);
                }

                Particle trail = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.3f, "CalamityMod/Particles/BloomCircle", false, 7, 0.2f, bColor * 0.9f, new Vector2(1, 1f), true, false, shrinkSpeed: 0.8f);
                GeneralParticleHandler.SpawnParticle(trail);
            }
            // Gain gravity after a while
            // This makes it more useful on the surface where you otherwise have less tiles to work with
            if (Projectile.timeLeft < 260)
            {
                Projectile.velocity.X *= 0.9711f;
                if (Projectile.velocity.Y < 15)
                    Projectile.velocity.Y += 0.19f;
                if (Projectile.velocity.Y < 5)
                    Projectile.velocity.Y *= 0.977f;
                wallBounces = 2; // Will always expire on the next tile collide
            }
            else if (wallBounces > 1) // If you only have one bounce before death, reduce lifetime to start falling faster
                Projectile.timeLeft--;
            Projectile.rotation += 0.3f * Projectile.direction;
            time++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Allow bounced bolts to hit enemies already hit before the bounce
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;

            Projectile.BounceOnTileCollision(oldVelocity);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            // Create Blast
            float blastSize = 80;
            float minMultiplier = 0.25f;
            int hitsToMinMult = 4;
            int debuff = BuffID.Frostburn;
            int debuffTime = 120;
            Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
            blast.localAI[0] = debuff;
            blast.localAI[1] = debuffTime;
            blast.timeLeft = 15;
            blast.DamageType = DamageClass.Magic;

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f, Pitch = 0.3f, MaxInstances = -1 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f, Pitch = -0.3f, MaxInstances = -1 }, Projectile.Center);

            // "Snowflake" visual effect
            float rot = Main.rand.NextFloat(-2, 2);
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 6f).ToRotationVector2().RotatedBy(rot) * 6f;
                Particle trail = new CustomSpark(Projectile.Center + velocity * 3, velocity * 0.5f, "CalamityMod/Particles/IceTypeParticle", false, 25, 1.3f, Color.Lerp(bColor, Color.White, 0.5f), new Vector2(1, 1.8f), true, false, shrinkSpeed: -0.45f);
                GeneralParticleHandler.SpawnParticle(trail);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), velocity * 1.5f);
                dust.noGravity = true;
                dust.scale = 1.3f;
                dust.color = bColor;
                dust.noLightEmittence = true;
            }

            if (wallBounces >= 2)
            {
                Projectile.Kill();
            }
            wallBounces++;

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 60);
            // Since this reduces the damage of the projectile directly, the damage of its explosions is also decreased
            // This is intedned for Frost Bolt, but is not kept for the upgrades
            // Feel free to change this to use the same pierce reduction as its upgrades if this doesn't work out
            if (Projectile.damage > 1)
                Projectile.damage = (int)(Projectile.damage * 0.85f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProjectileWithBackglow(bColor with { A = 0 }, Color.White, 2f * fadeIn);
            return false;
        }
    }
}
