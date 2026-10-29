using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlashRoundProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public int bounces = 2;
        public float shineRot = 0;
        public bool onSpawn = true;
        public ref float time => ref Projectile.ai[2];
        private const int MaxTime = 600;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = MaxTime;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.3f);

            if (bounces < 2 || Projectile.numHits > 0)
            {
                int falloffTime = 7;
                if (time > falloffTime)
                    Projectile.velocity.X *= 0.983f;
                if (Projectile.velocity.Y < 15 && time > falloffTime)
                    Projectile.velocity.Y += 0.16f;
                if (Projectile.velocity.Y < 5)
                    Projectile.velocity.Y *= 0.98f;
            }
            if (onSpawn)
            {
                Projectile.knockBack = 0;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10;
                shineRot = Main.rand.NextFloat(-5, 5);
                onSpawn = false;
            }
            // Handles spawning flash effect when bouncing off enemies/tiles
            if (time == 0 && Projectile.timeLeft < MaxTime)
            {
                MakeFlash(false);
            }
            shineRot += Math.Sign(shineRot) * 0.05f;

            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (time > 2 && time % 2 == 0 && targetDist < 1400)
            {
                float trailSize = Utils.GetLerpValue(2, 5, Projectile.velocity.Length(), true);
                Particle trail = new CustomSpark(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.UnitX) * 0.5f, "CalamityMod/Particles/BloomCircle", false, 5, 0.13f, Color.White * 0.6f, new Vector2(1f - 0.2f * trailSize, 1 + 2f * trailSize), true, false, shrinkSpeed: 0.8f * trailSize);
                GeneralParticleHandler.SpawnParticle(trail);
            }
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 91 : 264, -Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.5f, 0.9f));
                dust.scale = Main.rand.NextFloat(0.55f, 0.7f);
                dust.noGravity = true;
            }
            time++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float damageMult = Projectile.numHits == 0 ? 1 : 0.3f;
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            time = 0;
            Projectile.velocity = Vector2.Lerp(Utils.DirectionFrom(Projectile.Center, target.Center) * 12, Vector2.UnitY * -7, 0.75f).RotatedByRandom(0.25f);
            Projectile.netUpdate = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            time = 0;
            Projectile.BounceOnTileCollision(oldVelocity);
            if (Projectile.velocity.X < 2 && Projectile.velocity.X > 0)
                Projectile.velocity.X = 2;
            if (Projectile.velocity.X > -2 && Projectile.velocity.X < 0)
                Projectile.velocity.X = -2;
            if (Projectile.velocity.Y < 2 && Projectile.velocity.Y > 0)
                Projectile.velocity.Y = 2;
            if (Projectile.velocity.Y > -2 && Projectile.velocity.Y < 0)
                Projectile.velocity.Y = -2;

            Projectile.velocity *= 0.97f;

            int expectedDamage = Math.Max((int)(Projectile.damage * 1.13f), Projectile.damage + 2);
            Projectile.damage = expectedDamage;

            if (bounces <= 0)
            {
                Projectile.Kill();
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            bounces--;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float sine = (float)Math.Sin(Projectile.timeLeft * 0.175f / MathHelper.Pi);
            float trueSine = MathHelper.Lerp(sine, Math.Sign(sine) * 0.5f, 0.6f);
            Asset<Texture2D> trail = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineSoftEdge");
            Asset<Texture2D> shine = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");

            Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.6f, Projectile.rotation + shineRot, shine.Size() / 2f, new Vector2(1 + 0.9f * trueSine, 1 + 0.9f * -trueSine) * Projectile.scale * 0.13f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.6f, Projectile.rotation + shineRot, shine.Size() / 2f, new Vector2(1 + 0.9f * -trueSine, 1 + 0.9f * trueSine) * Projectile.scale * 0.13f, SpriteEffects.None, 0);
            return false;
        }

        public void MakeFlash(bool onlyFlash) // This is basically the tile/hit effects
        {
            if (!onlyFlash)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 91 : 264, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.6f) * Main.rand.NextFloat(8.5f, 12f));
                    dust.scale = Main.rand.NextFloat(0.75f, 1.1f);
                    dust.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item50 with { Volume = 0.3f, Pitch = -0.2f + 0.4f * bounces, MaxInstances = 15 }, Projectile.Center);
            }
            for (int i = 0; i < 2; i++)
            {
                Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.2f * (i), 0.45f * (i), 9, true, makeLight: 0);
                GeneralParticleHandler.SpawnParticle(blastRing);
            }
            float rot = Main.rand.NextFloat(-5, 5);
            for (int i = 0; i < 5; i++)
            {
                Vector2 dustVel = ((MathHelper.TwoPi * i / 5f) + rot).ToRotationVector2() * 5;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), dustVel);
                dust.scale = 1.1f;
                dust.fadeIn = 1.5f;
                dust.noGravity = true;
                dust.color = Color.White;
                dust.noLightEmittence = true;
            }
            // Doesn't actually do damage, just an aoe for knockback
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlashRoundFlash>(), 5, 0f, Projectile.owner);
        }
    }
}
