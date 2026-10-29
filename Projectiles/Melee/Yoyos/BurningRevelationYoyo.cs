using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class BurningRevelationYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<BurningRevelation>();
        public const int MaxUpdates = 3;
        public bool canDamage = true;
        public bool firing = false;
        public NPC targeted;
        public float fade = 0;
        public int hitCooldown = 0;

        public int timer = 0;
        public int yoyoPower = 0;
        public int yoyoPowerMax = 1000;

        public bool cloneYoyo = false;
        public bool setCloneDamage = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = BurningRevelation.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = BurningRevelation.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 18 * MaxUpdates;
        }

        public override void AI()
        {
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
            }
            if (!setCloneDamage && cloneYoyo)
            {
                // Since the effects of the yoyo itself are nerfed if it's a clone, it gets a small damage bonus to compensate a bit
                Projectile.damage = (int)(Projectile.damage * 1.1f);
                setCloneDamage = true;
            }
            Player Owner = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 0.9f);

            // Fading in and out for various visuals
            fade = MathHelper.Lerp(fade, (firing ? 1 : 0), 0.03f);

            if (firing)
            {
                if (targeted == null || targeted.life <= 0)
                    targeted = Projectile.Center.ClosestNPCAt(800f);

                if (timer % (cloneYoyo ? 20 : 10) == 0) // Fire Holy Stars
                {
                    int stardamage = (int)(Projectile.damage * 0.24f);

                    Vector2 vel = new Vector2(0, 10).RotatedBy(timer * 0.025f * (cloneYoyo ? -1 : 1));
                    Projectile damageStar = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<HolyStarDamage>(), stardamage, Projectile.knockBack, Projectile.owner, 0, 5, targeted == null ? -1 : targeted.whoAmI);
                    damageStar.extraUpdates = 1;
                    damageStar.scale = 0.5f;
                    Projectile damageStar2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -vel, ModContent.ProjectileType<HolyStarDamage>(), stardamage, Projectile.knockBack, Projectile.owner, 0, 5, targeted == null ? -1 : targeted.whoAmI);
                    damageStar2.extraUpdates = 1;
                    damageStar2.scale = 0.5f;

                    SoundStyle fire = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
                    SoundEngine.PlaySound(fire with { Volume = 0.6f, PitchVariance = 0.2f }, Projectile.Center);
                }

                if (timer > 0)
                {
                    timer--;
                    if (timer <= 0)
                    {
                        firing = false;
                        canDamage = true;
                        yoyoPower = 0;
                    }
                }
            }
            else
            {
                if (yoyoPower >= yoyoPowerMax)
                {
                    SoundStyle fireHeal = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRay");
                    SoundEngine.PlaySound(fireHeal with { Volume = 0.9f, Pitch = 0.3f }, Projectile.Center);
                    canDamage = false;
                    firing = true;
                    timer = 240;
                }
                if (Main.rand.NextBool(7))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), ((new Vector2(4, 4)).RotatedByRandom(100) + Projectile.velocity) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.15f);
                    dust.color = Main.rand.NextBool(5) ? Color.Khaki : Color.Goldenrod;
                    dust.noLightEmittence = true;
                }
                yoyoPower++;
            }

            if (hitCooldown > 0)
                hitCooldown--;

            if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() > 3200f && !firing) // 200 blocks
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hitCooldown == 0)
            {
                hitCooldown = Projectile.localNPCHitCooldown;

                targeted = target;
                yoyoPower += 15;

                // Create Explosion
                if (Projectile.owner == Main.myPlayer)
                {
                    float power = Utils.GetLerpValue(-100, yoyoPowerMax, yoyoPower, true);

                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BurningHolyBlast>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, power);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = DamageClass.MeleeNoSpeed;

                    for (int i = 0; i < (int)(30 * power); i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            Particle spark = new CustomSpark(Projectile.Center, ((new Vector2(19, 19) * power).RotatedByRandom(100)) * Main.rand.NextFloat(0.2f, 1f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 27, Main.rand.NextFloat(1.15f, 1.3f), Main.rand.NextBool(4) ? Color.Khaki : Color.Orange, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.1f, 0.2f));
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                        else
                        {
                            bool isSpark = Main.rand.NextBool(5);
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, isSpark ? 278 : ModContent.DustType<LightDust>(), ((new Vector2(15, 15) * power).RotatedByRandom(100)) * Main.rand.NextFloat(0.2f, 1f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(1.85f, 2.15f) * power * (isSpark ? 0.5f : 1);
                            dust.color = Main.rand.NextBool(5) ? Color.Khaki : Color.Goldenrod;
                            if (isSpark)
                                dust.noGravity = false;
                            else
                                dust.noLightEmittence = true;
                        }
                    }

                    Particle orb1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Goldenrod, "CalamityMod/Particles/SoftRoundExplosion", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.14f * power, 15);
                    GeneralParticleHandler.SpawnParticle(orb1);

                    Particle orb2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Khaki, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 2.1f * power, 15);
                    GeneralParticleHandler.SpawnParticle(orb2);

                    SoundStyle explode = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact");
                    SoundEngine.PlaySound(explode with { Volume = 0.5f, Pitch = 0.3f * power }, Projectile.Center);
                    SoundStyle explode2 = new("CalamityMod/Sounds/Item/HeliumFlashReady");
                    SoundEngine.PlaySound(explode2 with { Volume = 0.7f, Pitch = 0.6f * power }, Projectile.Center);
                }
            }
            
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 480);
        }

        public override bool? CanDamage() => canDamage ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D shineTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            // The back glow
            float power = Utils.GetLerpValue(-100, yoyoPowerMax, yoyoPower, true);
            float randSize = Main.rand.NextFloat(0.9f, 1.1f);
            Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.Goldenrod with { A = 0 }, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.65f * randSize * (1 - fade) * power, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.White with { A = 0 } * 0.65f, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.45f * randSize * (1 - fade) * power, SpriteEffects.None, 0);


            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Projectile.DrawProjectileWithBackglow(Color.Goldenrod with { A = 0 } * fade, lightColor, 4f * fade, texture);

            // The shine effect
            Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Goldenrod with { A = 0 }, 0, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 4.25f * randSize * fade, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.White with { A = 0 } * 0.65f, 0, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 4.05f * randSize * fade, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.Goldenrod with { A = 0 }, MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 4.25f * randSize * fade, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shineTexture, drawPos, null, Color.White with { A = 0 } * 0.65f, MathHelper.PiOver2, shineTexture.Size() * 0.5f, new Vector2(0.4f, 1f) * 4.05f * randSize * fade, SpriteEffects.None, 0);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 50, targetHitbox);
    }
}
