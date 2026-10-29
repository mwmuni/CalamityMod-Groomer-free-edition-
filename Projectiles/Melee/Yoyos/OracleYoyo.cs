using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class OracleYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheOracle>();
        public int AuraFrame;
        private Player Owner => Main.player[Projectile.owner];

        // projectile.localAI[1] is the Aura Charge of the red lightning aura
        // Minimum value is zero. Maximum value is 150.
        // The aura turns on and begins damaging enemies at 20 charge.
        // The yoyo "supercharges" at 50 charge.
        // Its size caps out at 100 charge.
        public ref float AuraCharge => ref Projectile.localAI[1];

        private const float MaxCharge = 150f;
        private const float MinAuraRadius = 20f;
        private const float SuperchargeThreshold = 50f;
        private const float MaxAuraRadius = 150f;
        private const float MinDischargeRate = 0.05f;
        private const float MaxDischargeRate = 0.53f;
        private const float DischargeRateScaleFactor = 0.003f;
        private const float ChargePerHit = 4f;
        private float rotationAngle = 0;
        private bool rotDirection = false;
        private const int HitsPerOrbVolley = 2;
        private int OrbCooldown = 0;
        public bool cloneYoyo = false;

        public int counter = 0;

        public SlotId Hum { get; set; }

        // The aura hits once per this many frames.
        private const int AuraLocalIFrames = 12;

        // Ensures that the main AI only runs once per frame, despite the projectile's multiple updates
        private const int UpdatesPerFrame = 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = TheOracle.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = TheOracle.Speed / UpdatesPerFrame;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AuraFrame);
            writer.Write(AuraCharge);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AuraFrame = reader.ReadInt32();
            AuraCharge = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 20;
            Projectile.MaxUpdates = UpdatesPerFrame;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = 5 * UpdatesPerFrame;
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
                    if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                    {
                        MainYoyo = x;
                        break;
                    }
                }

                if (Projectile.whoAmI != MainYoyo)
                    cloneYoyo = true;
            }

            if (OrbCooldown > 0)
                OrbCooldown--;

            if (AuraCharge <= SuperchargeThreshold)
            {
                Vector2 vel = new Vector2(45, 45).RotatedByRandom(100);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + vel, DustID.MinecartSpark, Vector2.Zero, 0, default, Main.rand.NextFloat(2.2f, 2.4f));
                dust.noGravity = true;
            }

            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
            {
                Projectile.Kill();
            }

            // Only do stuff once per frame, despite the yoyo's extra updates.
            if (!Projectile.FinalExtraUpdate())
                return;

            // Produces golden dust constantly while in flight. This helps light the yoyo.
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool(3) ? 244 : 246;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
                Main.dust[idx].scale = scale;
            }

            // The yoyo makes its own faint yellow light (unnoticeable once the lightning aura gets going)
            Lighting.AddLight(Projectile.Center, 0.6f, 0.42f, 0.1f);

            // The aura discharges over time based on its current charge.
            float discharge = MinDischargeRate + DischargeRateScaleFactor * AuraCharge;
            if (discharge > MaxDischargeRate)
                discharge = MaxDischargeRate;
            AuraCharge -= discharge;

            // Boundary checks on aura charge
            if (AuraCharge < 0f)
                AuraCharge = 0f;
            if (AuraCharge > MaxCharge)
                AuraCharge = MaxCharge;

            // If the aura is large enough to be considered "on", draw it, make sound and damage enemies
            if (AuraCharge > MinAuraRadius)
            {
                float auraRadius = AuraCharge > MaxAuraRadius ? MaxAuraRadius : AuraCharge;
                DrawLightningAura(auraRadius);

                if (!cloneYoyo)
                {
                    if (SoundEngine.TryGetActiveSound(Hum, out var hum) && hum.IsPlaying)
                    {
                        hum.Position = Projectile.Center;
                        hum.Pitch = MathHelper.Lerp(-0.4f, 0.2f, Utils.GetLerpValue(0f, MaxCharge, AuraCharge, true));
                        hum.Volume = MathHelper.Lerp(0f, 0.55f * 100, Utils.GetLerpValue(MinAuraRadius, MaxCharge / 2, AuraCharge, true));
                    }
                    else
                    {
                        SoundStyle charge = new("CalamityMod/Sounds/Item/OracleHum");
                        Hum = SoundEngine.PlaySound(charge with { Volume = 0.01f, IsLooped = true }, Projectile.Center);
                    }

                }
                
                if (AuraFrame % AuraLocalIFrames == 0)
                {
                    // The aura's direct damage scales with its charge and with melee stats.
                    float chargeRatio = AuraCharge / MaxCharge;
                    int auraDamage = (int)(Projectile.damage * MathHelper.Lerp(TheOracle.AuraBaseDamageMult, TheOracle.AuraMaxDamageMult, chargeRatio));
                    DealAuraDamage(auraRadius, auraDamage);
                }

                // Experimental clone yoyo orbiting, could be implemented later

                /*
                if (cloneYoyo)
                {
                    Vector2 cloneOffset = new Vector2(auraRadius, 0f);
                    cloneOffset = cloneOffset.RotatedBy(counter * 0.06f);
                    if (Owner.controlUseItem)
                    {
                        Projectile.Center = Main.player[Projectile.owner].ClampedMouseWorld() + cloneOffset;
                        Projectile.velocity = Vector2.Zero;
                    }
                }
                */
            }
            else
            {
                if (SoundEngine.TryGetActiveSound(Hum, out var hum) && hum.IsPlaying && !cloneYoyo)
                {
                    hum?.Stop();
                }
            }

            AuraFrame = (AuraFrame + 1) % AuraLocalIFrames;

            counter++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AuricRebuke>(), 90);

            // On hit effects do not apply if no damage was done.
            if (hit.Damage <= 0)
                return;

            // Charge up the lightning aura with every hit.
            AuraCharge += ChargePerHit;

            // Fire Auric orbs every few hits while supercharged.
            if (AuraCharge > SuperchargeThreshold && Projectile.numHits % HitsPerOrbVolley == 0 && OrbCooldown == 0)
            {
                OrbCooldown = 30;
                FireAuricOrbs();
            }
        }

        // Uses dust type 260, which lives for an extremely short amount of time
        private void DrawLightningAura(float radius)
        {
            // Light emits from the yoyo itself while the aura is active, eventually becoming insanely bright
            float brightness = radius * 0.03f;
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * brightness);

            // Number of particles on the circumference scales directly with the circumference
            float dustDensity = 0.2f;
            int numDust = (int)(dustDensity * MathHelper.TwoPi * radius);
            float angleIncrement = MathHelper.TwoPi / numDust;

            // Incrementally rotate the vector as a ring of dust is drawn
            Vector2 dustOffset = new Vector2(radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; ++i)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);

                Particle spark = new GlowOrbParticle(Projectile.Center + dustOffset, Vector2.One.RotatedByRandom(100), false, 2, Main.rand.NextFloat(0.65f, 1.1f), Main.rand.NextBool(11) ? Color.Lavender : Color.Cyan);
                GeneralParticleHandler.SpawnParticle(spark);

                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = 226;
                float scale = Main.rand.NextFloat(0.4f, 0.7f);
                Vector2 dustyVel = (dustOffset).SafeNormalize(Vector2.UnitX) * 10;
                if (Main.rand.NextBool(40))
                {
                    int idx = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                    Main.dust[idx].position = Projectile.Center + dustOffset;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].noLight = true;
                    Main.dust[idx].velocity = dustyVel.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.75f);
                    Main.dust[idx].scale = scale;
                    Main.dust[idx].noLightEmittence = true;
                }
            }

            // Rarely, draw some "arcs" which are lines of dust to the edge
            if (Main.rand.NextBool(3))
            {
                int numArcs = Main.rand.Next(2, 3 + 1);
                for (int i = 0; i < numArcs; ++i)
                {
                    rotDirection = Main.rand.NextBool();
                    float rotInstensity = Main.rand.NextFloat(0.15f, 0.4f);
                    Vector2 radiusVec = new Vector2(radius, 0f);
                    int dustPerArc = 40;
                    radiusVec = radiusVec.RotatedByRandom(MathHelper.TwoPi);
                    for (int j = 0; j < dustPerArc; ++j)
                    {
                        if (rotationAngle >= 1.55f)
                            rotDirection = true;
                        if (rotationAngle <= -1.55f)
                            rotDirection = false;

                        rotationAngle += (rotInstensity * (rotDirection ? -1 : 1));

                        Vector2 partialRadius = (float)j / dustPerArc * radiusVec;
                        Vector2 radiusBonus = (partialRadius.SafeNormalize(Vector2.UnitX) * 5).RotatedBy(MathHelper.ToRadians(90f)) * rotationAngle;

                        Particle spark = new GlowOrbParticle(Projectile.Center + partialRadius + radiusBonus, radiusVec * 0.001f, false, 3, 0.75f - j * 0.0025f, Main.rand.NextBool(11) ? Color.Lavender : Color.Cyan);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
        }

        private void DealAuraDamage(float radius, int damage)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];

            foreach (NPC target in Main.ActiveNPCs)
            {
                if (target.dontTakeDamage || target.friendly)
                    continue;

                // Shock any valid target within range. Check all four corners of their hitbox.
                float d1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
                float d2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
                float d3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
                float d4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());
                float dist = MathHelper.Min(d1, d2);
                dist = MathHelper.Min(dist, d3);
                dist = MathHelper.Min(dist, d4);

                if (dist <= radius)
                {
                    target.AddBuff(ModContent.BuffType<AuricRebuke>(), 300);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, Projectile.owner, target.whoAmI);
                        if (p.whoAmI.WithinBounds(Main.maxProjectiles))
                            p.DamageType = DamageClass.MeleeNoSpeed;
                    }
                }
            }
        }

        private void FireAuricOrbs()
        {
            int numOrbs = 3;
            float angleVariance = MathHelper.TwoPi / numOrbs;
            float spinOffsetAngle = MathHelper.Pi / (2f * numOrbs);
            Vector2 posVec = new Vector2(2f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numOrbs; ++i)
            {
                posVec = posVec.RotatedBy(angleVariance);
                Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                velocity.Normalize();
                velocity *= 18f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + posVec, velocity, ModContent.ProjectileType<Orbacle>(), Projectile.damage, 8f, Main.myPlayer, 0.0f, 0.0f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(Hum, out var hum) && hum.IsPlaying && !cloneYoyo)
            {
                hum?.Stop();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 60, targetHitbox);
    }
}
