using System;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class PlanteraAI : VanillaAIOverride
    {
        public const float SeedGatlingGateValue = 600f;
        public const float SeedGatlingDuration = 300f;
        public const float SeedGatlingColorChangeDuration = 180f;
        public const float SeedGatlingStopValue = SeedGatlingGateValue + SeedGatlingDuration;
        public const float SeedGatlingColorChangeGateValue = SeedGatlingStopValue - SeedGatlingColorChangeDuration;
        public const float TentaclePhaseSlowDuration = 1200f;
        public const float ChargePhaseGateValue = 900f;
        public const float ChargeTelegraphColorChangeGateValue = ChargePhaseGateValue - SeedGatlingColorChangeDuration;
        public const float ReduceSpeedForChargeDistance = 480f;
        public const float BeginChargeGateValue = -120f;
        public const float BeginChargeSlowDownGateValue = BeginChargeGateValue - 45f;
        public const float StopChargeGateValue = BeginChargeSlowDownGateValue - 30f;
        public const float MovementVelocityMultiplierForSlowAttacks = 0.5f;

        // Vanilla values
        public static float Phase2ContactDamageMult = 1.4f; // 140
        public static int PinkSeedDamage = 19; // 76
        public static int PoisonSeedDamage = 24; // 96
        public static int ThornBallDamage = 27; // 108

        // Expert AI has uses a magic number which we have to correct for
        public static int ContactDamageCorrection = Main.masterMode ? 150 : 100;

        // Rev+ exclusive
        public static int ThornBallSpikeDamage = 22; // 88
        public static int GasBulbDamage = 27; // 108
        public static int PinkCloudDamage = 22; // 88
        public static int GreenCloudDamage = 24; // 96
        public static float DashDamageMult = 1.25f; // 175

        private static void CheckThornBallLimit(ref bool shootThornBall)
        {
            int numThornBalls = 0;
            int thornBallLimit = 3;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.ThornBall)
                {
                    numThornBalls++;
                    if (numThornBalls >= thornBallLimit)
                    {
                        shootThornBall = false;
                        break;
                    }
                }
            }
        }

        private static void SpawnSeedDust(Vector2 spawnOffset, int dustSpawnBoxSize, int dustType, Vector2 dustVelocity, int dustAmount)
        {
            for (int k = 0; k < dustAmount; k++)
            {
                int dust = Dust.NewDust(spawnOffset, dustSpawnBoxSize, dustSpawnBoxSize, dustType, dustVelocity.X, dustVelocity.Y);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.4f;
            }
        }

        private void SpawnSporeTelegraphDust()
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, 0f, 0f, 0, default, 1.4f);
            Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101)).SafeNormalize(Vector2.UnitY);
            vector *= Main.rand.Next(50, 100) * 0.04f;
            Main.dust[dust].velocity = vector;
            vector = vector.SafeNormalize(Vector2.UnitY);
            vector *= 86f;
            Main.dust[dust].position = NPC.Center - vector;
        }

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases based on HP
            float phase2LifeRatio = 0.5f;
            bool addThornBallsToGatlingAttack = lifeRatio < 0.85f;
            bool addSporeGasBlastToGatlingAttack = lifeRatio < 0.75f;
            bool useNewGatlingAttackVariant = addSporeGasBlastToGatlingAttack && death;
            bool phase2 = lifeRatio <= phase2LifeRatio;
            bool phase3 = lifeRatio < 0.35f;
            bool phase4 = lifeRatio < 0.2f;

            NPC.damage = (int)Math.Round(ContactDamageCorrection * (phase2 ? Phase2ContactDamageMult : 1f));

            // Variables and target
            bool enrage = false;
            bool despawn = false;

            // Check for Jungle
            bool surface = !BossRushEvent.BossRushActive && Main.player[NPC.target].position.Y < Main.worldSurface * 16.0;

            // Tentacle limits
            int maxTentaclesAfterFirstTentaclePhase = death ? 4 : 2;
            int maxFreeTentaclesAfterFirstTentaclePhase = maxTentaclesAfterFirstTentaclePhase * 2;

            float speedUpDistance = 480f;
            bool speedUp = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > speedUpDistance; // 30 or 40 tile distance

            // Despawn
            if (Main.player[NPC.target].dead)
            {
                despawn = true;
                enrage = true;
            }

            // Despawn if too far from target
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 6000f)
                {
                    NPC.active = false;
                    NPC.life = 0;
                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
            }

            // Set whoAmI variable and spawn hooks
            NPC.plantBoss = NPC.whoAmI;
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasHook, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasHook, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasHook, NPC.whoAmI);
            }

            // Find positions of hooks
            int maxHooks = 3;
            int[] hookArray = new int[maxHooks];
            float hookPositionX = 0f;
            float hookPositionY = 0f;
            int numHooksSpawned = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.aiStyle == NPCAIStyleID.PlanteraHook)
                {
                    hookPositionX += n.Center.X;
                    hookPositionY += n.Center.Y;
                    hookArray[numHooksSpawned] = n.whoAmI;

                    numHooksSpawned++;
                    if (numHooksSpawned >= maxHooks)
                        break;
                }
            }
            hookPositionX /= numHooksSpawned;
            hookPositionY /= numHooksSpawned;

            // Velocity and acceleration
            float velocity = phase4 ? 7f : phase3 ? 6.5f : phase2 ? 6f : 4f;
            float acceleration = phase3 ? 0.06f : 0.04f;
            float chargeLineUpVelocity = phase4 ? 12f : phase3 ? 10f : 8f;
            float chargeLineUpAcceleration = phase4 ? 0.6f : phase3 ? 0.5f : 0.4f;
            float chargeVelocity = phase4 ? 22f : phase3 ? 20f : 18f;
            float chargeDeceleration = phase4 ? 0.92f : phase3 ? 0.95f : 0.96f;

            // Enrage if target is on the surface
            if (!BossRushEvent.BossRushActive && (surface || Main.player[NPC.target].position.Y > Main.UnderworldLayer * 16))
            {
                enrage = true;
                velocity += 8f;
                acceleration = 0.15f;
            }

            NPC.Calamity().CurrentlyEnraged = enrage;

            // Movement relative to the target and hook positions
            Vector2 npcCenterAccountingForHooks = new Vector2(hookPositionX, hookPositionY);
            float maxVelocityX = Main.player[NPC.target].Center.X - npcCenterAccountingForHooks.X;
            float maxVelocityY = Main.player[NPC.target].Center.Y - npcCenterAccountingForHooks.Y;
            bool phase1MoveAway = !phase2 && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 240f && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
            bool adjustProjectileShootLocation = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 80f;
            if (despawn)
            {
                maxVelocityY *= -1f;
                maxVelocityX *= -1f;
                velocity += 8f;
            }
            else if (phase1MoveAway)
            {
                maxVelocityY *= -1f;
                maxVelocityX *= -1f;
                velocity *= 1.5f;
                acceleration *= 1.5f;
            }
            float distanceFromTarget = (float)Math.Sqrt(maxVelocityX * maxVelocityX + maxVelocityY * maxVelocityY);

            if (death)
            {
                velocity += velocity * 0.35f * ((1f - lifeRatio) / 2);
                acceleration += acceleration * 0.35f * ((1f - lifeRatio) / 2);
                if (phase2)
                {
                    float aggressionScale = (phase2LifeRatio - lifeRatio) / phase2LifeRatio;
                    chargeLineUpVelocity += chargeLineUpVelocity * 0.15f * aggressionScale;
                    chargeLineUpAcceleration += chargeLineUpAcceleration * 0.15f * aggressionScale;
                    chargeVelocity += chargeVelocity * 0.15f * aggressionScale;
                    chargeDeceleration -= 0.05f * aggressionScale;
                }
            }

            if (Main.getGoodWorld)
            {
                velocity *= 1.15f;
                acceleration *= 1.15f;
            }

            // Slow down and fire a gatling of projectiles
            // These projectiles are slower than normal
            // Glow gradually more green the closer the gatling attack is to ending
            bool usingSeedGatling = NPC.ai[1] > SeedGatlingGateValue;
            bool slowedDuringTentaclePhase = NPC.ai[2] > 0f;
            bool doneWithTentaclePhase = NPC.ai[2] == -1f;
            bool charging = NPC.ai[3] <= -2f;
            bool secondCharge = calamityGlobalNPC.newAI[2] == 1f;
            if (!phase2)
            {
                NPC.ai[1] += 1f;
                if (usingSeedGatling)
                {
                    float currentSeedGatlingTime = NPC.ai[1] - SeedGatlingGateValue;

                    // Slow down more and more as gatling attack continues
                    velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(currentSeedGatlingTime / SeedGatlingDuration, 2D));

                    // Shoot projectiles
                    float shootProjectileGateValue = useNewGatlingAttackVariant ? 45f : 30f;
                    if (currentSeedGatlingTime >= 240f)
                        shootProjectileGateValue = useNewGatlingAttackVariant ? 9f : 3f;
                    else if (currentSeedGatlingTime >= 180f)
                        shootProjectileGateValue = useNewGatlingAttackVariant ? 15f : 5f;
                    else if (currentSeedGatlingTime >= 120f)
                        shootProjectileGateValue = useNewGatlingAttackVariant ? 18f : 9f;
                    else if (currentSeedGatlingTime >= 60f)
                        shootProjectileGateValue = useNewGatlingAttackVariant ? 30f : 15f;

                    if (NPC.ai[1] % shootProjectileGateValue == 0f)
                    {
                        bool shootThornBall = NPC.ai[1] % 90f == 0f && addThornBallsToGatlingAttack && !useNewGatlingAttackVariant;
                        bool shootPoisonSeed = NPC.ai[1] % 9f == 0f && !shootThornBall;
                        float projectileSpeed = 14f;
                        int projectileType = shootThornBall ? ProjectileID.ThornBall : shootPoisonSeed ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                        int damage = shootThornBall ? ThornBallDamage : shootPoisonSeed ? PoisonSeedDamage : PinkSeedDamage;
                        Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        Vector2 spawnOffset = NPC.Center + projectileVelocity * 70f;

                        if (useNewGatlingAttackVariant)
                        {
                            int spread = 8;
                            if (currentSeedGatlingTime >= 240f)
                                spread = 16;
                            else if (currentSeedGatlingTime >= 180f)
                                spread = 14;
                            else if (currentSeedGatlingTime >= 120f)
                                spread = 12;
                            else if (currentSeedGatlingTime >= 60f)
                                spread = 10;

                            float rotation = MathHelper.ToRadians(spread);
                            int numProj = 3;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                int dustType = shootPoisonSeed ? 74 : 73;
                                Vector2 dustVelocity = perturbedSpeed * projectileSpeed;
                                SpawnSeedDust(spawnOffset, 14, dustType, dustVelocity, 5);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnOffset, perturbedSpeed * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                            }
                        }
                        else
                        {
                            int dustType = shootPoisonSeed ? 74 : 73;
                            int dustSpawnBoxSize = shootThornBall ? 38 : 14;
                            int dustAmount = shootThornBall ? 15 : 5;
                            Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                            SpawnSeedDust(spawnOffset, dustSpawnBoxSize, dustType, dustVelocity, dustAmount);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float ai2 = projectileType == ProjectileID.ThornBall && (Main.rand.NextBool() || !Main.zenithWorld) ? 1f : 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), adjustProjectileShootLocation ? NPC.Center : spawnOffset, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer, 0f, 0f, ai2);
                            }
                        }
                    }
                }

                // Spore Gas vomit color telegraph
                if (addSporeGasBlastToGatlingAttack)
                {
                    bool startEmittingDust = NPC.ai[1] > SeedGatlingColorChangeGateValue;
                    if (startEmittingDust)
                    {
                        float dustEmitAmount = NPC.ai[1] - SeedGatlingColorChangeGateValue;
                        int dustInXChanceMin = 2;
                        int dustInXChanceMax = 8;
                        int dustChance = (int)Math.Round(MathHelper.Lerp(dustInXChanceMin, dustInXChanceMax, 1f - dustEmitAmount / SeedGatlingColorChangeDuration));
                        if (Main.rand.NextBool(dustChance))
                        {
                            SpawnSporeTelegraphDust();
                        }
                    }
                }

                if (NPC.ai[1] >= SeedGatlingStopValue)
                {
                    // Vomit dense spread of spore gas at the end of the gatling attack
                    if (addSporeGasBlastToGatlingAttack)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                        int totalProjectiles = death ? 36 : 30;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        float velocity2 = Main.getGoodWorld ? 10f : 5f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity2);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 projectileVelocity = spinningPoint.RotatedBy(radians * k);
                            Vector2 spawnOffset = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 50f;
                            float randomSpeed = Main.rand.NextFloat(0.8f, death ? 1.5f : 1.2f);

                            int dustType = 74;
                            Vector2 dustVelocity = projectileVelocity * randomSpeed;
                            for (int l = 0; l < 5; l++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity.X, dustVelocity.Y);
                                Main.dust[dust].scale = 1.4f;
                            }

                            float ai0 = Main.rand.Next(3);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), adjustProjectileShootLocation ? NPC.Center : spawnOffset, projectileVelocity * randomSpeed, type, GreenCloudDamage, 0f, Main.myPlayer, ai0);
                        }
                    }

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                    NPC.ai[1] = -SeedGatlingDuration;
                }
            }
            else
            {
                NPC.ai[1] = 0f;

                // Slow down for a while after tentacles are spawned
                if (slowedDuringTentaclePhase)
                    velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(1f - NPC.ai[2] / TentaclePhaseSlowDuration, 2D));

                // Prepare to charge
                // More charges are used in a row at lower HP
                if (doneWithTentaclePhase && !charging && !despawn)
                {
                    float timeToChargeIncrement = phase4 ? 2f : phase3 ? 1.5f : 1f;
                    if (death)
                        timeToChargeIncrement *= 2f;

                    NPC.ai[3] += timeToChargeIncrement;
                    if (NPC.ai[3] >= ChargePhaseGateValue)
                        NPC.ai[3] = -2f;
                }
            }

            // Move slowly for a bit after finishing gatling attack
            bool slowedAfterGatlingAttack = NPC.ai[1] < 0f && !phase2;
            if (slowedAfterGatlingAttack)
            {
                float absValueOfTimer = Math.Abs(NPC.ai[1]);
                velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(absValueOfTimer / SeedGatlingDuration, 2D));

                // Shoot homing pink bulb projectiles that leave behind lingering pink clouds
                float shootBulbGateValue = death ? 150f : 120f;
                if (addSporeGasBlastToGatlingAttack)
                    shootBulbGateValue *= 0.8f;

                if (absValueOfTimer % shootBulbGateValue == 0f)
                {
                    float projectileSpeed = 9f;
                    int projectileType = ModContent.ProjectileType<HomingGasBulb>();
                    Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                    Vector2 spawnOffset = NPC.Center + projectileVelocity * 70f;

                    int dustType = 73;
                    Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                    for (int k = 0; k < 5; k++)
                    {
                        int dust = Dust.NewDust(spawnOffset, 18, 18, dustType, dustVelocity.X, dustVelocity.Y);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.4f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), adjustProjectileShootLocation ? NPC.Center : spawnOffset, projectileVelocity * projectileSpeed, projectileType, GasBulbDamage, 0f, Main.myPlayer);
                }
            }

            if (charging)
            {
                // Slow down and return to normal behavior
                if (NPC.ai[3] <= BeginChargeSlowDownGateValue)
                {
                    NPC.velocity *= chargeDeceleration;
                    float timeToDecelerateDecrement = phase4 ? 1.5f : 1f;
                    NPC.ai[3] -= timeToDecelerateDecrement;
                    if (NPC.ai[3] <= StopChargeGateValue)
                    {
                        bool canChargeAgain = phase4 || (phase3 && Main.rand.NextBool());
                        bool chargeAgain = canChargeAgain && death && calamityGlobalNPC.newAI[2] == 0f;
                        NPC.ai[3] = chargeAgain ? -2f : 0f;
                        calamityGlobalNPC.newAI[2] = (death && calamityGlobalNPC.newAI[2] == 0f && chargeAgain) ? 1f : 0f;
                        NPC.SyncExtraAI();

                        if (!secondCharge)
                        {
                            // Spawn a few tentacles
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // If the most likely loop condition to be false isn't met, don't run the second one, this is more efficient
                                if (NPC.CountNPCS(NPCID.PlanterasTentacle) < maxTentaclesAfterFirstTentaclePhase)
                                {
                                    if (NPC.CountNPCS(ModContent.NPCType<PlanterasFreeTentacle>()) < maxFreeTentaclesAfterFirstTentaclePhase)
                                    {
                                        for (int i = 0; i < maxTentaclesAfterFirstTentaclePhase; i++)
                                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasTentacle, NPC.whoAmI, 0f, 0f, 1f, 0f);
                                    }
                                }
                            }
                        }
                    }
                }

                // Maintain charge velocity
                // Emit spore gas in phase 3
                else if (NPC.ai[3] <= BeginChargeGateValue)
                {
                    NPC.damage = (int)Math.Round(ContactDamageCorrection * Phase2ContactDamageMult * DashDamageMult);
                    float sporeGasDashGateValue = death ? 6f : 9f;
                    if (phase3 && NPC.ai[3] % sporeGasDashGateValue == 0f)
                    {
                        int projectileType = ModContent.ProjectileType<SporeGasPlantera>();
                        float randomVelocityMultiplier = secondCharge ? 0.05f : death ? 0.3f : 0.2f;
                        Vector2 projectileVelocity = NPC.velocity * Main.rand.NextVector2CircularEdge(randomVelocityMultiplier, randomVelocityMultiplier);
                        Vector2 spawnOffset = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 30f;

                        int dustType = 74;
                        Vector2 dustVelocity = projectileVelocity;
                        for (int k = 0; k < 5; k++)
                        {
                            int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity.X, dustVelocity.Y);
                            Main.dust[dust].scale = 1.4f;
                        }

                        float ai0 = Main.rand.Next(3);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnOffset, projectileVelocity, projectileType, GasBulbDamage, 0f, Main.myPlayer, ai0);
                    }

                    NPC.ai[3] -= 1f;
                    if (NPC.ai[3] <= BeginChargeSlowDownGateValue)
                        NPC.ai[3] = BeginChargeSlowDownGateValue;
                }

                // Move a specified distance away from the target and charge once that distance is reached
                else
                {
                    // Line up before charging
                    if (NPC.Calamity().newAI[0] == 0f)
                    {
                        NPC.Calamity().newAI[0] = Math.Sign((NPC.Center - Main.player[NPC.target].Center).X);
                        NPC.SyncExtraAI();
                    }

                    Vector2 destination = Main.player[NPC.target].Center + new Vector2(NPC.Calamity().newAI[0], 0);
                    Vector2 distanceFromDestination = destination - NPC.Center;
                    Vector2 desiredVelocity = (distanceFromDestination - NPC.velocity).SafeNormalize(Vector2.UnitY) * chargeLineUpVelocity;

                    if (Vector2.Distance(NPC.Center, destination) > ReduceSpeedForChargeDistance)
                        NPC.SimpleFlyMovement(desiredVelocity, chargeLineUpAcceleration);
                    else
                        NPC.velocity *= 0.98f;

                    // Emit dust to show that a spore and charge attack are about to happen
                    float dustEmitAmount = Math.Abs(BeginChargeGateValue) - Math.Abs(NPC.ai[3]);
                    int dustInXChanceMin = 2;
                    int dustInXChanceMax = 8;
                    int dustChance = (int)Math.Round(MathHelper.Lerp(dustInXChanceMin, dustInXChanceMax, 1f - dustEmitAmount / Math.Abs(BeginChargeGateValue)));
                    if (Main.rand.NextBool(dustChance))
                    {
                        SpawnSporeTelegraphDust();
                    }

                    float timeToLineUpChargeDecrement = phase4 ? 2f : 1f;
                    if (death)
                        timeToLineUpChargeDecrement *= 2f;

                    NPC.ai[3] -= timeToLineUpChargeDecrement;
                    if (NPC.ai[3] <= BeginChargeGateValue)
                    {
                        // Charge
                        NPC.ai[3] = BeginChargeGateValue;
                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * chargeVelocity;
                        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);

                        // Spore dust cloud
                        Vector2 dustVelocity = NPC.velocity * -0.25f;
                        for (int k = 0; k < 30; k++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.JungleSpore, dustVelocity.X, dustVelocity.Y, 250, default, 0.8f);
                            dust.fadeIn = 0.7f;
                        }

                        // Vomit spread of spore gas
                        int totalProjectiles = secondCharge ? 6 : 12;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        float velocity2 = Main.getGoodWorld ? 10f : 5f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity2);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 projectileVelocity = spinningPoint.RotatedBy(radians * k);
                            Vector2 spawnOffset = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 50f;
                            float randomSpeed = Main.rand.NextFloat(0.8f, secondCharge ? 1f : death ? 1.5f : 1.2f);

                            int dustType = 74;
                            Vector2 dustVelocity2 = projectileVelocity * randomSpeed;
                            for (int l = 0; l < 5; l++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity2.X, dustVelocity2.Y);
                                Main.dust[dust].scale = 1.4f;
                            }

                            float ai0 = Main.rand.Next(3);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnOffset, projectileVelocity * randomSpeed, type, GasBulbDamage, 0f, Main.myPlayer, ai0);
                        }
                    }

                    // Rotation
                    float rotationX = Main.player[NPC.target].Center.X - NPC.Center.X;
                    float rotationY = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                    NPC.rotation = (float)Math.Atan2(rotationY, rotationX) + MathHelper.PiOver2;
                }
            }
            else
            {
                // Velocity ranges from 4 to 7.2, Acceleration ranges from 0.04 to 0.072, non-enraged phase 1
                // Velocity ranges from 7 to 12.6, Acceleration ranges from 0.07 to 0.126, non-enraged phase 2
                // Velocity ranges from 9 to 16.2, Acceleration ranges from 0.07 to 0.126, non-enraged phase 3
                // Velocity ranges from 17 to 30.6, Acceleration ranges from 0.15 to 0.27, enraged phase 3

                // Distance Plantera can travel from her hooks
                float maxDistanceFromHooks = enrage ? 1000f : 600f;
                if (phase3)
                    maxDistanceFromHooks += 150f;
                if (death)
                {
                    maxDistanceFromHooks += maxDistanceFromHooks * 0.2f * ((1f - lifeRatio) / 2);
                    maxDistanceFromHooks += 200f;
                }

                if (distanceFromTarget >= maxDistanceFromHooks)
                {
                    distanceFromTarget = maxDistanceFromHooks / distanceFromTarget;
                    maxVelocityX *= distanceFromTarget;
                    maxVelocityY *= distanceFromTarget;
                }

                hookPositionX += maxVelocityX;
                hookPositionY += maxVelocityY;
                npcCenterAccountingForHooks = NPC.Center;
                maxVelocityX = hookPositionX - npcCenterAccountingForHooks.X;
                maxVelocityY = hookPositionY - npcCenterAccountingForHooks.Y;
                distanceFromTarget = (float)Math.Sqrt(maxVelocityX * maxVelocityX + maxVelocityY * maxVelocityY);

                if (distanceFromTarget < velocity)
                {
                    maxVelocityX = NPC.velocity.X;
                    maxVelocityY = NPC.velocity.Y;
                }
                else
                {
                    distanceFromTarget = velocity / distanceFromTarget;
                    maxVelocityX *= distanceFromTarget;
                    maxVelocityY *= distanceFromTarget;
                }

                if (NPC.velocity.X < maxVelocityX)
                {
                    NPC.velocity.X += acceleration;
                    if (NPC.velocity.X < 0f && maxVelocityX > 0f)
                        NPC.velocity.X += acceleration * 2f;
                }
                else if (NPC.velocity.X > maxVelocityX)
                {
                    NPC.velocity.X -= acceleration;
                    if (NPC.velocity.X > 0f && maxVelocityX < 0f)
                        NPC.velocity.X -= acceleration * 2f;
                }
                if (NPC.velocity.Y < maxVelocityY)
                {
                    NPC.velocity.Y += acceleration;
                    if (NPC.velocity.Y < 0f && maxVelocityY > 0f)
                        NPC.velocity.Y += acceleration * 2f;
                }
                else if (NPC.velocity.Y > maxVelocityY)
                {
                    NPC.velocity.Y -= acceleration;
                    if (NPC.velocity.Y > 0f && maxVelocityY < 0f)
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // Rotation
                float rotationX = Main.player[NPC.target].Center.X - NPC.Center.X;
                float rotationY = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                NPC.rotation = (float)Math.Atan2(rotationY, rotationX) + MathHelper.PiOver2;
            }

            // Phase 1
            if (!phase2)
            {
                // Emit light
                Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.8f, 0.2f, 0.4f);

                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                NPC.defense = 32;

                // Fire projectiles
                if (!usingSeedGatling && !slowedAfterGatlingAttack)
                {
                    float shootBoost = 2f * (1f - lifeRatio);
                    NPC.localAI[1] += 1f + shootBoost;

                    if (enrage)
                        NPC.localAI[1] += 2f;

                    if (Main.getGoodWorld)
                        NPC.localAI[1] += 1f;

                    float shootProjectileGateValue = death ? 40f : 60f;
                    if (NPC.localAI[1] >= shootProjectileGateValue)
                    {
                        NPC.localAI[1] = 0f;

                        bool shootThornBall = false;
                        if (useNewGatlingAttackVariant)
                        {
                            CheckThornBallLimit(ref shootThornBall);
                        }

                        bool shootPoisonSeed = (Main.getGoodWorld || Main.rand.NextBool(death ? 2 : 4)) && !shootThornBall;
                        int projectileType = shootThornBall ? ProjectileID.ThornBall : shootPoisonSeed ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                        float projectileSpeed = death ? 16f : 14f;
                        int damage = shootThornBall ? ThornBallDamage : shootPoisonSeed ? PoisonSeedDamage : PinkSeedDamage;
                        Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        Vector2 spawnOffset = NPC.Center + projectileVelocity * 70f;

                        int dustType = shootPoisonSeed ? 74 : 73;
                        int dustSpawnBoxSize = shootThornBall ? 38 : 14;
                        int dustAmount = shootThornBall ? 15 : 5;
                        Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                        SpawnSeedDust(spawnOffset, dustSpawnBoxSize, dustType, dustVelocity, dustAmount);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), adjustProjectileShootLocation ? NPC.Center : spawnOffset, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                            if (projectileType == ProjectileID.ThornBall && (Main.rand.NextBool() || !Main.zenithWorld))
                                Main.projectile[proj].tileCollide = false;
                        }
                    }
                }
            }

            // Phase 2
            else
            {
                // Emit light
                Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.4f, 0.8f, 0.2f);

                // Spore dust
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore, 0f, 0f, 250, default, 0.6f);
                    dust.fadeIn = 0.7f;
                }

                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                NPC.defense = 10;

                // Spawn tentacles
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.localAI[0] == 1f)
                    {
                        NPC.localAI[0] = 2f;
                        int totalTentacles = death ? 11 : 8;
                        if (Main.getGoodWorld)
                            totalTentacles *= 2;

                        for (int i = 0; i < totalTentacles; i++)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasTentacle, NPC.whoAmI);

                        if (Main.getGoodWorld)
                        {
                            foreach (NPC n in Main.ActiveNPCs)
                            {
                                if (n.aiStyle == NPCAIStyleID.PlanteraHook)
                                {
                                    for (int j = 0; j < totalTentacles / 2 - 1; j++)
                                    {
                                        int hookIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasTentacle, NPC.whoAmI);
                                        Main.npc[hookIndex].ai[3] = n.whoAmI + 1;
                                    }
                                }
                            }
                        }
                    }
                }

                // Slow down for 20 seconds after transitioning to phase 2
                // This gives players time to handle the tentacles before Plantera starts attack again
                // Decrement the timer far faster if there aren't any tentacles alive
                if (NPC.ai[2] == 0f)
                    NPC.ai[2] = TentaclePhaseSlowDuration;

                if (slowedDuringTentaclePhase)
                {
                    bool noAttachedTentacles = !NPC.AnyNPCs(NPCID.PlanterasTentacle);
                    bool noFreeTentacles = !NPC.AnyNPCs(ModContent.NPCType<PlanterasFreeTentacle>());
                    float tentacleIdleTimerDecrement = (noAttachedTentacles && noFreeTentacles) ? 4f : noAttachedTentacles ? 2f : 1f;
                    if (death)
                        tentacleIdleTimerDecrement *= 2f;

                    NPC.ai[2] -= tentacleIdleTimerDecrement;
                    if (NPC.ai[2] <= 0f)
                        NPC.ai[2] = -1f;
                }

                // Spawn gore
                if (NPC.localAI[2] == 0f)
                {
                    if (!Main.dedServ)
                    {
                        Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 378, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 379, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 380, NPC.scale);
                    }
                    NPC.localAI[2] = 1f;
                }

                if (!charging)
                {
                    // Fire spreads of poison seeds
                    NPC.localAI[3] += 1f;
                    float shootProjectileGateValue = slowedDuringTentaclePhase ? 120f : 90f;
                    if (NPC.localAI[3] >= shootProjectileGateValue)
                    {
                        float projectileSpeed = 14f;

                        Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);

                        int spread = 8 + (int)Math.Round((0.5f - lifeRatio) * 16f); // 8 to 16, wider spread is harder to avoid
                        int numProj = spread / 2;

                        // Always an odd number of projectiles
                        if (numProj % 2 == 0)
                            numProj++;

                        int type = ProjectileID.PoisonSeedPlantera;
                        int damage = PoisonSeedDamage;
                        float rotation = MathHelper.ToRadians(spread);

                        for (int i = 0; i < numProj; i++)
                        {
                            bool shootPinkSeed = i % 2 == 0;
                            if (shootPinkSeed)
                            {
                                type = ProjectileID.SeedPlantera;
                                damage = PinkSeedDamage;
                            }
                            else
                                type = ProjectileID.PoisonSeedPlantera;

                            Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            Vector2 spawnOffset = NPC.Center + perturbedSpeed * 50f;

                            int dustType = shootPinkSeed ? 73 : 74;
                            Vector2 dustVelocity = perturbedSpeed * projectileSpeed;
                            for (int k = 0; k < 5; k++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 14, 14, dustType, dustVelocity.X, dustVelocity.Y);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1.4f;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnOffset, perturbedSpeed * projectileSpeed * 0.5f, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileSpeed);
                        }

                        if (death)
                        {
                            bool shootThornBall = true;
                            CheckThornBallLimit(ref shootThornBall);

                            if (shootThornBall)
                            {
                                type = ProjectileID.ThornBall;
                                damage = ThornBallDamage;
                                Vector2 spawnOffset = NPC.Center + projectileVelocity * 50f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float ai2 = 0f;
                                    if (Main.rand.NextBool() || !Main.zenithWorld)
                                        ai2 = 1f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnOffset, projectileVelocity * projectileSpeed, type, damage, 0f, Main.myPlayer, 0f, 0f, ai2);
                                }
                            }
                        }

                        if (death && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float sporeSpeed = 12f;
                            Vector2 sporeVelocity = projectileVelocity * sporeSpeed;
                            int spore = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.Spore);
                            Main.npc[spore].velocity.X = sporeVelocity.X;
                            Main.npc[spore].velocity.Y = sporeVelocity.Y;
                            Main.npc[spore].netUpdate = true;
                        }

                        NPC.localAI[3] = 0f;
                    }
                }
            }

            // Heal if on surface
            if (surface)
            {
                if (Main.rand.NextBool(Main.IsItDay() ? 3 : 6))
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie, 0f, 0f, 200, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].fadeIn = 1.3f;
                    Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101)).SafeNormalize(Vector2.UnitY);
                    vector *= Main.rand.Next(50, 100) * 0.04f;
                    Main.dust[dust].velocity = vector;
                    vector = vector.SafeNormalize(Vector2.UnitY);
                    vector *= 86f;
                    Main.dust[dust].position = NPC.Center - vector;
                }

                // Heal, 100 (50 during daytime) seconds to reach full HP from 0
                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] >= (Main.IsItDay() ? 30f : 60f))
                {
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.SyncExtraAI();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = NPC.lifeMax / 100;
                        if (healAmt > NPC.lifeMax - NPC.life)
                            healAmt = NPC.lifeMax - NPC.life;

                        if (healAmt > 0)
                        {
                            NPC.life += healAmt;
                            NPC.HealEffect(healAmt, true);
                            NPC.netUpdate = true;
                        }
                    }
                }
            }

            if (NPC.ai[0] == 0f && NPC.life > 0)
                NPC.ai[0] = NPC.lifeMax;

            if (NPC.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healthInterval = death ? (int)(NPC.lifeMax * 0.03) : (int)(NPC.lifeMax * 0.04);
                    if ((NPC.life + healthInterval) < NPC.ai[0])
                    {
                        NPC.ai[0] = NPC.life;

                        if (phase2)
                        {
                            int spore = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.Spore, NPC.whoAmI);
                            float sporeSpeed = death ? 8f : 6f;
                            Vector2 sporeVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * sporeSpeed;
                            Main.npc[spore].velocity.X = sporeVelocity.X;
                            Main.npc[spore].velocity.Y = sporeVelocity.Y;
                            Main.npc[spore].netUpdate = true;
                        }
                    }
                }
            }

            return false;
        }

        public override void PostDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            Texture2D npcTexture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var originalColor = NPC.GetAlpha(drawColor);
            Color newColor = new Color(100, 255, 100, 255);
            Vector2 glowOffset = Vector2.UnitY * -4f;
            Vector2 drawPosition = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) + glowOffset;
            var origin = NPC.frame.Size() / 2;

            bool phase2 = lifeRatio <= 0.5f;
            if (!phase2)
            {
                float telegraphTimer = Math.Abs(NPC.ai[1]);
                bool startSeedGatlingSporeGasTelegraph = NPC.ai[1] > SeedGatlingColorChangeGateValue;
                bool endSeedGatlingSporeGasTelegraph = NPC.ai[1] < -SeedGatlingDuration + SeedGatlingColorChangeDuration;
                if (startSeedGatlingSporeGasTelegraph)
                {
                    float telegraphScalar = MathHelper.Clamp((telegraphTimer - SeedGatlingColorChangeGateValue) / SeedGatlingColorChangeDuration, 0f, 1f);
                    Color telegraphColor = Color.Lerp(originalColor, newColor, telegraphScalar);
                    spriteBatch.Draw(npcTexture, drawPosition, NPC.frame, telegraphColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }

                // -300 to -120
                else if (endSeedGatlingSporeGasTelegraph)
                {
                    float telegraphScalar = MathHelper.Clamp((telegraphTimer - (SeedGatlingDuration - SeedGatlingColorChangeDuration)) / SeedGatlingColorChangeDuration, 0f, 1f);
                    Color telegraphColor = Color.Lerp(originalColor, newColor, telegraphScalar);
                    spriteBatch.Draw(npcTexture, drawPosition, NPC.frame, telegraphColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }
            else
            {
                float telegraphTimer = Math.Abs(NPC.ai[3]);
                bool startChargeTelegraph = NPC.ai[3] > ChargeTelegraphColorChangeGateValue;
                bool endChargeTelegraph = NPC.ai[3] <= -2f;
                if (startChargeTelegraph)
                {
                    float telegraphScalar = MathHelper.Clamp((telegraphTimer - ChargeTelegraphColorChangeGateValue) / SeedGatlingColorChangeDuration, 0f, 1f);
                    Color telegraphColor = Color.Lerp(originalColor, newColor, telegraphScalar);
                    spriteBatch.Draw(npcTexture, drawPosition, NPC.frame, telegraphColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }

                // -195 to -2
                else if (endChargeTelegraph)
                {
                    float telegraphScalar = MathHelper.Clamp((Math.Abs(StopChargeGateValue) - telegraphTimer) / Math.Abs(StopChargeGateValue), 0f, 1f);
                    Color telegraphColor = Color.Lerp(originalColor, newColor, telegraphScalar);

                    if (CalamityClientConfig.Instance.Afterimages)
                    {
                        int afterimageAmount = 10;
                        int afterImageIncrement = 2;
                        for (int j = 0; j < afterimageAmount; j += afterImageIncrement)
                        {
                            Color afterimageColor = telegraphColor;
                            afterimageColor = Color.Lerp(afterimageColor, originalColor, 0.5f);
                            afterimageColor = NPC.GetAlpha(afterimageColor);
                            afterimageColor *= (afterimageAmount - j) / 15f;
                            Vector2 afterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                            afterimagePos -= new Vector2(npcTexture.Width, npcTexture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                            afterimagePos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY) + glowOffset;
                            spriteBatch.Draw(npcTexture, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                        }
                    }

                    spriteBatch.Draw(npcTexture, drawPosition, NPC.frame, telegraphColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }
        }

        public class HookAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                // Variables
                bool enrage = false;
                bool despawn = false;
                bool death = CalamityWorld.death || enrage;

                // Despawn if Plantera is gone
                if (NPC.plantBoss < 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                // Percent life remaining, Plantera
                float lifeRatio = Main.npc[NPC.plantBoss].life / (float)Main.npc[NPC.plantBoss].lifeMax;

                // Despawn if Plantera's target is dead
                if (Main.player[Main.npc[NPC.plantBoss].target].dead && !enrage)
                    despawn = true;

                // Enrage if Plantera's target is on the surface
                if (!enrage && !BossRushEvent.BossRushActive && ((Main.player[Main.npc[NPC.plantBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[NPC.plantBoss].target].position.Y > Main.UnderworldLayer * 16) | despawn))
                {
                    NPC.localAI[0] -= 4f;
                    enrage = true;
                }

                // Set centers for movement
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (NPC.ai[0] == 0f)
                        NPC.ai[0] = (int)(NPC.Center.X / 16f);
                    if (NPC.ai[1] == 0f)
                        NPC.ai[1] = (int)(NPC.Center.X / 16f);
                }

                // Find new spot to move to after set time has passed
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Move immediately
                    if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                        NPC.localAI[0] = 0f;

                    // Timer dictating whether to pick a new location or not
                    float moveBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                    NPC.localAI[0] -= 1f + moveBoost;
                    if (enrage)
                        NPC.localAI[0] -= 6f;

                    // Set timer to new amount if a different hook is currently moving
                    if (!despawn && NPC.localAI[0] <= 0f && NPC.ai[0] != 0f)
                    {
                        foreach (NPC n in Main.ActiveNPCs)
                        {
                            if (n.whoAmI != NPC.whoAmI && n.type == NPC.type && (n.velocity.X != 0f || n.velocity.Y != 0f))
                                NPC.localAI[0] = Main.rand.Next(60, 301);
                        }
                    }

                    // Pick a location to move to
                    if (NPC.localAI[0] <= 0f)
                    {
                        // Reset timer
                        NPC.localAI[0] = Main.rand.Next(300, 601);

                        // Pick location
                        bool hookCanMove = false;
                        int hookMoveTries = 0;
                        while (!hookCanMove && hookMoveTries <= 1000)
                        {
                            hookMoveTries++;

                            int targetTilePosX = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.X / 16f);
                            int targetTilePosY = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.Y / 16f);

                            if (NPC.ai[0] == 0f)
                            {
                                targetTilePosX = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.X + Main.npc[NPC.plantBoss].Center.X) / 32f);
                                targetTilePosY = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.Y + Main.npc[NPC.plantBoss].Center.Y) / 32f);
                            }

                            if (despawn)
                            {
                                targetTilePosX = (int)Main.npc[NPC.plantBoss].position.X / 16;
                                targetTilePosY = (int)(Main.npc[NPC.plantBoss].position.Y + 400f) / 16;
                            }

                            int hookTileOffset = 20;
                            hookTileOffset += (int)(100f * (hookMoveTries / 1000f));
                            int hookTileX = targetTilePosX + Main.rand.Next(-hookTileOffset, hookTileOffset + 1);
                            int hookTileY = targetTilePosY + Main.rand.Next(-hookTileOffset, hookTileOffset + 1);

                            try
                            {
                                if (WorldGen.SolidTile(hookTileX, hookTileY) || (Main.tile[hookTileX, hookTileY].WallType > WallID.None && (hookMoveTries > 500 || lifeRatio < 0.5f)))
                                {
                                    hookCanMove = true;
                                    NPC.ai[0] = hookTileX;
                                    NPC.ai[1] = hookTileY;
                                    NPC.netUpdate = true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                // Movement to new location
                if (NPC.ai[0] > 0f && NPC.ai[1] > 0f)
                {
                    // Hook movement velocity
                    float velocityBoost = death ? 6f * (1f - lifeRatio) : 3f * (1f - lifeRatio);
                    float velocity = 7f + velocityBoost;
                    if (enrage)
                        velocity *= 2f;
                    if (despawn)
                        velocity *= 2f;

                    // Moving to new location
                    Vector2 hookCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float hookMoveX = NPC.ai[0] * 16f - 8f - hookCenter.X;
                    float hookMoveY = NPC.ai[1] * 16f - 8f - hookCenter.Y;
                    float hookMoveDistance = (float)Math.Sqrt(hookMoveX * hookMoveX + hookMoveY * hookMoveY);
                    if (hookMoveDistance < 12f + velocity)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && NPC.localAI[3] == 1f)
                        {
                            NPC.localAI[3] = 0f;
                            WorldGen.SpawnPlanteraThorns(NPC.Center);
                        }

                        NPC.velocity.X = hookMoveX;
                        NPC.velocity.Y = hookMoveY;
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                            NPC.localAI[3] = 1f;

                        hookMoveDistance = velocity / hookMoveDistance;
                        NPC.velocity.X = hookMoveX * hookMoveDistance;
                        NPC.velocity.Y = hookMoveY * hookMoveDistance;
                    }

                    // Rotation
                    Vector2 hookCenterRotation = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float plantXDirection = Main.npc[NPC.plantBoss].Center.X - hookCenterRotation.X;
                    float plantYDirection = Main.npc[NPC.plantBoss].Center.Y - hookCenterRotation.Y;
                    NPC.rotation = (float)Math.Atan2(plantYDirection, plantXDirection) - MathHelper.PiOver2;
                }

                return false;
            }
        }

        public class TentacleAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

                // Emit light
                Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0.2f, 0.4f, 0.1f);

                // Spore dust
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore, 0f, 0f, 250, default, 0.4f);
                    dust.fadeIn = 0.7f;
                }

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                if (Main.getGoodWorld)
                {
                    if (Main.rand.NextBool(5))
                        NPC.reflectsProjectiles = true;
                    else
                        NPC.reflectsProjectiles = false;
                }

                // Die if Plantera is gone
                if (NPC.plantBoss < 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                // Set Plantera to a variable
                int plantBoss = NPC.plantBoss;

                // Retract to be near Plantera while she's charging
                bool planteraIsCharging = Main.npc[plantBoss].ai[3] <= -2f;

                // Become free if Plantera gets sick of your shit
                if (Main.npc[plantBoss].ai[2] == -1f && NPC.ai[2] != 1f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                // 3 seconds of extending from Plantera to max length to prevent unfair hits
                float extendTime = 180f;
                if (planteraIsCharging)
                {
                    if (NPC.localAI[0] > 0f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.SyncExtraAI();
                    }
                }
                else if (NPC.localAI[0] < extendTime)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= extendTime)
                        NPC.SyncExtraAI();
                }

                // Movement variables
                int maxOffset = 100;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                    {
                        NPC.ai[0] = Main.rand.Next(-maxOffset, maxOffset + 1);
                        NPC.ai[1] = Main.rand.Next(-maxOffset, maxOffset + 1);
                        NPC.netUpdate = true;
                    }
                }

                // Velocity and acceleration
                float tentacleAcceleration = 1.6f;
                float extendedDistanceFromPlantera = Math.Abs(NPC.ai[0] + NPC.ai[1]) / maxOffset;
                float tentacleDistance = MathHelper.Lerp(50f, 100f + (extendedDistanceFromPlantera * 300f), NPC.localAI[0] / extendTime);
                float deceleration = (death ? 0.5f : 0.8f) / (1f + extendedDistanceFromPlantera);

                if (death)
                {
                    tentacleAcceleration *= 1.2f;
                    extendedDistanceFromPlantera *= 1.1f;
                    tentacleDistance *= 1.2f;
                    deceleration *= 0.75f;
                }

                if (Main.getGoodWorld)
                    tentacleAcceleration += 4f;

                // Fast retraction
                if (planteraIsCharging)
                {
                    tentacleAcceleration *= 2f;
                    deceleration *= 0.5f;
                }

                // Despawn if Plantera is gone
                if (!Main.npc[plantBoss].active)
                {
                    NPC.active = false;
                    return false;
                }

                // Movement
                Vector2 planteraCenter = Main.npc[plantBoss].Center;
                float plantXOffset = planteraCenter.X + NPC.ai[0];
                float plantYOffset = planteraCenter.Y + NPC.ai[1];
                float plantXDist = plantXOffset - planteraCenter.X;
                float plantYDist = plantYOffset - planteraCenter.Y;
                float plantTotalDist = (float)Math.Sqrt(plantXDist * plantXDist + plantYDist * plantYDist);
                plantTotalDist = tentacleDistance / plantTotalDist;
                plantXDist *= plantTotalDist;
                plantYDist *= plantTotalDist;

                if (NPC.position.X < planteraCenter.X + plantXDist)
                {
                    NPC.velocity.X += tentacleAcceleration;
                    if (NPC.velocity.X < 0f && plantXDist > 0f)
                        NPC.velocity.X *= deceleration;
                }
                else if (NPC.position.X > planteraCenter.X + plantXDist)
                {
                    NPC.velocity.X -= tentacleAcceleration;
                    if (NPC.velocity.X > 0f && plantXDist < 0f)
                        NPC.velocity.X *= deceleration;
                }
                if (NPC.position.Y < planteraCenter.Y + plantYDist)
                {
                    NPC.velocity.Y += tentacleAcceleration;
                    if (NPC.velocity.Y < 0f && plantYDist > 0f)
                        NPC.velocity.Y *= deceleration;
                }
                else if (NPC.position.Y > planteraCenter.Y + plantYDist)
                {
                    NPC.velocity.Y -= tentacleAcceleration;
                    if (NPC.velocity.Y > 0f && plantYDist < 0f)
                        NPC.velocity.Y *= deceleration;
                }

                float velocityLimit = 12f + 6f * extendedDistanceFromPlantera;
                if (planteraIsCharging)
                    velocityLimit *= 1.5f;
                if (NPC.velocity.X > velocityLimit)
                    NPC.velocity.X = velocityLimit;
                if (NPC.velocity.X < -velocityLimit)
                    NPC.velocity.X = -velocityLimit;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
                if (NPC.velocity.Y < -velocityLimit)
                    NPC.velocity.Y = -velocityLimit;

                // Direction and rotation
                if (plantXDist > 0f)
                {
                    NPC.spriteDirection = 1;
                    NPC.rotation = (float)Math.Atan2(plantYDist, plantXDist);
                }
                if (plantXDist < 0f)
                {
                    NPC.spriteDirection = -1;
                    NPC.rotation = (float)Math.Atan2(plantYDist, plantXDist) + MathHelper.Pi;
                }

                return false;
            }
        }
    }
}
