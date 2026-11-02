using System;
using CalamityMod.Events;
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
    public class SkeletronAI : VanillaAIOverride
    {
        public const float ChargeGateValue = 600f;
        public const float ChargeTelegraphTime = 120f;
        public const float HandSlapGateValue = 300f;
        public const float HandSlapTelegraphTime = 120f;
        public const float HandSwipeDistance = 960f; // 60 tiles
        public const float HandSwipeDistance_Master = 1280f; // 80 tiles

        // Vanilla values
        public static float SpinDamageMult = 1.3f; // 91
        public static int SkullDamage = 17; // 68; Also applies to crossbones

        public override void SetDefaults(Mod mod)
        {
            DisableMultiplayerSmoothing = true;
        }

        private void SpawnCasterNearHead(int npcType)
        {
            for (int i = 0; i < 1000; i++)
            {
                int headYAcceleration = (int)(NPC.Center.X / 16f) + Main.rand.Next(-50, 51);
                int headYTopSpeed;
                for (headYTopSpeed = (int)(NPC.Center.Y / 16f) + Main.rand.Next(-50, 51); headYTopSpeed < Main.maxTilesY - 10 && !WorldGen.SolidTile(headYAcceleration, headYTopSpeed); headYTopSpeed++)
                {
                }

                headYTopSpeed--;
                if (!WorldGen.SolidTile(headYAcceleration, headYTopSpeed))
                {
                    int headXAcceleration = NPC.NewNPC(NPC.GetSource_FromAI(), headYAcceleration * 16 + 8, headYTopSpeed * 16, npcType);
                    if (Main.dedServ && headXAcceleration < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, headXAcceleration);

                    break;
                }
            }
        }

        private static void FireHandSwipeSkull(NPC hand, bool death, bool cancelSlap, bool phase2, bool phase3, float handSwipeVelocity)
        {
            if (death && Collision.CanHit(hand.Center, 1, 1, Main.player[hand.target].position, Main.player[hand.target].width, Main.player[hand.target].height) && !cancelSlap)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (phase2 && Vector2.Distance(Main.player[hand.target].Center, hand.Center) > 160f)
                    {
                        float skullProjSpeed = handSwipeVelocity * (phase3 ? 0.6f : 0.2f);
                        Vector2 initialProjectileVelocity = hand.Center.DirectionTo(Main.player[hand.target].Center) * skullProjSpeed;
                        int type = ProjectileID.Skull;
                        int skullProjectile = Projectile.NewProjectile(hand.GetSource_FromAI(), hand.Center, initialProjectileVelocity, type, SkullDamage, 0f, Main.myPlayer, -(phase3 ? 2f : 1f));
                        Main.projectile[skullProjectile].timeLeft = 600;
                    }
                }
            }
        }

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            float phase2LifeRatio = death ? 1f : 0.85f;
            float phase3LifeRatio = death ? 0.9f : 0.7f;
            float respawnHandsLifeRatio = 0.5f;
            float phase4LifeRatio = death ? 0.4f : 0.3f;
            float phase5LifeRatio = death ? 0.15f : 0.1f;

            // Begin firing spreads of skulls phase
            bool phase2 = lifeRatio < phase2LifeRatio;

            // Begin using a more dangerous charge attack phase
            bool phase3 = lifeRatio < phase3LifeRatio;

            // Spawn a new set of hands, fire skulls at the end of each charge and fire skulls from hands at the end of each slap phase
            bool respawnHands = lifeRatio < respawnHandsLifeRatio;

            // Fire giant cursed skull projectiles (yes, these curse you if you get hit) during charge attack and hands fire skulls phase
            bool phase4 = lifeRatio < phase4LifeRatio;

            // Rapid teleport and charge, stop using idle phase
            bool phase5 = lifeRatio < phase5LifeRatio;

            // Set defense
            NPC.defense = NPC.defDefense;
            NPC.damage = NPC.defDamage;

            NPC.reflectsProjectiles = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            // Spawn hands
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[0] = 1f;
                    SpawnHands();
                    NPC.netUpdate = true;
                }

                // Respawn hands
                if (respawnHands && calamityGlobalNPC.newAI[0] == 0f && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 160f)
                {
                    calamityGlobalNPC.newAI[0] = 1f;
                    SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = SoundID.ForceRoar.Pitch - 0.25f }, NPC.Center);
                    SpawnHands();

                    NPC.netUpdate = true;
                    NPC.SyncExtraAI();
                }

                void SpawnHands()
                {
                    int skeletronHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.SkeletronHand, NPC.whoAmI);
                    Main.npc[skeletronHand].ai[0] = death ? -1.3f : -1f;
                    Main.npc[skeletronHand].ai[1] = NPC.whoAmI;
                    Main.npc[skeletronHand].target = NPC.target;
                    Main.npc[skeletronHand].netUpdate = true;

                    skeletronHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.SkeletronHand, NPC.whoAmI);
                    Main.npc[skeletronHand].ai[0] = death ? 1.3f : 1f;
                    Main.npc[skeletronHand].ai[1] = NPC.whoAmI;
                    Main.npc[skeletronHand].ai[3] = 150f;
                    Main.npc[skeletronHand].Calamity().newAI[2] = 150f;
                    Main.npc[skeletronHand].target = NPC.target;
                    Main.npc[skeletronHand].netUpdate = true;

                    // Spawn two additional hands with different attack timings
                    if (death)
                    {
                        skeletronHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.SkeletronHand, NPC.whoAmI);
                        Main.npc[skeletronHand].ai[0] = -1.3f;
                        Main.npc[skeletronHand].Calamity().newAI[0] = -1f;
                        Main.npc[skeletronHand].ai[1] = NPC.whoAmI;
                        Main.npc[skeletronHand].ai[3] = respawnHands ? -75f : 0f;
                        Main.npc[skeletronHand].Calamity().newAI[2] = respawnHands ? -75f : 0f;
                        Main.npc[skeletronHand].target = NPC.target;
                        Main.npc[skeletronHand].netUpdate = true;

                        skeletronHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.SkeletronHand, NPC.whoAmI);
                        Main.npc[skeletronHand].ai[0] = 1.3f;
                        Main.npc[skeletronHand].Calamity().newAI[0] = -1f;
                        Main.npc[skeletronHand].ai[1] = NPC.whoAmI;
                        Main.npc[skeletronHand].ai[3] = respawnHands ? 75f : 150f;
                        Main.npc[skeletronHand].Calamity().newAI[2] = respawnHands ? 75f : 150f;
                        Main.npc[skeletronHand].target = NPC.target;
                        Main.npc[skeletronHand].netUpdate = true;
                    }
                }
            }

            // Despawn
            if (NPC.ai[1] != 3f)
            {
                int despawnDistanceInTiles = 500;
                if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
                {
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
                        NPC.ai[1] = 3f;
                }
                else if (NPC.timeLeft < 1800)
                    NPC.timeLeft = 1800;
            }

            // Daytime enrage
            if (Main.IsItDay() && !BossRushEvent.BossRushActive && NPC.ai[1] != 3f && NPC.ai[1] != 2f)
            {
                NPC.ai[1] = 2f;
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
            }

            // Hand count
            int numHandsAlive = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.SkeletronHand)
                    numHandsAlive++;
            }

            // Hand variables
            bool handsDead = numHandsAlive == 0;
            int numProj = Main.getGoodWorld ? 22 : death ? 5 : 3;
            float spread = Main.getGoodWorld ? 180 : 60;
            float headSpinVelocityMult = phase3 ? 12f : 4.5f;

            switch (numHandsAlive)
            {
                case 0:
                    numProj = Main.getGoodWorld ? 36 : death ? 9 : 7;
                    spread = Main.getGoodWorld ? 180 : death ? 90 : 82;
                    headSpinVelocityMult = phase3 ? 12f : 6f;
                    break;

                case 1:
                    numProj = Main.getGoodWorld ? 27 : death ? 7 : 5;
                    spread = Main.getGoodWorld ? 150 : death ? 76 : 68;
                    headSpinVelocityMult = phase3 ? 11.5f : 5f;
                    break;

                case 2:
                    numProj = Main.getGoodWorld ? 18 : death ? 6 : 4;
                    spread = Main.getGoodWorld ? 140 : death ? 70 : 62;
                    headSpinVelocityMult = phase3 ? 11f : 4.5f;
                    break;

                case 3:
                    numProj = Main.getGoodWorld ? 15 : death ? 5 : 3;
                    spread = Main.getGoodWorld ? 130 : death ? 64 : 56;
                    headSpinVelocityMult = phase3 ? 10.5f : 4f;
                    break;

                case 4:
                    numProj = Main.getGoodWorld ? 12 : death ? 4 : 3;
                    spread = Main.getGoodWorld ? 120 : 56;
                    headSpinVelocityMult = phase3 ? 10f : 3.5f;
                    break;
            }

            // Reduce the amount of skulls per spread in later phases due to near-constant teleporting
            if (!death && numProj > 3)
            {
                if (phase4)
                    numProj--;
                if (phase5 && numProj > 3)
                    numProj--;
            }

            if (death)
                headSpinVelocityMult *= 1.08f; // Very volatile value

            // Velocity used to move Skeletron away from the target before charging
            float moveAwayVelocity = headSpinVelocityMult;
            if (!phase3)
                moveAwayVelocity *= 2f;

            // Hand DR, scale DR up if the hands are still alive as Skeletron's HP lowers
            NPC.chaseable = handsDead;
            float minDR = 0f;
            float maxDR = 0.9999f;
            calamityGlobalNPC.DR = !handsDead ? (float)Math.Sqrt(MathHelper.Lerp(minDR, maxDR, respawnHands ? (respawnHandsLifeRatio - lifeRatio) / respawnHandsLifeRatio : 2f - lifeRatio / respawnHandsLifeRatio)) : minDR;
            calamityGlobalNPC.unbreakableDR = !handsDead;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = !handsDead;

            // Value to start teleport dust
            int teleportGateValue = phase5 ? 180 : 300;

            // Bool to disable skull firing after charging if teleport was recent or is about to happen
            bool disableSkullsAfterCharge = NPC.ai[3] <= 60f || NPC.ai[3] > teleportGateValue + 60f;

            // Teleport while not despawning
            if (NPC.ai[1] != 3f)
            {
                int dustType = DustID.GemDiamond;

                // Post-teleport
                if (NPC.ai[3] == -60f)
                {
                    NPC.ai[3] = 0f;

                    SoundEngine.PlaySound(SoundID.Item66, NPC.Center);

                    // Fire skulls after teleport
                    if (Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileID.Skull;

                            // Inverse parabolic projectile spreads
                            Vector2 baseVel = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * (death ? 6f : 5f);
                            Vector2 firingPos = NPC.Center + baseVel * 5f;
                            float centralCount = 0.5f * (numProj - 1f);
                            for (int i = 0; i < numProj; i++)
                            {
                                float offset = MathHelper.ToRadians(MathHelper.Lerp(-spread * 0.5f, spread * 0.5f, i / (numProj - 1f)));
                                float velocityMult = MathHelper.Lerp(0.5f, 1.5f, MathF.Abs(centralCount - i) / centralCount);
                                Projectile shot = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), firingPos, baseVel.RotatedBy(offset) * velocityMult, type, SkullDamage, 0f, Main.myPlayer, -2f);
                                shot.timeLeft = 600;
                            }

                            NPC.netUpdate = true;
                        }
                    }

                    // Teleport dust
                    for (int m = 0; m < 30; m++)
                    {
                        int teleportDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                        Main.dust[teleportDust].noGravity = true;
                        Main.dust[teleportDust].velocity.X *= 2f;
                    }
                }

                // Teleport after a certain time
                // If hands are dead: 7 seconds
                // If hands are not dead: 14 seconds
                // If hands are dead in phase 2: 4.7 seconds
                NPC.ai[3] += 1f + (((phase2 && handsDead) || phase4) ? 0.5f : 0f) - (handsDead ? 0f : 0.5f);

                // Dust to show teleport
                int ai3 = (int)NPC.ai[3]; // 0 to 30, and -60

                if (NPC.localAI[0] == 1f && calamityGlobalNPC.newAI[2] == 0f && calamityGlobalNPC.newAI[3] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 skullFaceDirection = NPC.Center + new Vector2(NPC.direction * 20, 6f);
                        Vector2 skullTargetDirection = Main.player[NPC.target].Center - skullFaceDirection;
                        Point skullTileCoords = NPC.Center.ToTileCoordinates();
                        Point targetTileCoords = Main.player[NPC.target].Center.ToTileCoordinates();
                        int randomTeleportOffset = 20 - (int)Math.Ceiling(MathHelper.Lerp(0f, 10f, 1f - lifeRatio));
                        int skullPositionOffset = 4;
                        int targetPositionOffset = randomTeleportOffset - 4;
                        int teleportTries = 0;

                        bool targetTooFar = false;
                        if (skullTargetDirection.Length() > 2000f)
                            targetTooFar = true;

                        while (!targetTooFar && teleportTries < 100)
                        {
                            teleportTries++;
                            int teleportTileX = Main.rand.Next(targetTileCoords.X - randomTeleportOffset, targetTileCoords.X + randomTeleportOffset + 1);
                            int teleportTileY = Main.rand.Next(targetTileCoords.Y - randomTeleportOffset, targetTileCoords.Y + randomTeleportOffset + 1);
                            if ((teleportTileY < targetTileCoords.Y - targetPositionOffset || teleportTileY > targetTileCoords.Y + targetPositionOffset || teleportTileX < targetTileCoords.X - targetPositionOffset || teleportTileX > targetTileCoords.X + targetPositionOffset) && (teleportTileY < skullTileCoords.Y - skullPositionOffset || teleportTileY > skullTileCoords.Y + skullPositionOffset || teleportTileX < skullTileCoords.X - skullPositionOffset || teleportTileX > skullTileCoords.X + skullPositionOffset) && !Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                            {
                                // New location params
                                calamityGlobalNPC.newAI[2] = teleportTileX * 16 - NPC.width / 2;
                                calamityGlobalNPC.newAI[3] = teleportTileY * 16 - NPC.height;
                                NPC.SyncExtraAI();
                                break;
                            }
                        }
                    }
                }

                // Teleport location telegraph
                if (calamityGlobalNPC.newAI[2] != 0f && calamityGlobalNPC.newAI[3] != 0f)
                {
                    for (int m = 0; m < 5; m++)
                    {
                        Vector2 position = new Vector2(calamityGlobalNPC.newAI[2], calamityGlobalNPC.newAI[3]);
                        int teleportDust = Dust.NewDust(position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                        Main.dust[teleportDust].noGravity = true;
                    }
                }

                // Teleport
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f && NPC.ai[1] != 1f && calamityGlobalNPC.newAI[2] != 0f && calamityGlobalNPC.newAI[3] != 0f)
                {
                    // Teleport dust
                    for (int m = 0; m < 30; m++)
                    {
                        int teleportDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                        Main.dust[teleportDust].noGravity = true;
                        Main.dust[teleportDust].velocity.X *= 2f;
                    }

                    // New location
                    NPC.Center = new Vector2(calamityGlobalNPC.newAI[2], calamityGlobalNPC.newAI[3]);
                    NPC.velocity = Vector2.Zero;

                    NPC.ai[3] = -60f;
                    calamityGlobalNPC.newAI[2] = calamityGlobalNPC.newAI[3] = 0f;
                    NPC.SyncExtraAI();
                    NPC.netUpdate = true;
                }
            }

            // Skull shooting
            if ((handsDead) && NPC.ai[1] == 0f && !phase4)
            {
                float skullProjFrequency = phase2 ? (48f - (death ? 10f * (1f - lifeRatio) : 0f)) : 60f;
                if (Main.getGoodWorld)
                    skullProjFrequency *= 0.8f;
                skullProjFrequency = (float)Math.Ceiling(skullProjFrequency);

                if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[1] % skullProjFrequency == 0f && calamityGlobalNPC.newAI[1] > 45f)
                {
                    Vector2 skullFiringPos = NPC.Center;
                    float skullProjTargetX = Main.player[NPC.target].Center.X - skullFiringPos.X;
                    float skullProjTargetY = Main.player[NPC.target].Center.Y - skullFiringPos.Y;
                    if (Collision.CanHit(skullFiringPos, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float skullProjSpeed = phase2 ? (5f + (death ? 1f * (1f - lifeRatio) : 0f)) : 4f;
                        int spread2 = 50;
                        Vector2 skullProjDirection = new Vector2(skullProjTargetX + Main.rand.Next(-spread2, spread2 + 1) * 0.01f, skullProjTargetY + Main.rand.Next(-spread2, spread2 + 1) * 0.01f).SafeNormalize(Vector2.UnitY);
                        skullProjDirection *= skullProjSpeed;
                        skullProjDirection += NPC.velocity;
                        skullFiringPos += skullProjDirection * 5f;

                        int type = ProjectileID.Skull;

                        int skullProjectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), skullFiringPos, skullProjDirection, type, SkullDamage, 0f, Main.myPlayer, -1f);
                        Main.projectile[skullProjectile].timeLeft = 600;
                        if (death && handsDead)
                        {
                            skullProjDirection = new Vector2(skullProjTargetX, skullProjTargetY).SafeNormalize(Vector2.UnitY);
                            skullProjDirection *= skullProjSpeed * 2f;
                            int skullProjectile2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), skullFiringPos, skullProjDirection, type, SkullDamage, 0f, Main.myPlayer, -2f);
                            Main.projectile[skullProjectile2].timeLeft = 600;
                        }

                        NPC.netUpdate = true;
                    }
                }
            }

            // Float above target
            if (NPC.ai[1] == 0f)
            {
                calamityGlobalNPC.newAI[1] += 1f;
                float chargePhaseChangeRateBoost = phase5 ? (death ? 24f : 8f) : phase4 ? (death ? 6f : 4f) : ((death ? 4.5f : 3f) * ((1f - lifeRatio) / (1f - phase4LifeRatio)));
                if (!handsDead)
                    chargePhaseChangeRateBoost *= 0.25f;

                float chargePhaseChangeRate = chargePhaseChangeRateBoost + 1f;
                NPC.ai[2] += chargePhaseChangeRate;
                NPC.localAI[1] += chargePhaseChangeRate;
                float chargePhaseGateValue = ChargeGateValue;
                if (NPC.localAI[1] > chargePhaseGateValue)
                    NPC.localAI[1] = chargePhaseGateValue;

                float forcedMoveAwayTime = death ? 30f : 45f;
                float canChargeDistance = 320f; // 20 tile distance
                bool hasMovedForcedDistance = NPC.localAI[2] >= forcedMoveAwayTime;
                bool canCharge = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) >= canChargeDistance;
                bool charge = NPC.ai[2] >= chargePhaseGateValue && canCharge;
                bool forceCharge = NPC.ai[2] > chargePhaseGateValue + 120f;
                if (charge || forceCharge)
                {
                    NPC.localAI[2] += 1f;
                    if ((hasMovedForcedDistance || !phase3) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] = 1f;
                        NPC.localAI[0] = 1f;
                        NPC.localAI[1] = chargePhaseGateValue;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;

                        NPC.SyncExtraAI();
                        NPC.SyncVanillaLocalAI();
                        NPC.netUpdate = true;
                    }
                }

                float headYAcceleration = (Main.getGoodWorld ? 0.07f : death ? (0.06f + 0.04f * (1f - lifeRatio)) : 0.04f);
                float headYTopSpeed = headYAcceleration * 100f;
                float headXAcceleration = (Main.getGoodWorld ? 0.21f : death ? (0.16f + 0.08f * (1f - lifeRatio)) : 0.08f);
                float headXTopSpeed = headXAcceleration * 100f;
                float deceleration = Main.getGoodWorld ? 0.83f : death ? 0.86f : 0.89f;

                float moveAwayGateValue = chargePhaseGateValue - (5f + chargePhaseChangeRate);
                bool moveAwayBeforeCharge = NPC.ai[2] >= moveAwayGateValue;
                if (moveAwayBeforeCharge)
                {
                    if (!canCharge || !hasMovedForcedDistance)
                    {
                        float phase5Multiplier = 1.2f;
                        float maxVelocity = (NPC.ai[2] - moveAwayGateValue) * (moveAwayVelocity * 0.002f);
                        if (phase5)
                            maxVelocity *= phase5Multiplier;

                        float maxVelocityCap = moveAwayVelocity;
                        if (phase5)
                            maxVelocityCap *= phase5Multiplier;
                        if (maxVelocity > maxVelocityCap)
                            maxVelocity = maxVelocityCap;

                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * -maxVelocity;
                    }

                    // New charge attack
                    if (phase3)
                    {
                        NPC.rotation += NPC.direction * 0.3f;

                        if (NPC.localAI[0] == 0f)
                        {
                            NPC.localAI[0] = 1f;
                            NPC.SyncVanillaLocalAI();

                            SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                        }
                    }
                    else
                        NPC.rotation = NPC.velocity.X / 15f;

                    return false;
                }

                NPC.rotation = NPC.velocity.X / 15f;

                if (NPC.Top.Y > Main.player[NPC.target].Top.Y - 250f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.98f;
                    NPC.velocity.Y -= headYAcceleration;
                    if (NPC.velocity.Y > headYTopSpeed)
                        NPC.velocity.Y = headYTopSpeed;
                }
                else if (NPC.Top.Y < Main.player[NPC.target].Top.Y - 250f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.98f;
                    NPC.velocity.Y += headYAcceleration;
                    if (NPC.velocity.Y < -headYTopSpeed)
                        NPC.velocity.Y = -headYTopSpeed;
                }

                if (NPC.Center.X > Main.player[NPC.target].Center.X)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;
                    NPC.velocity.X -= headXAcceleration;
                    if (NPC.velocity.X > headXTopSpeed)
                        NPC.velocity.X = headXTopSpeed;
                }

                if (NPC.Center.X < Main.player[NPC.target].Center.X)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;
                    NPC.velocity.X += headXAcceleration;
                    if (NPC.velocity.X < -headXTopSpeed)
                        NPC.velocity.X = -headXTopSpeed;
                }
            }

            // Spin charge
            else if (NPC.ai[1] == 1f)
            {
                if (Main.getGoodWorld)
                {
                    NPC.reflectsProjectiles = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == 0f)
                    {
                        if (NPC.CountNPCS(NPCID.DarkCaster) < 6)
                        {
                            SpawnCasterNearHead(NPCID.DarkCaster);
                        }

                        if (Main.zenithWorld)
                        {
                            if (!NPC.AnyNPCs(NPCID.DiabolistWhite))
                            {
                                SpawnCasterNearHead(NPCID.DiabolistWhite);
                            }
                        }
                    }
                }

                NPC.defense = NPC.defDefense - 10;
                NPC.damage = (int)Math.Round(NPC.defDamage * SpinDamageMult);

                float phaseChangeRateBoost = phase3 ? 0f : 1f - (lifeRatio - phase3LifeRatio) / (1f - phase3LifeRatio);
                NPC.ai[2] += 1f + phaseChangeRateBoost;

                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] == 2f)
                    SoundEngine.PlaySound(phase3 ? SoundID.ForceRoarPitched : SoundID.ForceRoar, NPC.Center);

                // Reset telegraph timer to create color fade
                if (NPC.localAI[1] > 0f)
                {
                    NPC.localAI[1] -= 2f;
                    if (NPC.localAI[1] <= 0f)
                    {
                        NPC.localAI[1] = 0f;
                        NPC.SyncVanillaLocalAI();
                    }
                }

                bool dontGoMach10 = false;
                float dashPhaseTime = death ? 210f : 300f;
                //Netmode check fixes issues where attack ends at different times for server vs clients
                if (NPC.ai[2] >= dashPhaseTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.getGoodWorld)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.DarkCaster) < 6)
                        {
                            SpawnCasterNearHead(NPCID.DarkCaster);
                        }
                    }

                    NPC.ai[2] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    NPC.SyncVanillaLocalAI();
                    NPC.SyncExtraAI();
                    NPC.netUpdate = true;

                    dontGoMach10 = true;
                }

                NPC.rotation += NPC.direction * 0.3f;

                Vector2 headSpinPos = NPC.Center;
                float headSpinTargetX = Main.player[NPC.target].Center.X - headSpinPos.X;
                float headSpinTargetY = Main.player[NPC.target].Center.Y - headSpinPos.Y;
                float headSpinTargetDist = (float)Math.Sqrt(headSpinTargetX * headSpinTargetX + headSpinTargetY * headSpinTargetY);

                // Increase speed while charging
                if (!phase3)
                {
                    float velocityBoost = MathHelper.Lerp(0f, 3f, (1f - lifeRatio) / (1f - phase3LifeRatio));
                    if (handsDead)
                        headSpinVelocityMult += velocityBoost;
                }

                float altDashStopDistance = death ? 320f : 400f;
                float headSpeedIncreaseDist = phase3 ? altDashStopDistance : 160f;
                if (headSpinTargetDist > headSpeedIncreaseDist)
                {
                    float velocityMult = phase3 ? 0.00075f : 0.0015f;
                    float baseDistanceVelocityMult = 1f + MathHelper.Clamp((headSpinTargetDist - headSpeedIncreaseDist) * 0.0015f, 0.05f, death ? 2f : 1.5f);
                    headSpinVelocityMult *= baseDistanceVelocityMult;
                }

                if (Main.getGoodWorld)
                    headSpinVelocityMult *= 1.3f;

                headSpinTargetDist = headSpinVelocityMult / headSpinTargetDist;
                Vector2 headSpinVelocity = new Vector2(headSpinTargetX, headSpinTargetY) * headSpinTargetDist;

                if (!dontGoMach10)
                {
                    if (phase3)
                    {
                        // Dash directly towards the target until within 15 tiles of the target, and then continue in the same direction
                        float altDashPhaseTime = dashPhaseTime * (death ? 0.9f : 0.85f);
                        if (NPC.ai[2] < altDashPhaseTime)
                        {
                            if (NPC.Center.Distance(Main.player[NPC.target].Center) > altDashStopDistance || NPC.ai[2] == 1f + phaseChangeRateBoost)
                                NPC.velocity = headSpinVelocity.SafeNormalize(Vector2.UnitY) * headSpinVelocityMult + NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 2f;
                            else
                                NPC.ai[2] = altDashPhaseTime;
                        }
                    }
                    else
                        NPC.velocity = headSpinVelocity;
                }
            }

            // Daytime enrage
            else if (NPC.ai[1] == 2f)
            {
                NPC.damage = 1000;
                calamityGlobalNPC.DR = 0.9999f;
                calamityGlobalNPC.unbreakableDR = true;

                calamityGlobalNPC.CurrentlyEnraged = true;
                calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                NPC.rotation += NPC.direction * 0.3f;
                Vector2 enrageSpinPos = NPC.Center;
                float enrageSpinTargetX = Main.player[NPC.target].Center.X - enrageSpinPos.X;
                float enrageSpinTargetY = Main.player[NPC.target].Center.Y - enrageSpinPos.Y;
                float enrageSpinTargetDist = (float)Math.Sqrt(enrageSpinTargetX * enrageSpinTargetX + enrageSpinTargetY * enrageSpinTargetY);
                enrageSpinTargetDist = 8f / enrageSpinTargetDist;
                NPC.velocity.X = enrageSpinTargetX * enrageSpinTargetDist;
                NPC.velocity.Y = enrageSpinTargetY * enrageSpinTargetDist;
            }

            // Despawn
            else if (NPC.ai[1] == 3f)
            {
                // Disable teleports
                if (NPC.ai[3] != 0f || calamityGlobalNPC.newAI[2] != 0f || calamityGlobalNPC.newAI[3] != 0f)
                {
                    NPC.ai[3] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    NPC.SyncExtraAI();
                    NPC.netUpdate = true;
                }

                NPC.velocity.Y += 0.1f;
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= 0.95f;
                NPC.velocity.X *= 0.95f;
                if (NPC.timeLeft > 50)
                    NPC.timeLeft = 50;
            }

            // Emit dust
            if (NPC.ai[1] != 2f && NPC.ai[1] != 3f && numHandsAlive != 0)
            {
                int idleDust = Dust.NewDust(new Vector2(NPC.Center.X - 15f - NPC.velocity.X * 5f, NPC.position.Y + NPC.height - 2f), 30, 10, DustID.Blood, -NPC.velocity.X * 0.2f, 3f, 0, default, 2f);
                Main.dust[idleDust].noGravity = true;
                Main.dust[idleDust].velocity.X = Main.dust[idleDust].velocity.X * 1.3f;
                Main.dust[idleDust].velocity.X = Main.dust[idleDust].velocity.X + NPC.velocity.X * 0.4f;
                Main.dust[idleDust].velocity.Y = Main.dust[idleDust].velocity.Y + (2f + NPC.velocity.Y);
                for (int j = 0; j < 2; j++)
                {
                    idleDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 120f), NPC.width, 60, DustID.Blood, NPC.velocity.X, NPC.velocity.Y, 0, default, 2f);
                    Main.dust[idleDust].noGravity = true;
                    Main.dust[idleDust].velocity -= NPC.velocity;
                    Main.dust[idleDust].velocity.Y = Main.dust[idleDust].velocity.Y + 5f;
                }
            }

            return false;
        }

        public override void PostDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Telegraph for charges
            float beginTelegraphGateValue = ChargeGateValue - ChargeTelegraphTime;
            if (NPC.localAI[1] > beginTelegraphGateValue)
            {
                float colorScale = MathHelper.Clamp((NPC.localAI[1] - beginTelegraphGateValue) / ChargeTelegraphTime, 0f, 1f);
                Color drawColor2 = new Color(150, 150, 150, 0) * colorScale;
                var halfSize = NPC.frame.Size() / 2;
                SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame,
                        drawColor2, NPC.rotation, halfSize, NPC.scale, spriteEffects, 0f);
                }
            }
        }

        public class SkeletronHandAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                float yMultiplier = 1f;
                if (calamityGlobalNPC.newAI[0] != 0f)
                    yMultiplier = calamityGlobalNPC.newAI[0];
                if (death)
                    yMultiplier *= 1.3f;

                // Inflict 0 damage for 3 seconds after spawning
                if (calamityGlobalNPC.newAI[1] < 180f)
                {
                    calamityGlobalNPC.newAI[1] += 1f;
                    if (calamityGlobalNPC.newAI[1] % 15f == 0f)
                        NPC.SyncExtraAI();

                    NPC.damage = 0;
                }
                else
                    NPC.damage = NPC.defDamage;

                NPC.spriteDirection = -(int)NPC.ai[0];

                if (Main.npc[(int)NPC.ai[1]].ai[3] == -60f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Teleport dust
                        for (int m = 0; m < 10; m++)
                        {
                            int teleportDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemDiamond, 0f, 0f, 200, default, 3f);
                            Main.dust[teleportDust].noGravity = true;
                            Main.dust[teleportDust].velocity.X *= 2f;
                        }

                        // New location
                        NPC.Center = Main.npc[(int)NPC.ai[1]].Center;
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;
                    }
                }

                float skeletronLifeRatio = 1f;
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPCAIStyleID.SkeletronHead)
                {
                    NPC.ai[2] += 10f;
                    if (NPC.ai[2] > 50f || !Main.dedServ)
                    {
                        NPC.life = -1;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }
                }
                else
                    skeletronLifeRatio = Main.npc[(int)NPC.ai[1]].life / (float)Main.npc[(int)NPC.ai[1]].lifeMax;

                // This bool exists for fairness so the hands don't slap when Skeletron is in phase 3 and getting ready to do the new charge
                bool cancelSlap = Main.npc[(int)NPC.ai[1]].ai[2] >= ChargeGateValue;

                // Fire skulls from hands at the end of each slap phase (master mode only)
                bool phase2 = skeletronLifeRatio < 0.5f;

                // Attack far more often if still alive
                bool phase3 = skeletronLifeRatio < 0.3f;

                float velocityMultiplier = MathHelper.Lerp(death ? 0.6f : 0.7f, 1f, skeletronLifeRatio);
                float velocityIncrement = MathHelper.Lerp(0.2f, death ? 0.4f : 0.3f, 1f - skeletronLifeRatio);
                float handSwipeVelocity = MathHelper.Lerp(16f, death ? 24f : 20f, 1f - skeletronLifeRatio);
                float deceleration = Main.getGoodWorld ? 0.78f : death ? 0.82f : 0.86f;

                if (death)
                {
                    velocityMultiplier *= 0.75f;
                    velocityIncrement *= 1.5f;
                    handSwipeVelocity *= 1.35f;
                    deceleration *= 0.75f;
                }

                float handSwipeDistance = death ? HandSwipeDistance_Master : HandSwipeDistance;
                float handSwipeDuration = handSwipeDistance / handSwipeVelocity;
                float slapGateValue = HandSlapGateValue;

                float slapTimerIncrement = MathHelper.Lerp(death ? 1.5f : 1f, death ? 3f : 2f, 1f - skeletronLifeRatio);
                if (phase3)
                    slapTimerIncrement *= (death ? 2.5f : 2f);
                else if (phase2)
                    slapTimerIncrement *= (death ? 2f : 1.5f);

                if (NPC.ai[2] == 0f || NPC.ai[2] == 3f)
                {
                    if (Main.npc[(int)NPC.ai[1]].ai[1] == 3f && NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    if (Main.npc[(int)NPC.ai[1]].ai[1] != 0f || cancelSlap)
                    {
                        deceleration *= 0.75f;
                        velocityIncrement *= 1.5f;

                        float maxX = velocityIncrement * 100f * velocityMultiplier;
                        float maxY = velocityIncrement * 100f * velocityMultiplier;

                        if (NPC.Top.Y > Main.npc[(int)NPC.ai[1]].Top.Y - 100f * yMultiplier)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y *= deceleration;
                            NPC.velocity.Y -= velocityIncrement;
                            if (NPC.velocity.Y > maxY)
                                NPC.velocity.Y = maxY;
                        }
                        else if (NPC.Top.Y < Main.npc[(int)NPC.ai[1]].Top.Y - 100f * yMultiplier)
                        {
                            if (NPC.velocity.Y < 0f)
                                NPC.velocity.Y *= deceleration;
                            NPC.velocity.Y += velocityIncrement;
                            if (NPC.velocity.Y < -maxY)
                                NPC.velocity.Y = -maxY;
                        }

                        if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X - 120f * NPC.ai[0])
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X *= deceleration;
                            NPC.velocity.X -= velocityIncrement;
                            if (NPC.velocity.X > maxX)
                                NPC.velocity.X = maxX;
                        }

                        if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 120f * NPC.ai[0])
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X *= deceleration;
                            NPC.velocity.X += velocityIncrement;
                            if (NPC.velocity.X < -maxX)
                                NPC.velocity.X = -maxX;
                        }
                    }
                    else
                    {
                        if (calamityGlobalNPC.newAI[3] == 1f)
                        {
                            calamityGlobalNPC.newAI[2] += slapTimerIncrement;
                            NPC.ai[3] += slapTimerIncrement;
                            if (NPC.ai[3] >= slapGateValue)
                            {
                                NPC.target = Main.npc[(int)NPC.ai[1]].target;
                                NPC.ai[2] += 1f;
                                NPC.ai[3] = calamityGlobalNPC.newAI[2] = slapGateValue;
                                calamityGlobalNPC.newAI[3] = 0f;
                                NPC.netUpdate = true;
                                NPC.SyncExtraAI();
                            }
                        }
                        else
                        {
                            calamityGlobalNPC.newAI[2] -= slapTimerIncrement * 2f;
                            if (calamityGlobalNPC.newAI[2] <= 0f)
                            {
                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 1f;
                                NPC.SyncExtraAI();
                            }
                        }

                        float maxX = velocityIncrement * 100f * velocityMultiplier;
                        float maxY = velocityIncrement * 100f * velocityMultiplier;

                        if (NPC.Top.Y > Main.npc[(int)NPC.ai[1]].Top.Y + 230f * yMultiplier)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y *= deceleration;
                            NPC.velocity.Y -= velocityIncrement;
                            if (NPC.velocity.Y > maxY)
                                NPC.velocity.Y = maxY;
                        }
                        else if (NPC.Top.Y < Main.npc[(int)NPC.ai[1]].Top.Y + 230f * yMultiplier)
                        {
                            if (NPC.velocity.Y < 0f)
                                NPC.velocity.Y *= deceleration;
                            NPC.velocity.Y += velocityIncrement;
                            if (NPC.velocity.Y < -maxY)
                                NPC.velocity.Y = -maxY;
                        }

                        if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0])
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X *= deceleration;
                            NPC.velocity.X -= velocityIncrement;
                            if (NPC.velocity.X > maxX)
                                NPC.velocity.X = maxX;
                        }

                        if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0])
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X *= deceleration;
                            NPC.velocity.X += velocityIncrement;
                            if (NPC.velocity.X < -maxX)
                                NPC.velocity.X = -maxX;
                        }
                    }

                    Vector2 handCurrentPos = NPC.Center;
                    float handIdleXPos = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - handCurrentPos.X;
                    float handIdleYPos = Main.npc[(int)NPC.ai[1]].Top.Y + 230f - handCurrentPos.Y;
                    float handIdleDist = (float)Math.Sqrt(handIdleXPos * handIdleXPos + handIdleYPos * handIdleYPos);
                    NPC.rotation = (float)Math.Atan2(handIdleYPos, handIdleXPos) + MathHelper.PiOver2;

                    return false;
                }

                if (NPC.ai[2] == 1f)
                {
                    Vector2 handCurrentPosition = NPC.Center;
                    float handDrawbackXPos = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - handCurrentPosition.X;
                    float handDrawbackYPos = Main.npc[(int)NPC.ai[1]].Top.Y + 230f - handCurrentPosition.Y;
                    float handDrawbackDist = (float)Math.Sqrt(handDrawbackXPos * handDrawbackXPos + handDrawbackYPos * handDrawbackYPos);
                    NPC.rotation = (float)Math.Atan2(handDrawbackYPos, handDrawbackXPos) + MathHelper.PiOver2;
                    NPC.velocity.X *= 0.95f;
                    NPC.velocity.Y -= velocityIncrement;

                    if (NPC.velocity.Y < -14f)
                        NPC.velocity.Y = -14f;
                    else if (NPC.velocity.Y > 10f)
                        NPC.velocity.Y = 10f;

                    if (NPC.Top.Y < Main.npc[(int)NPC.ai[1]].Top.Y - 200f)
                    {
                        NPC.ai[2] = 2f;
                        NPC.ai[3] = 0f;
                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * handSwipeVelocity;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 2f)
                {
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= handSwipeDuration || Vector2.Distance(Main.npc[(int)NPC.ai[1]].Center, NPC.Center) > handSwipeDistance || cancelSlap)
                    {
                        NPC.ai[2] = 3f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;

                        // Spawn projectiles
                        FireHandSwipeSkull(NPC, death, cancelSlap, phase2, phase3, handSwipeVelocity);
                    }
                }
                else if (NPC.ai[2] == 4f)
                {
                    Vector2 handStrikeCurrentPos = NPC.Center;
                    float handStrikeXPos = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - handStrikeCurrentPos.X;
                    float handStrikeYPos = Main.npc[(int)NPC.ai[1]].Top.Y + 230f - handStrikeCurrentPos.Y;
                    float handStrikeDist = (float)Math.Sqrt(handStrikeXPos * handStrikeXPos + handStrikeYPos * handStrikeYPos);
                    NPC.rotation = (float)Math.Atan2(handStrikeYPos, handStrikeXPos) + MathHelper.PiOver2;
                    NPC.velocity.Y *= 0.95f;
                    NPC.velocity.X += velocityIncrement * -NPC.ai[0];

                    if (NPC.velocity.X < -10f)
                        NPC.velocity.X = -10f;
                    else if (NPC.velocity.X > 14f)
                        NPC.velocity.X = 14f;

                    if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 500f || NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 500f)
                    {
                        NPC.ai[2] = 5f;
                        NPC.ai[3] = 0f;
                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * handSwipeVelocity;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 5f)
                {
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= handSwipeDuration || Vector2.Distance(Main.npc[(int)NPC.ai[1]].Center, NPC.Center) > handSwipeDistance || cancelSlap)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;

                        // Spawn projectiles
                        FireHandSwipeSkull(NPC, death, cancelSlap, phase2, phase3, handSwipeVelocity);
                    }
                }

                return false;
            }

            public override void PostDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                var calNPC = NPC.GetGlobalNPC<CalamityGlobalNPC>();
                float beginTelegraphGateValue = HandSlapGateValue - HandSlapTelegraphTime;
                if (calNPC.newAI[2] > beginTelegraphGateValue)
                {
                    float colorScale = MathHelper.Clamp((calNPC.newAI[2] - beginTelegraphGateValue) / HandSlapTelegraphTime, 0f, 1f);
                    Color drawColor2 = new Color(150, 150, 150, 0) * colorScale;
                    var halfSize = NPC.frame.Size() / 2;
                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (NPC.spriteDirection == 1)
                        spriteEffects = SpriteEffects.FlipHorizontally;

                    Vector2 glowOffset = Vector2.UnitY * 8f;
                    for (int i = 0; i < 2; i++)
                    {
                        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) - glowOffset, NPC.frame,
                            drawColor2, NPC.rotation, halfSize, NPC.scale, spriteEffects, 0f);
                    }
                }
            }
        }

        public class DungeonGuardianAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                return true;
            }

            public override void PostAI(Mod mod)
            {
                Player target = Main.player[NPC.target];
                if (NPC.ai[1] != 3f)
                {
                    Vector2 targetVector = target.Center - NPC.Center;
                    float targetDist = targetVector.Length();
                    targetDist = 12f / targetDist;
                    NPC.velocity.X = targetVector.X * targetDist;
                    NPC.velocity.Y = targetVector.Y * targetDist;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (NPC.localAI[1]++ % 60f == 59f)
                        {
                            Vector2 source = NPC.Center;
                            if (Collision.CanHit(source, 1, 1, target.Center, target.width, target.height))
                            {
                                float speed = 5f;
                                float xDist = target.Center.X - source.X + Main.rand.Next(-20, 21);
                                float yDist = target.Center.Y - source.Y + Main.rand.Next(-20, 21);
                                Vector2 velocity = new Vector2(xDist, yDist);
                                float distTarget = velocity.Length();
                                distTarget = speed / distTarget;
                                velocity.X *= distTarget;
                                velocity.Y *= distTarget;
                                Vector2 offset = new Vector2(velocity.X * 1f + Main.rand.Next(-50, 51) * 0.01f, velocity.Y * 1f + Main.rand.Next(-50, 51) * 0.01f).SafeNormalize(Vector2.UnitY);
                                offset *= speed;
                                offset += NPC.velocity;
                                velocity.X = offset.X;
                                velocity.Y = offset.Y;
                                int damage = 2500;
                                int projType = ProjectileID.Skull;
                                source += offset * 5f;
                                int skull = Projectile.NewProjectile(NPC.GetSource_FromAI(), source, velocity, projType, damage, 0f, Main.myPlayer, -1f);
                                Main.projectile[skull].timeLeft = 600;
                                Main.projectile[skull].tileCollide = false;
                            }
                        }
                    }
                }
            }
        }

    }
}
