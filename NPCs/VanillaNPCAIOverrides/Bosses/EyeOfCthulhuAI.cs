using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class EyeOfCthulhuAI : VanillaAIOverride
    {
        private const float ProjectileOffset = 50f;

        // Rev+ exclusive
        public static float Phase1ContactDamageMult = 1.333f; // 40 (buffed from 30)
        public static float Phase2ContactDamageMult = 1.6f; // 48 (buffed from 36)
        public static float Phase3ContactDamageMult = 1.8f; // 54 (buffed from 40)
        public static int BloodShotDamage = 8; // 32

        private void SpawnServant(Vector2 servantSpawnCenter, Vector2 servantSpawnVelocity)
        {
            int servantSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu);
            Main.npc[servantSpawn].velocity.X = servantSpawnVelocity.X;
            Main.npc[servantSpawn].velocity.Y = servantSpawnVelocity.Y;

            if (Main.dedServ && servantSpawn < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, servantSpawn);
        }

        private void UpdateChargeLineUp(float lineUpDistControl)
        {
            NPC.ai[2] += 1f;

            if (NPC.ai[2] == lineUpDistControl && Vector2.Distance(NPC.position, Main.player[NPC.target].position) < 200f)
                NPC.ai[2] -= 1f;

            if (NPC.ai[2] >= lineUpDistControl)
            {
                NPC.velocity *= 0.95f;
                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                    NPC.velocity.Y = 0f;
            }
            else
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            Lighting.AddLight(NPC.Center, 0.5f, 0.5f, 0.5f);

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Phases
            float phase2LifeRatio = death ? 0.75f : 0.6f;
            float phase3LifeRatio = death ? 0.4f : 0.3f;
            float finalPhaseRevLifeRatio = death ? 0.2f : 0.15f;
            float penultimatePhaseDeathLifeRatio = death ? 0.3f : 0.2f;
            float finalPhaseDeathLifeRatio = death ? 0.15f : 0.1f;
            bool phase2 = lifeRatio < phase2LifeRatio;
            bool phase3 = lifeRatio < phase3LifeRatio;
            bool finalPhaseRev = lifeRatio < finalPhaseRevLifeRatio;
            bool penultimatePhaseDeath = lifeRatio < penultimatePhaseDeathLifeRatio;
            bool finalPhaseDeath = lifeRatio < finalPhaseDeathLifeRatio;

            float lineUpDist = death ? 15f : 20f;

            // Set contact damage
            NPC.damage = (int)Math.Round(NPC.defDamage * (phase3 ? Phase3ContactDamageMult : phase2 ? Phase2ContactDamageMult : Phase1ContactDamageMult));

            // Servant and projectile velocity, the projectile velocity is multiplied by 2
            float servantAndProjectileVelocity = death ? 7f : 6f;
            NPC.reflectsProjectiles = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            bool dead = Main.player[NPC.target].dead;
            float targetXDistance = NPC.Center.X - Main.player[NPC.target].position.X - (Main.player[NPC.target].width / 2);
            float targetYDistance = NPC.position.Y + NPC.height - 59f - Main.player[NPC.target].position.Y - (Main.player[NPC.target].height / 2);
            float eyeRotation = (float)Math.Atan2(targetYDistance, targetXDistance) + MathHelper.PiOver2;

            if (eyeRotation < 0f)
                eyeRotation += MathHelper.TwoPi;
            else if (eyeRotation > MathHelper.TwoPi)
                eyeRotation -= MathHelper.TwoPi;

            float eyeRotationAcceleration = 0f;
            if (NPC.ai[0] == 0f && NPC.ai[1] == 0f)
                eyeRotationAcceleration = 0.04f;
            if (NPC.ai[0] == 0f && NPC.ai[1] == 2f && NPC.ai[2] > 40f)
                eyeRotationAcceleration = 0.1f;
            if (NPC.ai[0] == 3f && NPC.ai[1] == 0f)
                eyeRotationAcceleration = 0.1f;
            if (NPC.ai[0] == 3f && NPC.ai[1] == 2f && NPC.ai[2] > 40f)
                eyeRotationAcceleration = 0.16f;
            if (NPC.ai[0] == 3f && NPC.ai[1] == 4f && NPC.ai[2] > lineUpDist)
                eyeRotationAcceleration = 0.3f;
            if (NPC.ai[0] == 3f && NPC.ai[1] == 5f)
                eyeRotationAcceleration = 0.1f;

            if (NPC.rotation < eyeRotation)
            {
                if ((eyeRotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= eyeRotationAcceleration;
                else
                    NPC.rotation += eyeRotationAcceleration;
            }
            else if (NPC.rotation > eyeRotation)
            {
                if ((NPC.rotation - eyeRotation) > MathHelper.Pi)
                    NPC.rotation += eyeRotationAcceleration;
                else
                    NPC.rotation -= eyeRotationAcceleration;
            }

            if (NPC.rotation > eyeRotation - eyeRotationAcceleration && NPC.rotation < eyeRotation + eyeRotationAcceleration)
                NPC.rotation = eyeRotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > eyeRotation - eyeRotationAcceleration && NPC.rotation < eyeRotation + eyeRotationAcceleration)
                NPC.rotation = eyeRotation;

            if (Main.rand.NextBool(5))
            {
                int randomBlood = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), DustID.Blood, NPC.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[randomBlood];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            bool shootProjectile = Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1) &&
                NPC.SafeDirectionTo(Main.player[NPC.target].Center).AngleBetween((NPC.rotation + MathHelper.PiOver2).ToRotationVector2()) < MathHelper.ToRadians(18f) &&
                Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > 240f;

            bool charge = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) >= 320f; // 20 tile distance

            if ((dead || Main.IsItDay()) && !BossRushEvent.BossRushActive)
            {
                NPC.velocity.Y -= 0.04f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;
                return false;
            }

            else if (NPC.ai[0] == 0f)
            {
                if (NPC.ai[1] == 0f)
                {
                    float hoverSpeed = death ? 7f + 3f * (1f - lifeRatio) : 7f;
                    float hoverAcceleration = death ? 0.15f + 0.1f * (1f - lifeRatio) : 0.15f;

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 3f;
                        hoverAcceleration += 0.08f;
                    }

                    float attackSwitchTimer = death ? (120f - 180f * (1f - lifeRatio)) : 180f;
                    bool timeToCharge = NPC.ai[2] >= attackSwitchTimer;
                    Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * 400f;
                    Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * (hoverSpeed + (timeToCharge ? ((NPC.ai[2] - attackSwitchTimer) * 0.01f) : 0f));
                    NPC.SimpleFlyMovement(idealVelocity, hoverAcceleration + (timeToCharge ? ((NPC.ai[2] - attackSwitchTimer) * 0.001f) : 0f));

                    NPC.ai[2] += 1f;
                    if (timeToCharge && charge)
                    {
                        NPC.ai[1] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;

                        CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                        NPC.netUpdate = true;
                    }
                    else if (NPC.WithinRange(hoverDestination, 900f))
                    {
                        if (!Main.player[NPC.target].dead)
                            NPC.ai[3] += 1f;

                        float servantSpawnGateValue = death ? 15f : 40f;
                        if (Main.getGoodWorld)
                            servantSpawnGateValue *= 0.8f;

                        if (NPC.ai[3] >= servantSpawnGateValue && shootProjectile)
                        {
                            NPC.ai[3] = 0f;
                            NPC.rotation = eyeRotation;

                            Vector2 servantSpawnVelocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * servantAndProjectileVelocity;
                            Vector2 servantSpawnCenter = NPC.Center + servantSpawnVelocity.SafeNormalize(Vector2.UnitY) * ProjectileOffset;
                            int maxServants = 3;
                            bool spawnServant = NPC.CountNPCS(NPCID.ServantofCthulhu) < maxServants;
                            if (spawnServant)
                                SoundEngine.PlaySound(SoundID.NPCHit1, servantSpawnCenter);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (spawnServant)
                                {
                                    //Phase 1 Servant Spawn
                                    int eye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu);
                                    Main.npc[eye].velocity = servantSpawnVelocity;

                                    if (Main.dedServ && eye < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, eye);
                                }
                                else
                                {
                                    int projType = ProjectileID.BloodNautilusShot;
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + servantSpawnVelocity.SafeNormalize(Vector2.UnitY) * ProjectileOffset, servantSpawnVelocity * 2f, projType, BloodShotDamage, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 600;
                                }
                            }

                            if (spawnServant)
                            {
                                for (int m = 0; m < 10; m++)
                                    Dust.NewDust(servantSpawnCenter, 20, 20, DustID.Blood, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                            }
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    NPC.rotation = eyeRotation;
                    float additionalVelocityPerCharge = 2f;
                    float chargeSpeed = (death ? 10.5f : 8f) + NPC.ai[3] * additionalVelocityPerCharge;
                    if (death)
                        chargeSpeed += 10f * (1f - lifeRatio);
                    if (Main.getGoodWorld)
                        chargeSpeed += 4f;

                    NPC.velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * chargeSpeed;

                    NPC.ai[1] = 2f;
                    NPC.ForceNetUpdate(false);
                }
                else if (NPC.ai[1] == 2f)
                {
                    int chargeDelay = death ? (75 - (int)Math.Round(30f * (1f - lifeRatio))) : 95;
                    if (Main.getGoodWorld)
                        chargeDelay -= 30;

                    float slowDownGateValue = chargeDelay * (death ? 0.85f : 0.65f);

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= slowDownGateValue)
                    {
                        float decelerationScalar = death ? ((lifeRatio - phase2LifeRatio) / (1f - phase2LifeRatio)) : 1f;
                        if (decelerationScalar < 0f)
                            decelerationScalar = 0f;

                        NPC.velocity *= (MathHelper.Lerp(death ? 0.76f : 0.92f, death ? 0.88f : 0.96f, decelerationScalar));
                        if (Main.getGoodWorld)
                            NPC.velocity *= 0.99f;

                        if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                            NPC.velocity.X = 0f;
                        if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                            NPC.velocity.Y = 0f;
                    }
                    else
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                    if (NPC.ai[2] >= chargeDelay)
                    {
                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;
                        NPC.rotation = eyeRotation;

                        float numCharges = death ? 4f : 3f;
                        if (NPC.ai[3] >= numCharges)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                        }
                        else
                            NPC.ai[1] = 1f;
                    }
                }

                if (phase2)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    NPC.ForceNetUpdate(false);
                }
            }

            else if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
            {
                if (Main.getGoodWorld)
                    NPC.reflectsProjectiles = true;

                if (NPC.ai[0] == 1f)
                {
                    NPC.ai[2] += 0.005f;
                    if (NPC.ai[2] > 0.5f)
                        NPC.ai[2] = 0.5f;
                }
                else
                {
                    NPC.ai[2] -= 0.005f;
                    if (NPC.ai[2] < 0f)
                        NPC.ai[2] = 0f;
                }

                NPC.rotation += NPC.ai[2];

                float phaseChangeRate = death ? 2f : 1f;
                float servantSpawnGateValue = Main.getGoodWorld ? 4f : 20f;
                NPC.ai[1] += phaseChangeRate;
                if (NPC.ai[1] % servantSpawnGateValue == 0f)
                {
                    float servantVelocity = death ? 9.3f : 5.65f;
                    Vector2 servantSpawnVelocity = Main.rand.NextVector2CircularEdge(servantVelocity, servantVelocity);
                    if (Main.getGoodWorld)
                        servantSpawnVelocity *= 3f;

                    Vector2 servantSpawnCenter = NPC.Center + servantSpawnVelocity.SafeNormalize(Vector2.UnitY) * ProjectileOffset;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //Phase Transition Servant Spawn
                        SpawnServant(servantSpawnCenter, servantSpawnVelocity);
                    }

                    for (int n = 0; n < 10; n++)
                        Dust.NewDust(servantSpawnCenter, 20, 20, DustID.Blood, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                }

                if (NPC.ai[1] == 100f)
                {
                    NPC.ai[0] += 1f;
                    NPC.ai[1] = 0f;

                    if (NPC.ai[0] == 3f)
                    {
                        NPC.ai[2] = 0f;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                        if (!Main.dedServ)
                        {
                            for (int phase2Gore = 0; phase2Gore < 2; phase2Gore++)
                            {
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int i = 0; i < 20; i++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    }
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                NPC.velocity *= 0.98f;

                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                    NPC.velocity.Y = 0f;
            }

            else
            {
                NPC.defense = 0;

                if (NPC.ai[1] == 0f & phase3)
                    NPC.ai[1] = 5f;

                if (NPC.ai[1] == 0f)
                {
                    float hoverSpeed = (death ? 7.5f : 5.5f) + (death ? 8.5f : 3f) * (phase2LifeRatio - lifeRatio);
                    float hoverAcceleration = (death ? 0.08f : 0.06f) + (death ? 0.08f : 0.02f) * (phase2LifeRatio - lifeRatio);

                    Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * 400f;
                    float distanceFromHoverDestination = NPC.Distance(hoverDestination);

                    if (distanceFromHoverDestination > 400f)
                    {
                        hoverSpeed += 1.25f;
                        hoverAcceleration += 0.075f;
                        if (distanceFromHoverDestination > 600f)
                        {
                            hoverSpeed += 1.25f;
                            hoverAcceleration += 0.075f;
                            if (distanceFromHoverDestination > 800f)
                            {
                                hoverSpeed += 1.25f;
                                hoverAcceleration += 0.075f;
                            }
                        }
                    }

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 1f;
                        hoverAcceleration += 0.1f;
                    }

                    float phaseLimit = death ? (160f - 150f * (phase2LifeRatio - lifeRatio)) : 200f;
                    bool timeToCharge = NPC.ai[2] >= phaseLimit;
                    Vector2 idealHoverVelocity = NPC.SafeDirectionTo(hoverDestination) * (hoverSpeed + (timeToCharge ? ((NPC.ai[2] - phaseLimit) * 0.01f) : 0f));
                    NPC.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration + (timeToCharge ? ((NPC.ai[2] - phaseLimit) * 0.001f) : 0f));

                    NPC.ai[2] += 1f;

                    if (death)
                    {
                        float projectileGateValue = lifeRatio < 0.5f ? 40f : 60f;
                        if (NPC.ai[2] % projectileGateValue == 0f && shootProjectile)
                        {
                            Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * servantAndProjectileVelocity * 2f;
                            Vector2 projectileSpawnCenter = NPC.Center + projectileVelocity;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ProjectileID.BloodNautilusShot;
                                int numProj = 3;
                                int spread = 18;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * ProjectileOffset, perturbedSpeed, type, BloodShotDamage, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 600;
                                }
                            }
                        }
                    }

                    if (timeToCharge && charge)
                    {
                        NPC.ai[1] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                else if (NPC.ai[1] == 1f)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    NPC.rotation = eyeRotation;

                    float additionalVelocityPerCharge = 3f;
                    float chargeSpeed = (death ? 12f : 10f) + ((death ? 10f : 3.5f) * (phase2LifeRatio - lifeRatio)) + NPC.ai[3] * additionalVelocityPerCharge;
                    if (NPC.ai[3] == 1f)
                        chargeSpeed *= 1.15f;
                    if (NPC.ai[3] == 2f)
                        chargeSpeed *= 1.3f;
                    if (Main.getGoodWorld)
                        chargeSpeed *= 1.2f;

                    NPC.velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * chargeSpeed;
                    NPC.ai[1] = 2f;
                    NPC.ForceNetUpdate(false);
                }

                else if (NPC.ai[1] == 2f)
                {
                    int phase2ChargeDelay = death ? (70 - (int)Math.Round(25f * (phase2LifeRatio - lifeRatio))) : 85;

                    float slowDownGateValue = phase2ChargeDelay * (death ? 0.9f : 0.75f);

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= slowDownGateValue)
                    {
                        float decelerationScalar = death ? ((lifeRatio - phase3LifeRatio) / (phase2LifeRatio - phase3LifeRatio)) : 1f;
                        if (decelerationScalar < 0f)
                            decelerationScalar = 0f;

                        NPC.velocity *= (MathHelper.Lerp(death ? 0.6f : 0.9f, death ? 0.7f : 0.95f, decelerationScalar));
                        if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                            NPC.velocity.X = 0f;
                        if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                            NPC.velocity.Y = 0f;
                    }
                    else
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                    if (NPC.ai[2] >= phase2ChargeDelay)
                    {
                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;
                        NPC.rotation = eyeRotation;

                        float numCharges = death ? 4f : 3f;
                        if (NPC.ai[3] >= numCharges)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.ForceNetUpdate(false);
                        }
                        else
                            NPC.ai[1] = 1f;
                    }
                }

                else if (NPC.ai[1] == 3f)
                {
                    if ((NPC.ai[3] == 4f & phase3) && NPC.Center.Y > Main.player[NPC.target].Center.Y)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.ForceNetUpdate(false);
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speedBoost = death ? 10f * (phase3LifeRatio - lifeRatio) : 7f * (phase3LifeRatio - lifeRatio);
                        float finalChargeSpeed = (death ? 23f : 18f) + speedBoost;

                        Vector2 eyeChargeDirection = NPC.Center;
                        float targetX = Main.player[NPC.target].Center.X - eyeChargeDirection.X;
                        float targetY = Main.player[NPC.target].Center.Y - eyeChargeDirection.Y;
                        float targetVelocity = Math.Abs(Main.player[NPC.target].velocity.X) + Math.Abs(Main.player[NPC.target].velocity.Y) / 4f;
                        targetVelocity += 10f - targetVelocity;

                        if (targetVelocity < (death ? 2f : 5f))
                            targetVelocity = (death ? 2f : 5f);
                        if (targetVelocity > (death ? 6f : 15f))
                            targetVelocity = (death ? 6f : 15f);

                        if (NPC.ai[2] == -1f)
                        {
                            targetVelocity *= 4f;
                            finalChargeSpeed *= 1.3f;
                        }

                        targetX -= Main.player[NPC.target].velocity.X * targetVelocity;
                        targetY -= Main.player[NPC.target].velocity.Y * targetVelocity / 4f;

                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                        float targetDistCopy = targetDistance;

                        targetDistance = finalChargeSpeed / targetDistance;
                        NPC.velocity.X = targetX * targetDistance;
                        NPC.velocity.Y = targetY * targetDistance;

                        if (targetDistCopy < 100f)
                        {
                            if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                            {
                                float absoluteXVel = Math.Abs(NPC.velocity.X);
                                float absoluteYVel = Math.Abs(NPC.velocity.Y);

                                if (NPC.Center.X > Main.player[NPC.target].Center.X)
                                    absoluteYVel *= -1f;
                                if (NPC.Center.Y > Main.player[NPC.target].Center.Y)
                                    absoluteXVel *= -1f;

                                NPC.velocity.X = absoluteYVel;
                                NPC.velocity.Y = absoluteXVel;
                            }
                        }
                        else if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                        {
                            float absoluteEyeVel = (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) / 2f;
                            float absoluteEyeVelBackup = absoluteEyeVel;

                            if (NPC.Center.X > Main.player[NPC.target].Center.X)
                                absoluteEyeVelBackup *= -1f;
                            if (NPC.Center.Y > Main.player[NPC.target].Center.Y)
                                absoluteEyeVel *= -1f;

                            NPC.velocity.X = absoluteEyeVelBackup;
                            NPC.velocity.Y = absoluteEyeVel;
                        }

                        NPC.ai[1] = 4f;
                        NPC.ForceNetUpdate(false);
                    }
                }

                else if (NPC.ai[1] == 4f)
                {
                    if (NPC.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);

                    float lineUpDistControl = lineUpDist;
                    UpdateChargeLineUp(lineUpDistControl);

                    float lineUpDistNetUpdate = lineUpDistControl + 13f;
                    if (NPC.ai[2] >= lineUpDistNetUpdate)
                    {
                        NPC.ForceNetUpdate(false);

                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;

                        float maxCharges = death ? (finalPhaseDeath ? 0f : penultimatePhaseDeath ? 1f : 2f) : finalPhaseRev ? 2f : 3f;
                        if (NPC.ai[3] >= maxCharges)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                        }
                        else
                            NPC.ai[1] = 3f;
                    }
                }

                else if (NPC.ai[1] == 5f)
                {
                    float offset = death ? 540f : 600f;
                    float speedBoost = death ? 15f * (phase3LifeRatio - lifeRatio) : 5f * (phase3LifeRatio - lifeRatio);
                    float accelerationBoost = death ? 0.425f * (phase3LifeRatio - lifeRatio) : 0.125f * (phase3LifeRatio - lifeRatio);
                    float hoverSpeed = (death ? 10f : 8f) + speedBoost;
                    float hoverAcceleration = (death ? 0.3125f : 0.25f) + accelerationBoost;

                    bool horizontalCharge = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 3f;
                    float timeGateValue = horizontalCharge ? (110f - (death ? 30f * (phase3LifeRatio - lifeRatio) : 0f)) : (95f - (death ? 55f * (phase3LifeRatio - lifeRatio) : 0f));
                    if (NPC.ai[2] > timeGateValue)
                    {
                        float velocityScalar = NPC.ai[2] - timeGateValue;
                        hoverSpeed += velocityScalar * 0.05f;
                        hoverAcceleration += velocityScalar * 0.0025f;
                    }

                    Vector2 eyeLineUpChargeDirection = NPC.Center;
                    float lineUpChargeTargetX = Main.player[NPC.target].Center.X - eyeLineUpChargeDirection.X;
                    float lineUpChargeTargetY = Main.player[NPC.target].Center.Y + offset - eyeLineUpChargeDirection.Y;
                    Vector2 hoverDestination = Main.player[NPC.target].Center + Vector2.UnitY * offset;

                    if (horizontalCharge)
                    {
                        float horizontalChargeOffset = death ? 450f : 500f;
                        offset = calamityGlobalNPC.newAI[0] == 1f ? -horizontalChargeOffset : horizontalChargeOffset;
                        hoverSpeed *= 1.5f;
                        hoverAcceleration *= 1.5f;
                        hoverDestination = Main.player[NPC.target].Center + Vector2.UnitX * offset;
                    }

                    Vector2 idealHoverVelocity = NPC.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    NPC.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration);

                    float servantSpawnGateValue = horizontalCharge ? (death ? 23f : 35f) : (death ? 17f : 27f);
                    float maxServantSpawnsPerAttack = 2f;

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] % servantSpawnGateValue == 0f && shootProjectile && NPC.ai[2] <= servantSpawnGateValue * maxServantSpawnsPerAttack)
                    {
                        //phase 3 servant spawning
                        Vector2 servantSpawnVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * servantAndProjectileVelocity;
                        Vector2 servantSpawnCenter = NPC.Center + servantSpawnVelocity.SafeNormalize(Vector2.UnitY) * ProjectileOffset;
                        if (death)
                        {
                            servantSpawnCenter = Main.player[NPC.target].Center + Main.rand.NextVector2CircularEdge(1600, 1600);
                            servantSpawnVelocity = (Main.player[NPC.target].Center - servantSpawnCenter).SafeNormalize(Vector2.UnitY) * servantAndProjectileVelocity * 2;
                        }
                        int maxServants = death ? (finalPhaseDeath ? 2 : penultimatePhaseDeath ? 3 : 4) : (finalPhaseRev ? 2 : 4);
                        bool spawnServant = NPC.CountNPCS(NPCID.ServantofCthulhu) < maxServants;

                        if (spawnServant)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath13, servantSpawnCenter);

                            for (int m = 0; m < 10; m++)
                                Dust.NewDust(servantSpawnCenter, 20, 20, DustID.Blood, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (spawnServant)
                            {
                                //Final Phase Servant Spawn
                                SpawnServant(servantSpawnCenter, servantSpawnVelocity);
                            }
                            else if (!Main.getGoodWorld)
                            {
                                var spawnVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * servantAndProjectileVelocity;
                                int amount = death ? 3 : 1;
                                for (var i = 0; i < amount; i++)
                                {
                                    Vector2 perturbedSpeed = spawnVelocity.RotatedBy(MathHelper.Lerp(-0.5f, 0.5f, i / (float)(amount - 1))) * (death ? 1.25f : 1f);
                                    int projType = ProjectileID.BloodNautilusShot;
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * ProjectileOffset, perturbedSpeed * 2f, projType, BloodShotDamage, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 600;
                                }
                            }

                            if (Main.getGoodWorld)
                            {
                                int type = ProjectileID.BloodNautilusShot;
                                Vector2 projectileVelocity = servantSpawnVelocity * 3f;
                                int numProj = death ? 5 : 3;
                                int spread = 20;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * ProjectileOffset, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 600;
                                }
                            }
                        }
                    }

                    float requiredDistanceForHorizontalCharge = 160f;
                    if (NPC.ai[2] >= timeGateValue && (NPC.Distance(hoverDestination) < requiredDistanceForHorizontalCharge || !horizontalCharge))
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0: // Normal Eye behavior
                                NPC.ai[1] = 3f;
                                NPC.ai[2] = -1f;
                                NPC.ai[3] = -1f;
                                break;

                            case 1: // Charge from the left
                                NPC.ai[1] = 6f;
                                NPC.ai[2] = 0f;
                                break;

                            case 2: // Normal Eye behavior
                                NPC.ai[1] = 3f;
                                NPC.ai[2] = -1f;
                                break;

                            case 3: // Charge from the right
                                NPC.ai[1] = 6f;
                                NPC.ai[2] = 0f;
                                break;

                            default:
                                break;
                        }

                        calamityGlobalNPC.newAI[0] += ((death && calamityGlobalNPC.newAI[0] % 2f != 0f) ? Main.rand.Next(2) + 1f : 1f);
                        if (calamityGlobalNPC.newAI[0] > 3f)
                            calamityGlobalNPC.newAI[0] = death ? Main.rand.Next(2) : 0f;

                        NPC.SyncExtraAI();
                    }

                    NPC.ForceNetUpdate(false);
                }

                else if (NPC.ai[1] == 6f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speedBoost = death ? 15f * (phase3LifeRatio - lifeRatio) : 5f * (phase3LifeRatio - lifeRatio);
                        float chargeSpeed = (death ? 23f : 18f) + speedBoost;
                        NPC.velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * chargeSpeed;

                        NPC.ai[1] = 7f;
                        NPC.ForceNetUpdate(false);
                    }
                }

                else if (NPC.ai[1] == 7f)
                {
                    if (NPC.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                    float lineUpDistControl = (float)Math.Round(lineUpDist * 2.5f);
                    UpdateChargeLineUp(lineUpDistControl);

                    float lineUpDistNetUpdate = lineUpDistControl + 13f;
                    if (NPC.ai[2] >= lineUpDistNetUpdate)
                    {
                        NPC.ForceNetUpdate(false);

                        NPC.ai[2] = 0f;
                        NPC.ai[1] = 0f;
                    }
                }
            }

            return false;
        }
    }
}
