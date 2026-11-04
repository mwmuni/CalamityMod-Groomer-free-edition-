using System;
using CalamityMod.Events;
using CalamityMod.ExtraTextures;
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
    public class WallOfFleshAI : VanillaAIOverride
    {
        public const float LaserShootGateValue = 400f;
        public const float LaserShootTelegraphTime = LaserShootGateValue * 0.5f;
        public const float TotalLasersPerBarrage = 3f;
        public const float EnragedLaserFiringDuration = 300f;

        // Rev+ exclusive
        public static int LaserDamage = 15; // 60 (modified to be always at maximum Expert damage and does not scale)
        public static int SickleDamage = 22; // 88

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Despawn
            if (NPC.position.X < 160f || NPC.position.X > ((Main.maxTilesX - 10) * 16))
                NPC.active = false;

            // Set Wall of Flesh variables
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                Main.wofDrawAreaBottom = -1;
                Main.wofDrawAreaTop = -1;
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Clamp life ratio to prevent bad velocity math.
            lifeRatio = MathHelper.Clamp(lifeRatio, 0f, 1f);

            // Phases based on HP
            bool phase2 = lifeRatio < 0.66f;
            bool phase3 = lifeRatio < 0.33f;

            if (Main.getGoodWorld && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(180))
            {
                if (NPC.CountNPCS(NPCID.FireImp) < 4)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        int targetTileX = (int)(NPC.Center.X / 16f);
                        int targetTileY = (int)(NPC.Center.Y / 16f);
                        if (NPC.target >= 0)
                        {
                            targetTileX = (int)(Main.player[NPC.target].Center.X / 16f);
                            targetTileY = (int)(Main.player[NPC.target].Center.Y / 16f);
                        }

                        targetTileX += Main.rand.Next(-50, 51);
                        for (targetTileY += Main.rand.Next(-50, 51); targetTileY < Main.maxTilesY - 10 && !WorldGen.SolidTile(targetTileX, targetTileY); targetTileY++)
                        {
                        }

                        targetTileY--;
                        if (!WorldGen.SolidTile(targetTileX, targetTileY))
                        {
                            int impSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), targetTileX * 16 + 8, targetTileY * 16, NPCID.FireImp);
                            if (Main.dedServ && impSpawn < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, impSpawn);

                            break;
                        }
                    }
                }
            }

            // Start leech spawning based on HP
            NPC.ai[1] += 1f;
            if (NPC.ai[2] == 0f)
            {
                if (death)
                    NPC.ai[1] += 1f;

                if (phase2)
                    NPC.ai[1] += 1f;
                if (phase3)
                    NPC.ai[1] += 1f;
                if (Main.getGoodWorld)
                    NPC.ai[1] += 9f;

                if (NPC.ai[1] > 2700f)
                    NPC.ai[2] = 1f;
            }

            // Leech spawn
            if (NPC.ai[2] > 0f && NPC.ai[1] > 60f)
            {
                int leechAmt = phase3 ? 3 : 2;

                NPC.ai[2] += 1f;
                NPC.ai[1] = 0f;
                if (NPC.ai[2] > leechAmt)
                    NPC.ai[2] = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.CountNPCS(NPCID.LeechHead) < 10)
                    {
                        int leechSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y + 20f), NPCID.LeechHead, 1);
                        int leechVelocity = death ? 12 : 9;
                        Main.npc[leechSpawn].velocity.X = NPC.direction * leechVelocity;
                    }

                    if (phase2 || death)
                    {
                        // Get target vector
                        Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * NPC.velocity.Length();
                        Vector2 projectileSpawn = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 50f;

                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, ProjectileID.DemonSickle, SickleDamage, 0f, Main.myPlayer, 0f, projectileVelocity.Length() * 3f);
                        Main.projectile[proj].timeLeft = 600;
                        Main.projectile[proj].tileCollide = false;
                    }
                }
            }

            // Play sound
            NPC.localAI[3] += 1f;
            if (NPC.localAI[3] >= (600 + Main.rand.Next(1000)))
            {
                NPC.localAI[3] = -Main.rand.Next(200);
                SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
            }

            // Set whoAmI variable
            Main.wofNPCIndex = NPC.whoAmI;

            // Set eye positions
            int currentEyeTileCenterX = (int)(NPC.position.X / 16f);
            int currentEyeTileWidthX = (int)((NPC.position.X + NPC.width) / 16f);
            int currentEyeTileHeightY = (int)(NPC.Center.Y / 16f);
            int eyeMovementTries = 0;
            int eyeMovementTileY = currentEyeTileHeightY + 7;
            while (eyeMovementTries < 15 && eyeMovementTileY > Main.UnderworldLayer)
            {
                eyeMovementTileY++;
                for (int eyeMovementTileX = currentEyeTileCenterX; eyeMovementTileX <= currentEyeTileWidthX; eyeMovementTileX++)
                {
                    try
                    {
                        if (WorldGen.SolidTile(eyeMovementTileX, eyeMovementTileY) || Main.tile[eyeMovementTileX, eyeMovementTileY].LiquidAmount > 0)
                            eyeMovementTries++;
                    }
                    catch
                    { eyeMovementTries += 15; }
                }
            }
            eyeMovementTileY += 4;
            if (Main.wofDrawAreaBottom == -1)
                Main.wofDrawAreaBottom = eyeMovementTileY * 16;
            else if (Main.wofDrawAreaBottom > eyeMovementTileY * 16)
            {
                Main.wofDrawAreaBottom--;
                if (Main.wofDrawAreaBottom < eyeMovementTileY * 16)
                    Main.wofDrawAreaBottom = eyeMovementTileY * 16;
            }
            else if (Main.wofDrawAreaBottom < eyeMovementTileY * 16)
            {
                Main.wofDrawAreaBottom++;
                if (Main.wofDrawAreaBottom > eyeMovementTileY * 16)
                    Main.wofDrawAreaBottom = eyeMovementTileY * 16;
            }

            eyeMovementTries = 0;
            eyeMovementTileY = currentEyeTileHeightY - 7;
            while (eyeMovementTries < 15 && eyeMovementTileY < Main.maxTilesY - 10)
            {
                eyeMovementTileY--;
                for (int i = currentEyeTileCenterX; i <= currentEyeTileWidthX; i++)
                {
                    try
                    {
                        if (WorldGen.SolidTile(i, eyeMovementTileY) || Main.tile[i, eyeMovementTileY].LiquidAmount > 0)
                            eyeMovementTries++;
                    }
                    catch
                    { eyeMovementTries += 15; }
                }
            }
            eyeMovementTileY -= 4;
            if (Main.wofDrawAreaTop == -1)
                Main.wofDrawAreaTop = eyeMovementTileY * 16;
            else if (Main.wofDrawAreaTop > eyeMovementTileY * 16)
            {
                Main.wofDrawAreaTop--;
                if (Main.wofDrawAreaTop < eyeMovementTileY * 16)
                    Main.wofDrawAreaTop = eyeMovementTileY * 16;
            }
            else if (Main.wofDrawAreaTop < eyeMovementTileY * 16)
            {
                Main.wofDrawAreaTop++;
                if (Main.wofDrawAreaTop > eyeMovementTileY * 16)
                    Main.wofDrawAreaTop = eyeMovementTileY * 16;
            }

            // Set Y position
            float mouthYPosition = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2 - NPC.height / 2;
            int worldBottomTileY = (Main.maxTilesY - 180) * 16;
            if (mouthYPosition < worldBottomTileY)
                mouthYPosition = worldBottomTileY;
            NPC.position.Y = mouthYPosition;

            float targetPosition = Main.player[NPC.target].Center.X;
            float npcPosition = NPC.Center.X;

            // Speed up if target is too far, slow down if too close
            float distanceFromTarget;
            if (NPC.velocity.X < 0f)
                distanceFromTarget = npcPosition - targetPosition;
            else
                distanceFromTarget = targetPosition - npcPosition;

            float halfAverageScreenWidth = 960f;
            float distanceBeforeSlowingDown = 640f;
            float timeBeforeEnrage = death ? (150f - 130f * (1f - lifeRatio)) : 600f;
            float speedMult = 1f;

            if (calamityGlobalNPC.newAI[0] < timeBeforeEnrage)
            {
                if (distanceFromTarget > halfAverageScreenWidth)
                {
                    speedMult += (distanceFromTarget - halfAverageScreenWidth) * 0.001f;
                    calamityGlobalNPC.newAI[0] += 1f;

                    // Enrage after 10 seconds of target being off screen
                    if (calamityGlobalNPC.newAI[0] >= timeBeforeEnrage)
                    {
                        calamityGlobalNPC.newAI[1] = 1f;

                        // Tell eyes to fire different lasers
                        NPC.ai[3] = 1f;

                        // Play roar sound on players nearby
                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < 2800f)
                            SoundEngine.PlaySound(SoundID.NPCDeath10 with { Pitch = SoundID.NPCDeath10.Pitch - 0.25f }, Main.LocalPlayer.Center);
                    }
                }
                else if (distanceFromTarget < distanceBeforeSlowingDown)
                    speedMult += (distanceFromTarget - distanceBeforeSlowingDown) * 0.002f;

                if (distanceFromTarget < halfAverageScreenWidth)
                {
                    if (calamityGlobalNPC.newAI[0] > 0f)
                        calamityGlobalNPC.newAI[0] -= 1f;
                }

                speedMult = MathHelper.Clamp(speedMult, 0.4f, 2f);
            }

            // Enrage if target is off screen for too long
            if (calamityGlobalNPC.newAI[1] == 1f)
            {
                // Triple speed
                speedMult = 3.25f;

                // Return to normal if very close to target
                if (distanceFromTarget < distanceBeforeSlowingDown)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.ai[3] = 0f;
                }
            }

            calamityGlobalNPC.CurrentlyEnraged = distanceFromTarget > halfAverageScreenWidth || NPC.ai[3] == 1f;

            float deathModeVelocityBoost = 0f;
            if (death)
            {
                float velocityBoostStartDistance = distanceBeforeSlowingDown;
                float velocityBoostMaxDistance = velocityBoostStartDistance * 1.5f;
                float distanceFromTargetX = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
                float lerpAmount = MathHelper.Clamp((distanceFromTargetX - velocityBoostStartDistance) / velocityBoostMaxDistance, 0f, 1f);
                deathModeVelocityBoost = MathHelper.Lerp(0f, 4f, lerpAmount);
            }

            // NOTE: Max velocity is 8 in Expert Mode
            // NOTE: Max velocity is 9 in For The Worthy

            float velocityBoost = 4f * (1f - lifeRatio);
            float velocityX = 2f + deathModeVelocityBoost + velocityBoost;
            velocityX *= speedMult;

            if (death)
                velocityX *= 1.1f;

            if (Main.getGoodWorld)
            {
                velocityX *= 1.1f;
                velocityX += 0.1f;
            }

            // NOTE: Values below are based on Rev Mode only!
            // Max velocity without enrage is 12
            // Min velocity is 1.5
            // Max velocity with enrage is 18

            // Set X velocity
            if (NPC.velocity.X == 0f)
            {
                NPC.TargetClosest();
                if (Main.player[NPC.target].dead)
                {
                    float wallVelocity = float.PositiveInfinity;
                    int wallDirection = 0;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[NPC.target];
                        if (player.active)
                        {
                            float playerDist = NPC.Distance(player.Center);
                            if (wallVelocity > playerDist)
                            {
                                wallVelocity = playerDist;
                                wallDirection = (NPC.Center.X < player.Center.X) ? 1 : -1;
                            }
                        }
                    }

                    NPC.direction = wallDirection;
                }

                NPC.velocity.X = NPC.direction;
            }

            if (NPC.velocity.X < 0f)
            {
                NPC.velocity.X = -velocityX;
                NPC.direction = -1;
            }
            else
            {
                NPC.velocity.X = velocityX;
                NPC.direction = 1;
            }

            if (Main.player[NPC.target].dead || !Main.player[NPC.target].gross)
                NPC.TargetClosest_WOF();

            if (Main.player[NPC.target].dead)
            {
                NPC.localAI[1] += 0.0055555557f;
                if (NPC.localAI[1] >= 1f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
                    NPC.life = 0;
                    NPC.active = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);

                    return false;
                }
            }
            else
                NPC.localAI[1] = MathHelper.Clamp(NPC.localAI[1] - 1f / 30f, 0f, 1f);

            // Direction
            NPC.spriteDirection = NPC.direction;
            Vector2 mouthLocation = NPC.Center;
            float mouthTargetX = Main.player[NPC.target].Center.X - mouthLocation.X;
            float mouthTargetY = Main.player[NPC.target].Center.Y - mouthLocation.Y;
            float mouthTargetDist = (float)Math.Sqrt(mouthTargetX * mouthTargetX + mouthTargetY * mouthTargetY);
            mouthTargetX *= mouthTargetDist;
            mouthTargetY *= mouthTargetDist;

            // Rotation based on direction
            if (NPC.direction > 0)
            {
                if (Main.player[NPC.target].Center.X > NPC.Center.X)
                    NPC.rotation = (float)Math.Atan2(-mouthTargetY, -mouthTargetX) + MathHelper.Pi;
                else
                    NPC.rotation = 0f;
            }
            else if (Main.player[NPC.target].Center.X < NPC.Center.X)
                NPC.rotation = (float)Math.Atan2(mouthTargetY, mouthTargetX) + MathHelper.Pi;
            else
                NPC.rotation = 0f;

            // Expert hungry respawn over time
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Range of 2 to 11
                float spawnBoost = death ? 1f : (float)Math.Ceiling(lifeRatio * 10f);
                int chance = (int)(1f + spawnBoost);

                // Range of 4 to 121
                chance *= chance;

                // Range of 23 to 134
                chance = (chance * 19 + 400) / 20;

                // Range of 32 to 59
                if (chance < 60)
                    chance = (chance * 3 + 60) / 4;

                // Range of 64 to 268
                chance *= 2;

                if (death)
                    chance /= 2;

                if (chance < 2)
                    chance = 2;

                if (Main.rand.NextBool(chance))
                {
                    int maxHungriesBasedOnHP = (int)Math.Round(MathHelper.Lerp(death ? 2f : 1f, death ? 6f : 4f, NPC.life / (float)NPC.lifeMax));
                    if (NPC.CountNPCS(NPCID.TheHungry) < maxHungriesBasedOnHP)
                    {
                        int hungryAmt = 0;
                        int maxHungries = 10;
                        Span<float> array = stackalloc float[maxHungries];
                        for (int j = 0; j < Main.maxNPCs; j++)
                        {
                            if (hungryAmt < maxHungries && Main.npc[j].active && Main.npc[j].type == NPCID.TheHungry)
                            {
                                array[hungryAmt] = Main.npc[j].ai[0];
                                hungryAmt++;
                            }
                        }

                        int maxValue = 1 + hungryAmt * 2;
                        if (death)
                            maxValue /= 2;

                        if (maxValue < 2)
                            maxValue = 2;

                        if (hungryAmt < maxHungries && Main.rand.Next(maxValue) <= 1)
                        {
                            int spawnHungryControl = -1;
                            for (int k = 0; k < 1000; k++)
                            {
                                int randomHungrySpawnValue = Main.rand.Next(maxHungries);
                                float hungryArrayValue = randomHungrySpawnValue * 0.1f - 0.05f;
                                bool shouldRespawnHungry = true;
                                for (int i = 0; i < hungryAmt; i++)
                                {
                                    if (hungryArrayValue == array[i])
                                    {
                                        shouldRespawnHungry = false;
                                        break;
                                    }
                                }
                                if (shouldRespawnHungry)
                                {
                                    spawnHungryControl = randomHungrySpawnValue;
                                    break;
                                }
                            }
                            if (spawnHungryControl >= 0)
                            {
                                int hungryRespawns = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)mouthYPosition, NPCID.TheHungry, NPC.whoAmI);
                                Main.npc[hungryRespawns].ai[0] = spawnHungryControl * 0.1f - 0.05f;
                            }
                        }
                    }
                }
            }

            // Spawn eyes and hungries
            if (NPC.localAI[0] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 2f;

                mouthYPosition = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2;
                mouthYPosition = (mouthYPosition + Main.wofDrawAreaTop) / 2f;
                int eyeSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)mouthYPosition, NPCID.WallofFleshEye, NPC.whoAmI);
                Main.npc[eyeSpawn].ai[0] = 1f;
                if (death)
                    Main.npc[eyeSpawn].ai[3] = 1f;

                mouthYPosition = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2;
                mouthYPosition = (mouthYPosition + Main.wofDrawAreaBottom) / 2f;
                eyeSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)mouthYPosition, NPCID.WallofFleshEye, NPC.whoAmI);
                Main.npc[eyeSpawn].ai[0] = -1f;
                if (death)
                    Main.npc[eyeSpawn].ai[3] = -1f;

                mouthYPosition = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2;
                mouthYPosition = (mouthYPosition + Main.wofDrawAreaBottom) / 2f;

                int maxHungries = death ? 14 : 11;
                float maxOffset = death ? (0.2f / 3f) : 0.1f;
                for (int j = 0; j < maxHungries; j++)
                {
                    int hungrySpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)mouthYPosition, NPCID.TheHungry, NPC.whoAmI);
                    Main.npc[hungrySpawn].ai[0] = j * maxOffset - 0.05f;
                }
            }

            return false;
        }

        public class HungryAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                if (NPC.justHit)
                    NPC.ai[1] = 10f;

                if (Main.wofNPCIndex < 0)
                {
                    NPC.active = false;
                    return false;
                }

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                CalamityUtils.CalamityTargeting(NPC, default);
                float acceleration = death ? 0.15f : 0.12f;
                float distanceFromWall = 300f;
                NPC.damage = NPC.defDamage;
                NPC.defense = NPC.defDefense;
                if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.5)
                {
                    NPC.damage = NPC.defDamage * 2;
                    NPC.defense = 30;
                    acceleration += death ? 0.1f : 0.08f;
                }
                else if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.75)
                {
                    NPC.damage = (int)Math.Round(NPC.defDamage * 1.5f);
                    NPC.defense = 20;
                    acceleration += death ? 0.05f : 0.04f;
                }

                if (NPC.whoAmI % 4 == 0)
                    distanceFromWall *= 1.75f;

                if (NPC.whoAmI % 4 == 1)
                    distanceFromWall *= 1.5f;

                if (NPC.whoAmI % 4 == 2)
                    distanceFromWall *= 1.25f;

                if (NPC.whoAmI % 3 == 0)
                    distanceFromWall *= 1.5f;

                if (NPC.whoAmI % 3 == 1)
                    distanceFromWall *= 1.25f;

                distanceFromWall *= 0.75f;

                float num404 = Main.npc[Main.wofNPCIndex].Center.X;
                float y3 = Main.npc[Main.wofNPCIndex].position.Y;
                float num405 = Main.wofDrawAreaBottom - Main.wofDrawAreaTop;
                y3 = (float)Main.wofDrawAreaTop + num405 * NPC.ai[0];
                NPC.ai[2] += 1f;
                if (NPC.ai[2] > 100f)
                {
                    distanceFromWall = (int)(distanceFromWall * 1.3f);
                    if (NPC.ai[2] > 200f)
                        NPC.ai[2] = 0f;
                }

                Vector2 vector40 = new Vector2(num404, y3);
                float num406 = Main.player[NPC.target].Center.X - (float)(NPC.width / 2) - vector40.X;
                float num407 = Main.player[NPC.target].Center.Y - (float)(NPC.height / 2) - vector40.Y;
                float num408 = (float)Math.Sqrt(num406 * num406 + num407 * num407);
                if (NPC.ai[1] == 0f)
                {
                    if (num408 > distanceFromWall)
                    {
                        num408 = distanceFromWall / num408;
                        num406 *= num408;
                        num407 *= num408;
                    }

                    if (NPC.position.X < num404 + num406)
                    {
                        NPC.velocity.X += acceleration;
                        if (NPC.velocity.X < 0f && num406 > 0f)
                            NPC.velocity.X += acceleration * 2.5f;
                    }
                    else if (NPC.position.X > num404 + num406)
                    {
                        NPC.velocity.X -= acceleration;
                        if (NPC.velocity.X > 0f && num406 < 0f)
                            NPC.velocity.X -= acceleration * 2.5f;
                    }

                    if (NPC.position.Y < y3 + num407)
                    {
                        NPC.velocity.Y += acceleration;
                        if (NPC.velocity.Y < 0f && num407 > 0f)
                            NPC.velocity.Y += acceleration * 2.5f;
                    }
                    else if (NPC.position.Y > y3 + num407)
                    {
                        NPC.velocity.Y -= acceleration;
                        if (NPC.velocity.Y > 0f && num407 < 0f)
                            NPC.velocity.Y -= acceleration * 2.5f;
                    }

                    float maxVelocity = 4f;
                    if (Main.wofNPCIndex >= 0)
                    {
                        float velocityBoost = 1.5f;
                        float wallLifeRatio = Main.npc[Main.wofNPCIndex].life / (float)Main.npc[Main.wofNPCIndex].lifeMax;
                        if (wallLifeRatio < 0.75f)
                            velocityBoost += 0.7f;

                        if (wallLifeRatio < 0.5f)
                            velocityBoost += 0.7f;

                        if (wallLifeRatio < 0.25f)
                            velocityBoost += 0.9f;

                        if (wallLifeRatio < 0.1f)
                            velocityBoost += 0.9f;

                        velocityBoost *= death ? 1.4f : 1.25f;
                        velocityBoost += 0.3f;
                        maxVelocity += velocityBoost * 0.35f;
                        if (NPC.Center.X < Main.npc[Main.wofNPCIndex].Center.X && Main.npc[Main.wofNPCIndex].velocity.X > 0f)
                            maxVelocity += 6f;

                        if (NPC.Center.X > Main.npc[Main.wofNPCIndex].Center.X && Main.npc[Main.wofNPCIndex].velocity.X < 0f)
                            maxVelocity += 6f;
                    }

                    if (NPC.velocity.X > maxVelocity)
                        NPC.velocity.X = maxVelocity;

                    if (NPC.velocity.X < -maxVelocity)
                        NPC.velocity.X = -maxVelocity;

                    if (NPC.velocity.Y > maxVelocity)
                        NPC.velocity.Y = maxVelocity;

                    if (NPC.velocity.Y < -maxVelocity)
                        NPC.velocity.Y = -maxVelocity;
                }
                else if (NPC.ai[1] > 0f)
                    NPC.ai[1] -= 1f;
                else
                    NPC.ai[1] = 0f;

                if (num406 > 0f)
                {
                    NPC.spriteDirection = 1;
                    NPC.rotation = (float)Math.Atan2(num407, num406);
                }

                if (num406 < 0f)
                {
                    NPC.spriteDirection = -1;
                    NPC.rotation = (float)Math.Atan2(num407, num406) + MathHelper.Pi;
                }

                Lighting.AddLight(NPC.Center, 0.3f, 0.2f, 0.1f);

                return false;
            }
        }

        public class EyeAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Avoid cheap bullshit
                NPC.damage = 0;

                // Despawn
                if (Main.wofNPCIndex < 0)
                {
                    NPC.active = false;
                    return false;
                }

                NPC.realLife = Main.wofNPCIndex;

                if (Main.npc[Main.wofNPCIndex].life > 0)
                    NPC.life = Main.npc[Main.wofNPCIndex].life;

                // Percent life remaining
                float lifeRatio = Main.npc[Main.wofNPCIndex].life / (float)Main.npc[Main.wofNPCIndex].lifeMax;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.target = Main.npc[Main.wofNPCIndex].target;

                // Velocity, direction, and position
                bool shouldFireLasers = true;
                float phase2LifeRatio = 0.4f;
                bool deathModeDetach = lifeRatio < phase2LifeRatio && death;
                bool canHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                if (!deathModeDetach)
                {
                    NPC.position.X = Main.npc[Main.wofNPCIndex].position.X;
                    NPC.direction = Main.npc[Main.wofNPCIndex].direction;
                    NPC.spriteDirection = NPC.direction;

                    float expectedPosition = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2;
                    if (NPC.ai[0] > 0f)
                        expectedPosition = (expectedPosition + Main.wofDrawAreaTop) / 2f;
                    else
                        expectedPosition = (expectedPosition + Main.wofDrawAreaBottom) / 2f;
                    expectedPosition -= NPC.height / 2;

                    bool belowExpectedPosition = NPC.position.Y > expectedPosition + 1f;
                    bool aboveExpectedPosition = NPC.position.Y < expectedPosition - 1f;
                    if (belowExpectedPosition)
                    {
                        float distanceBelowExpectedPosition = NPC.position.Y - expectedPosition + 1f;
                        float movementVelocity = MathHelper.Clamp(distanceBelowExpectedPosition * 0.03125f, 1f, 5f);
                        NPC.velocity.Y = -movementVelocity;
                    }
                    else if (aboveExpectedPosition)
                    {
                        float distanceAboveExpectedPosition = expectedPosition - 1f - NPC.position.Y;
                        float movementVelocity = MathHelper.Clamp(distanceAboveExpectedPosition * 0.03125f, 1f, 5f);
                        NPC.velocity.Y = movementVelocity;
                    }
                    else
                    {
                        NPC.velocity.Y = 0f;
                        NPC.position.Y = expectedPosition;
                    }
                }
                else
                {
                    float distanceAboveTarget = (canHit ? 240f : 120f) * NPC.ai[0];
                    float distanceAwayFromTargetX = 560f;
                    float distanceAwayFromTargetXLeeway = 40f;
                    float distanceAwayFromTargetY = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                    float distanceAwayFromTargetYLeeway = 40f;
                    float absoluteDistanceX = Math.Abs(Main.player[NPC.target].Center.X - NPC.Center.X);
                    bool tooFarX = absoluteDistanceX > distanceAwayFromTargetX + distanceAwayFromTargetXLeeway || absoluteDistanceX < distanceAwayFromTargetX - distanceAwayFromTargetXLeeway;
                    bool tooFarY = distanceAwayFromTargetY > distanceAboveTarget + distanceAwayFromTargetYLeeway || distanceAwayFromTargetY < distanceAboveTarget - distanceAwayFromTargetYLeeway;
                    bool tooFar = tooFarX || tooFarY;
                    Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * distanceAboveTarget + Vector2.UnitX * distanceAwayFromTargetX * NPC.ai[3];
                    if (tooFar)
                    {
                        Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * 16f;
                        NPC.SimpleFlyMovement(idealVelocity, 0.36f);
                    }

                    if (NPC.Distance(Main.player[NPC.target].Center) < distanceAwayFromTargetX || NPC.Distance(hoverDestination) > 120f)
                        shouldFireLasers = false;

                    float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    if (NPC.ai[1] == 0f)
                    {
                        NPC.ai[1] = 1f;
                        SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);
                        for (int i = 0; i < 100; i++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.Y);
                            Main.dust[dust].scale = Main.rand.NextFloat(1.5f, 4f);
                            Main.dust[dust].velocity *= Main.rand.NextFloat(0.5f, 1.5f);
                        }
                    }

                    // 50% chance to change Y position
                    float eyePositionRandomChangeGateValue = MathHelper.Lerp(death ? 180f : 240f, death ? 480f : 720f, lifeRatio / phase2LifeRatio);
                    if (NPC.ai[2] >= eyePositionRandomChangeGateValue)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[0] = Main.rand.NextBool() ? 1f : -1f;
                        NPC.netUpdate = true;
                    }
                    NPC.ai[2] += 1f;
                }

                Vector2 eyeLocation = NPC.Center;
                Vector2 lookAt = Main.player[NPC.target].Center;
                float eyeTargetX = lookAt.X - eyeLocation.X;
                float eyeTargetY = lookAt.Y - eyeLocation.Y;
                float wallVelocity = (float)Math.Sqrt(eyeTargetX * eyeTargetX + eyeTargetY * eyeTargetY);
                eyeTargetX *= wallVelocity;
                eyeTargetY *= wallVelocity;

                // Rotation based on direction and whether to fire lasers or not
                if (NPC.direction > 0)
                {
                    if (Main.player[NPC.target].Center.X > NPC.Center.X)
                    {
                        NPC.rotation = (float)Math.Atan2(-eyeTargetY, -eyeTargetX) + MathHelper.Pi;
                    }
                    else
                    {
                        NPC.rotation = 0f;
                        if (!deathModeDetach)
                            shouldFireLasers = false;
                    }
                }
                else if (Main.player[NPC.target].Center.X < NPC.Center.X)
                {
                    NPC.rotation = (float)Math.Atan2(eyeTargetY, eyeTargetX) + MathHelper.Pi;
                }
                else
                {
                    NPC.rotation = 0f;
                    if (!deathModeDetach)
                        shouldFireLasers = false;
                }

                // Fire lasers
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bool charging = Main.npc[Main.wofNPCIndex].ai[3] == 1f;

                    // Set up enraged laser firing timer
                    float enragedLaserTimer = EnragedLaserFiringDuration;
                    if (charging)
                        NPC.localAI[3] = enragedLaserTimer;

                    bool fireEnragedLasers = NPC.localAI[3] > 0f && NPC.localAI[3] < enragedLaserTimer;

                    // Decrement the enraged laser timer
                    if (NPC.localAI[3] > 0f)
                    {
                        NPC.localAI[3] -= 1f;

                        // Stop firing normal lasers when enrage ends
                        if (NPC.localAI[3] == 0f)
                            NPC.localAI[1] = 0f;
                    }

                    float shootBoost = fireEnragedLasers ? (death ? 5f : 4f) : (death ? 3f : 3f * (1f - lifeRatio));
                    NPC.localAI[1] += 1f + shootBoost;

                    if (NPC.localAI[2] == 0f)
                    {
                        if (NPC.localAI[1] > LaserShootGateValue)
                        {
                            NPC.localAI[2] = 1f;
                            NPC.localAI[1] = 0f;
                        }
                    }
                    else if (NPC.localAI[1] > 45f && (canHit || deathModeDetach) && !charging)
                    {
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= TotalLasersPerBarrage + 1f)
                            NPC.localAI[2] = 0f;

                        if (shouldFireLasers)
                        {
                            float velocity = (fireEnragedLasers ? 3f : 4f) + shootBoost;
                            int projectileType = ProjectileID.EyeLaser;

                            bool targetTooClose = NPC.Distance(Main.player[NPC.target].Center) < 160f;
                            float projectileOffset = targetTooClose ? 60f : 150f;
                            Vector2 projectileVelocity = (lookAt - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                            Vector2 projectileSpawn = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * projectileOffset;

                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, LaserDamage, 0f, Main.myPlayer, 1f, 0f);
                            Main.projectile[proj].timeLeft = 900;

                            if (!canHit)
                                Main.projectile[proj].tileCollide = false;
                        }
                    }
                }

                return false;
            }

            public override void PostDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                // Laser telegraph
                bool enraged = NPC.localAI[3] > 0f;
                float eyeTelegraphGateValue = LaserShootGateValue - LaserShootTelegraphTime;
                if (NPC.localAI[1] > eyeTelegraphGateValue || NPC.localAI[2] > 0f || enraged)
                {
                    var halfSize = NPC.frame.Size() / 2;
                    SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    float colorScale = enraged ? MathHelper.Clamp(NPC.localAI[3] / EnragedLaserFiringDuration, 0f, 1f) :
                        NPC.localAI[2] > 0f ? 1f - ((NPC.localAI[2] - 1f) / TotalLasersPerBarrage) :
                        MathHelper.Clamp((NPC.localAI[1] - eyeTelegraphGateValue) / LaserShootTelegraphTime, 0f, 1f);

                    Color drawColor2 = new Color(128, 0, 255) * colorScale;
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame,
                        drawColor2, NPC.rotation, halfSize, NPC.scale, spriteEffects, 0f);
                }
            }
        }
    }
}
