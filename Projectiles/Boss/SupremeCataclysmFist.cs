using System;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SupremeCataclysmFist : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public ref float Time => ref Projectile.ai[0];
        public Vector2 shootVel;
        public int rotDirection = 1;
        public bool broIsAlive = true;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 54;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            // Difficulty modes
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // The Orb
            if (Projectile.ai[2] >= 3)
            {
                if (Main.zenithWorld)
                {
                    if (Projectile.timeLeft > 400)
                    {
                        shootVel = Projectile.velocity * 0.4f;
                        Projectile.timeLeft = 400;
                        rotDirection = Main.rand.NextBool() ? -1 : 1;
                        Projectile.velocity *= 6f;
                        broIsAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>());
                    }
                    else
                    {
                        if (Time > 10)
                            Projectile.tileCollide = true;
                        else
                            Projectile.velocity *= 0.99f;
                        Projectile.rotation += 0.02f;
                    }

                    if (Projectile.timeLeft == 1)
                    {
                        int points = 25;
                        float radians = MathHelper.TwoPi / points;
                        Vector2 spinningPoint = Vector2.Normalize(new Vector2(-4f, -4f));
                        for (int b = 0; b < 2; b++)
                        {
                            float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                            for (int k = 0; k < points; k++)
                            {
                                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);

                                Dust cataclysmdust = Dust.NewDustPerfect(Projectile.Center + velocity * (b == 0 ? 7 : 5), DustID.SilverFlame, velocity * (b == 0 ? 9 : 7));
                                cataclysmdust.noGravity = true;
                                cataclysmdust.scale = Main.rand.NextFloat(1.3f, 1.9f);
                                cataclysmdust.color = Color.Red;
                            }
                        }
                        Projectile.Kill();
                    }

                    Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                    Dust cataclysmdust2 = Dust.NewDustPerfect(Projectile.Center + vel * 2, DustID.SilverFlame, vel);
                    cataclysmdust2.noGravity = true;
                    cataclysmdust2.scale = Main.rand.NextFloat(0.9f, 1.2f);
                    cataclysmdust2.color = Color.Red;
                }
                else
                {
                    if (Projectile.timeLeft > 240)
                    {
                        shootVel = Projectile.velocity * 0.4f;
                        Projectile.timeLeft = 240;
                        rotDirection = Main.rand.NextBool() ? -1 : 1;
                        Projectile.velocity *= 5.5f;
                        broIsAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>());
                    }
                    else
                    {
                        float randSize = Main.rand.NextFloat(0.8f, 1.2f);
                        for (int i = 0; i < 2; i++)
                        {
                            Particle bloom = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.5f), "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.7f * randSize, 0f, 10);
                            GeneralParticleHandler.SpawnParticle(bloom);
                        }
                        Particle bloom3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.9f, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.4f * randSize, 0f, 10);
                        GeneralParticleHandler.SpawnParticle(bloom3);

                        Projectile.velocity *= 0.945f;
                    }

                    // Adds rotation, but it's SUPER unfair a lot of the time
                    //shootVel = shootVel.RotatedBy(Time * 0.00011f * rotDirection);


                    if (Projectile.timeLeft == 205) // Fist direction telegraphs
                    {
                        for (int k = 0; k < 28; k++)
                        {
                            Vector2 vel1 = -shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f);
                            GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center + vel1 * (9 + k * 11), vel1, false, 7, 0.17f, Color.Lerp(Color.Red, Color.Magenta, 0.5f) * 0.5f, new Vector2(0.9f, 0.4f), true, false);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                        if (broIsAlive == false)
                        {
                            for (int k = 0; k < 28; k++)
                            {
                                Vector2 vel2 = (-shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f)).RotatedBy(MathHelper.ToRadians(120f));
                                GlowSparkParticle spark2 = new GlowSparkParticle(Projectile.Center + vel2 * (9 + k * 11), vel2, false, 7, 0.17f, Color.Lerp(Color.Red, Color.Magenta, 0.5f) * 0.5f, new Vector2(0.9f, 0.4f), true, false);
                                GeneralParticleHandler.SpawnParticle(spark2);
                            }
                            for (int k = 0; k < 28; k++)
                            {
                                Vector2 vel3 = (-shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f)).RotatedBy(MathHelper.ToRadians(-120f));
                                GlowSparkParticle spark3 = new GlowSparkParticle(Projectile.Center + vel3 * (9 + k * 11), vel3, false, 7, 0.17f, Color.Lerp(Color.Red, Color.Magenta, 0.5f) * 0.5f, new Vector2(0.9f, 0.4f), true, false);
                                GeneralParticleHandler.SpawnParticle(spark3);
                            }
                        }
                    }

                    if (Projectile.timeLeft <= 200 && Time % 3 == 0)
                    {
                        Vector2 randPos = (-shootVel * 1.5f).RotatedByRandom(1) * Main.rand.NextFloat(5, 15);
                        int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                        SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound with { Volume = 1.2f, Pitch = 0.55f }, Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos, (-shootVel * MathHelper.Clamp(Time * 0.02f, 0.1f, 1.8f)) * Main.rand.NextFloat(0.75f, 1f), type, Projectile.damage, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                    }
                    if (Projectile.timeLeft <= 200 && Time % 3 == 0 && broIsAlive == false)
                    {
                        Vector2 randPos = (-shootVel * 1.5f).RotatedBy(MathHelper.ToRadians(120f)).RotatedByRandom(1) * Main.rand.NextFloat(5, 15);
                        Vector2 randPos2 = (-shootVel * 1.5f).RotatedBy(MathHelper.ToRadians(-120f)).RotatedByRandom(1) * Main.rand.NextFloat(5, 15);
                        int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos, (-shootVel * MathHelper.Clamp(Time * 0.02f, 0.1f, 1.8f)).RotatedBy(MathHelper.ToRadians(120f)) * Main.rand.NextFloat(0.75f, 1f), type, Projectile.damage, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos2, (-shootVel * MathHelper.Clamp(Time * 0.02f, 0.1f, 1.8f)).RotatedBy(MathHelper.ToRadians(-120f)) * Main.rand.NextFloat(0.75f, 1f), type, Projectile.damage, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                    }
                    if (Projectile.timeLeft == 1)
                    {
                        int points = 25;
                        float radians = MathHelper.TwoPi / points;
                        Vector2 spinningPoint = Vector2.Normalize(new Vector2(-4f, -4f));
                        for (int b = 0; b < 2; b++)
                        {
                            float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                            for (int k = 0; k < points; k++)
                            {
                                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);

                                Dust cataclysmdust = Dust.NewDustPerfect(Projectile.Center + velocity * (b == 0 ? 7 : 5), DustID.SilverFlame, velocity * (b == 0 ? 9 : 7));
                                cataclysmdust.noGravity = true;
                                cataclysmdust.scale = Main.rand.NextFloat(1.3f, 1.9f);
                                cataclysmdust.color = Color.Red;
                            }
                        }
                        Projectile.Kill();
                    }

                    Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                    Dust cataclysmdust2 = Dust.NewDustPerfect(Projectile.Center + vel * 2, DustID.SilverFlame, vel);
                    cataclysmdust2.noGravity = true;
                    cataclysmdust2.scale = Main.rand.NextFloat(0.9f, 1.2f);
                    cataclysmdust2.color = Color.Red;
                }
            }
            else if (Projectile.ai[2] <= 2)
            {
                // Rapid punches
                if (Projectile.ai[2] == 1)
                {
                    Projectile.extraUpdates = 2;
                }
                else // Regular punches
                    Projectile.velocity *= 1.0055f;

                // Orb mini fists
                if (Projectile.ai[2] == 0)
                {
                    Projectile.scale = 0.8f;
                    Projectile.extraUpdates = 2;
                    Projectile.Opacity = 1;
                    if (Projectile.timeLeft > 300)
                        Projectile.timeLeft = 300;
                    Projectile.velocity *= 1.006f;
                }

                if (Main.zenithWorld && Projectile.ai[2] >= 3)
                    return;

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
                }
            }

            // Decide frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];

            // Fade in and handle visuals.
            if (Projectile.ai[2] < 3)
                Projectile.Opacity = Utils.GetLerpValue(0f, 12f, Projectile.timeLeft, true) * Utils.GetLerpValue(1200f, 1193f, Projectile.timeLeft, true);
            
            Time++;

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) == false && !Main.zenithWorld)
            {
                Projectile.timeLeft = 1;
                for (int k = 0; k < 10; k++)
                {
                    Vector2 velocity = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.8f, 1.2f);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity, DustID.RainbowTorch, velocity * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.9f);
                    dust.color = Color.Lerp(Color.Red, Color.Magenta, 0.5f);
                }
            }

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.5f * Projectile.Opacity, 0f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.zenithWorld && Projectile.ai[2] >= 3)
            {
                Texture2D ballTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/Basketball").Value;
                Main.EntitySpriteDraw(ballTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, ballTexture.Size() * 0.5f, Projectile.scale / 1.8f, SpriteEffects.None, 0);
                return false;
            }

            lightColor.R = (byte)(255 * Projectile.Opacity);

            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            if (Projectile.ai[1] == 1f)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SupremeCataclysmFistAlt").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => (Projectile.Opacity >= 1f || Projectile.ai[2] >= 3);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundStyle b = new("CalamityMod/Sounds/Custom/Kickball");
            SoundEngine.PlaySound(b with { PitchVariance = 0.15f }, Projectile.Center);

            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, (Projectile.ai[2] >= 3 ? 90 : 35 * Projectile.scale), targetHitbox);
    }
}
