using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class OblivionYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Oblivion>();
        public const int MaxUpdates = 2;
        //How many hits the yoyo has gotten. Starts at 1 to avoid a dividing by zero error
        public int hitCounter = 1;
        //20 hits to maximum
        public int hitCountMax = 21;
        public int time = 0;
        //Used for visual intensity rising as the yoyo gets more hits
        public float power = 0;
        public float powerMax = 1;
        //The grace period after the yoyo reaches max power to avoid insta-shattering
        public int graceTimer = 0;
        public bool shatter = false;
        //Yoyo bag behavior
        public bool cloneYoyo = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Oblivion.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Oblivion.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 15 * MaxUpdates;
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            //Using yoyo glove or yoyo bag makes yoyos run out of air time 2-4x faster (randomized) when the second yoyo is out
            //Therefore, we set the flight to infinite, but manually set it to recall after 15 seconds and update it across both yoyos
            if (time >= 1800)
            {
                if (!cloneYoyo)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.active && p.type == Type && p.owner == Projectile.owner)
                        {
                            if (p.ModProjectile is OblivionYoyo oblivion)
                            {
                                oblivion.Projectile.ai[0] = -1f;
                            }
                        }
                    }
                }
            }
            // Determine if the yoyo is a yoyo bag/yoyo glove clone
            if (!cloneYoyo)
            {
                int MainYoyo = -1;
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile proj = Main.projectile[x];
                    if (proj.active && proj.type == Type && proj.owner == Projectile.owner)
                    {
                        MainYoyo = x;
                        break;
                    }
                }
                if (Projectile.whoAmI != MainYoyo)
                    cloneYoyo = true;
                //Yoyo behavior. Nothing will trigger on yoyos from yoyo bag

                //Fireball spawning. Fire rate increases with how many hits the yoyo has gotten
                if (time % (120 / hitCounter) == 0 && !shatter && Projectile.owner == Main.myPlayer)
                {
                    float randRot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    Vector2 spawnVel = randRot.ToRotationVector2() * 2f;
                    Projectile flames = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, spawnVel, ModContent.ProjectileType<OblivionFire>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack, Projectile.owner);
                }
                if (graceTimer > 0)
                    graceTimer--;
                if (shatter)
                {
                    //Constantly keep the yoyo out, even if the player lets go of the mouse
                    Projectile.ai[0] = 0f;
                    Projectile.Center = player.Calamity().mouseWorld;
                    //Charge up visuals
                    power += 0.01f;
                    float fade = Utils.GetLerpValue(powerMax + 2, 0, power);
                    float numberOfDusts = 2f;
                    float rotFactor = 360f / numberOfDusts;
                    for (int i = 0; i < numberOfDusts; i++)
                    {
                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 250f, 0.04f);
                        velOffset *= Main.rand.NextFloat(25, 45) * fade;
                        Particle energy = new GlowOrbParticle(Projectile.Center + velOffset * 2.5f, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, false, (int)(14 - (5 * fade)), Main.rand.NextFloat(1.1f, 1.25f) - 0.5f * fade, Main.rand.NextBool() ? Color.Red : Color.MediumVioletRed);
                        GeneralParticleHandler.SpawnParticle(energy);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + velOffset * 2.5f, DustID.FireworksRGB, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, 0, default, Main.rand.NextFloat(0.6f, 0.8f));
                        dust.noGravity = true;
                        dust.color = Color.DarkRed;
                    }
                    if (time == 240)
                    {
                        #region Visuals and Sounds
                        Player Owner = Main.player[Projectile.owner];
                        for (int i = 0; i < 2; i++)
                        {
                            Particle centerVoid = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), (2.2f + i * 0.5f), 0f, 80, false);
                            GeneralParticleHandler.SpawnParticle(centerVoid);
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f, 3f, 25, true);
                            GeneralParticleHandler.SpawnParticle(blastRing);
                            Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.MediumVioletRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.5f, 1.5f, 25, true);
                            GeneralParticleHandler.SpawnParticle(blastRing2);
                        }
                        Particle backBlast1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.MediumVioletRed, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 2.56f * 1.3f, 18);
                        GeneralParticleHandler.SpawnParticle(backBlast1, false, GeneralDrawLayer.AfterEverything);

                        Particle backBlast2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.MediumVioletRed, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.3f, 30);
                        GeneralParticleHandler.SpawnParticle(backBlast2, false, GeneralDrawLayer.AfterEverything);

                        Particle backBlast3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.DarkRed * 0.55f, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.3f * 1.35f, 30);
                        GeneralParticleHandler.SpawnParticle(backBlast3, false, GeneralDrawLayer.AfterEverything);

                        Particle ring1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/FlameExplosion2", new Vector2(0.01f, 0.085f), MathHelper.PiOver2, -4f, 4f, 30);
                        GeneralParticleHandler.SpawnParticle(ring1);

                        Particle ring2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/FlameExplosion", new Vector2(0.02f, 0.07f), 0f, -4f, 4f, 25);
                        GeneralParticleHandler.SpawnParticle(ring2);

                        Particle ring3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/FlameExplosion", new Vector2(0.02f, 0.07f), MathHelper.PiOver4, -4f, 4f, 25);
                        GeneralParticleHandler.SpawnParticle(ring3);

                        Particle ring4 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/FlameExplosion", new Vector2(0.02f, 0.07f), -MathHelper.PiOver4, -4f, 4f, 25);
                        GeneralParticleHandler.SpawnParticle(ring4);

                        for (int i = 0; i < 19; i++)
                        {
                            int dustStyle = ModContent.DustType<VoidDustInverted>();
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, dustStyle);
                            dust.scale = Main.rand.NextFloat(1.4f, 2.2f);
                            dust.velocity = new Vector2(21, 21).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1f);
                            dust.noGravity = true;
                            dust.color = Color.MediumVioletRed;
                        }
                        for (int i = 0; i < (25); i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                            dust.noGravity = false;
                            dust.velocity = new Vector2(18, 18).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1f);
                            dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                            dust.color = Color.DarkRed;
                        }
                        Owner.SetScreenshake(6f);
                        SoundStyle boom = new("CalamityMod/Sounds/Custom/CalamitasClone/BulletHellEnd");
                        SoundEngine.PlaySound(boom with { Volume = 1.5f }, Projectile.Center);
                        #endregion
                        if (Projectile.owner == Main.myPlayer)
                        {
                            float blastSize = 240;
                            float minMultiplier = 0.25f;
                            int hitsToMinMult = 3;
                            int debuff1 = ModContent.BuffType<BrimstoneFlames>();
                            int debufftime = 180;
                            //I promise this is balanced
                            Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 50f), Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                            blast.localAI[0] = debuff1;
                            blast.localAI[1] = debufftime;
                            blast.timeLeft = 2;
                            blast.DamageType = Projectile.DamageType;
                            blast.CritChance = 100;
                        }

                        //Set cooldown
                        player.Calamity().oblivionCooldown = 300;
                        int duration = CalamityUtils.SecondsToFrames(5);
                        player.AddCooldown(OblivionCooldown.ID, duration, true);

                        //We manually kill the projectile to skip the retracting, to make it look like the yoyo is destroyed
                        //This needs to happen for both the main and secondary yoyo
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.type == Type && p.owner == Projectile.owner)
                            {
                                p.Kill();
                            }
                        }
                    }
                }
            }
            //If the player gets more than 200 blocks away, kill the yoyo
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f)
                Projectile.Kill();
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            //Only applies to the main yoyo
            if (!cloneYoyo)
            {
                hitCounter++;
                if (!shatter)
                {
                    power += 0.05f;
                    SoundStyle onHit = new("CalamityMod/Sounds/Item/TearsOfHeavenUse");
                    SoundEngine.PlaySound(onHit, Projectile.Center);
                    if (power > powerMax)
                    {
                        power = powerMax;
                    }
                }
                if (hitCounter == hitCountMax)
                {
                    //Refresh duration on both yoyos
                    for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.type == Type && p.owner == Projectile.owner)
                            {
                                if (p.ModProjectile is OblivionYoyo oblivion)
                                {
                                    oblivion.time = 0;
                                }
                            }
                        }
                    //Start a 1.5 second grace period where the yoyo cannot damage enemies. This is to avoid instant shattering after reaching max power
                    graceTimer = 90;
                    //Small burst and a ring of darts
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int totalProjectiles = 18;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        float velocity = 8f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2, ModContent.ProjectileType<OblivionBolt>(), Projectile.damage, 0f, Projectile.owner, 0f, Projectile.ai[1], velocity * 1.5f);
                        }
                        float blastSize = 120;
                        float minMultiplier = 0.25f;
                        int hitsToMinMult = -1;
                        int debuff1 = ModContent.BuffType<BrimstoneFlames>();
                        int debufftime = 180;
                        Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 5f), Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                        blast.localAI[0] = debuff1;
                        blast.localAI[1] = debufftime;
                        blast.timeLeft = 2;
                        blast.DamageType = Projectile.DamageType;
                    }
                    #region Visuals and Sounds
                    SoundStyle boom = new("CalamityMod/Sounds/Custom/CalamitasClone/CalClone_BigFireballBit", 4);
                    SoundEngine.PlaySound(boom with { Volume = 1.5f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1f, Pitch = -0.5f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.7f, Pitch = 1f }, Projectile.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        Particle explosion = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.MediumVioletRed, Utils.GetLerpValue(0, 5, i, true)), "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0f, 0.12f + 0.005f * i, (int)(20 - i * 1.5f));
                        GeneralParticleHandler.SpawnParticle(explosion);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Particle explosion = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.DarkRed, Color.DarkOrchid, Utils.GetLerpValue(0, 3, i, true)), "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0f, 0.12f + 0.01f * i, (int)(20 - i * 2f));
                        GeneralParticleHandler.SpawnParticle(explosion);
                    }
                    Particle outerGlow = new CustomPulse(Projectile.Center, Vector2.Zero, Color.DarkRed, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.2f, 2f, 24);
                    GeneralParticleHandler.SpawnParticle(outerGlow);
                    Particle innerGlow = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.1f, 1f, 24);
                    GeneralParticleHandler.SpawnParticle(innerGlow);
                    #endregion
                }
                //If the yoyo hits while at maximum, reset the duration of both yoyos and trigger shattering
                if (hitCounter > hitCountMax && !shatter)
                {
                    Player Owner = Main.player[Projectile.owner];
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 4f }, Projectile.Center);
                    Owner.SetScreenshake(3f);
                    shatter = true;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.active && p.type == Type && p.owner == Projectile.owner)
                        {
                            if (p.ModProjectile is OblivionYoyo oblivion)
                            {
                                oblivion.time = 0;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);

        //During the grace period after reaching max power, the yoyo cannot hit enemies for 1.5s to avoid unfair shattering
        public override bool? CanDamage() => graceTimer == 0;

        //Avoids instantly spawning a fireball
        public override void OnSpawn(IEntitySource source)
        {
            time = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D shineTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;

            float randSize = Main.rand.NextFloat(0.9f, 1.1f);
            float rotationAngle = MathHelper.PiOver4;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            //Only applies to the main yoyo
            if (!cloneYoyo)
            {
                //Red backglow, scales in size with power
                Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.Red with { A = 0 }, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.3f * randSize * power, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.MediumVioletRed with { A = 0 } * 0.65f, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.2f * randSize * power, SpriteEffects.None, 0);

                //X, scales in size with power
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Red with { A = 0 }, rotationAngle, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 2.75f * randSize * power, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Purple with { A = 0 } * 0.65f, rotationAngle, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 2.25f * randSize * power, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Red with { A = 0 }, rotationAngle + MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 2.75f * randSize * power, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Purple with { A = 0 } * 0.65f, rotationAngle + MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 2.25f * randSize * power, SpriteEffects.None, 0);

                //Yoyo drawing
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);

                //Red cross, doesn't scale in size unless shattering
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.DarkRed with { A = 0 }, 0, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 1.35f * randSize * (shatter ? power : 1f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Purple with { A = 0 } * 0.65f, 0, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 1.35f * randSize * (shatter ? power : 1f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.DarkRed with { A = 0 }, MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 1.35f * randSize * (shatter ? power : 1f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Purple with { A = 0 } * 0.65f, MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 1.35f * randSize * (shatter ? power : 1f), SpriteEffects.None, 0);

                //Black backglow, scales in size with power
                for (int i = 0; i < 2; i++)
                {
                    Particle backglow = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.25f * (power * 2), 0f, 10, false);
                    GeneralParticleHandler.SpawnParticle(backglow);
                    backglow.DrawLayer = Enums.GeneralDrawLayer.BeforeProjectiles;
                    backglow.DrawLayer = Enums.GeneralDrawLayer.BeforeNPCs;
                }
            }
            else
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
