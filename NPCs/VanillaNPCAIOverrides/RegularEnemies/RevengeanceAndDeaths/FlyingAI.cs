using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;

public static partial class RevengeanceAndDeathAI
{
    public class FlyingAI : VanillaAIOverride
    {
        private void BounceOffTiles(float reboundSpeed)
        {
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -reboundSpeed;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    NPC.velocity.X = 2f;
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    NPC.velocity.X = -2f;
            }

            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -reboundSpeed;
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                    NPC.velocity.Y = 2f;
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                    NPC.velocity.Y = -2f;
            }
        }

        public override bool AI(Mod mod)
        {
            if (NPC.target < 0 || NPC.target <= Main.maxPlayers || Main.player[NPC.target].dead)
                NPC.TargetClosest();

            if (NPC.type == NPCID.BloodSquid)
            {
                if (Main.dayTime)
                {
                    NPC.velocity.Y -= 0.3f;
                    NPC.EncourageDespawn(60);
                }

                NPC.position += NPC.netOffset;
                if (NPC.alpha == 255)
                {
                    NPC.spriteDirection = NPC.direction;
                    NPC.velocity.Y = -6f;
                    for (int i = 0; i < 35; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood);
                        dust.velocity *= 1f;
                        dust.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        dust.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        dust.velocity += NPC.velocity * 0.5f;
                    }
                }

                NPC.alpha -= 15;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                if (NPC.alpha != 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust eyeFishDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood);
                        eyeFishDust.velocity *= 1f;
                        eyeFishDust.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        eyeFishDust.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        eyeFishDust.velocity += NPC.velocity * 0.3f;
                    }
                }

                NPC.position -= NPC.netOffset;
            }

            NPCAimedTarget targetData = NPC.GetTargetData();
            bool targetDead = false;
            if (targetData.Type == NPCTargetType.Player)
                targetDead = Main.player[NPC.target].dead;

            bool queenBeeHornet = NPC.type == NPCID.HornetHoney && NPC.ai[3] == 1f;
            if (queenBeeHornet)
            {
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    if (NPC.localAI[1] != 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] = 0f;
                        NPC.SyncVanillaLocalAI();
                    }
                }
                else if (NPC.localAI[1] == 0f)
                    NPC.localAI[2]++;

                if (NPC.localAI[2] >= (CalamityWorld.death ? 60f : 120f))
                {
                    NPC.localAI[1] = 1f;
                    NPC.localAI[2] = 0f;
                    NPC.SyncVanillaLocalAI();
                }

                if (NPC.localAI[1] == 0f)
                {
                    NPC.alpha = 0;
                    NPC.noTileCollide = false;
                }
                else
                {
                    NPC.wet = false;
                    NPC.alpha = 200;
                    NPC.noTileCollide = true;
                }
            }

            bool deathModeVelocityBuff = true;
            float maxVelocity = 6f;
            float acceleration = 0.05f;
            if (NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Crimera)
            {
                maxVelocity = 4f;
                acceleration = 0.035f;
            }
            else if (NPC.type == NPCID.Corruptor)
            {
                maxVelocity = 4.2f;
                acceleration = 0.022f;
            }
            else if (NPC.type == NPCID.BloodSquid)
            {
                maxVelocity = 6f;
                acceleration = 0.1f;
            }
            else if (NPC.type == NPCID.Hornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy))
            {
                maxVelocity = 3.5f;
                acceleration = 0.021f;
                if (NPC.type == NPCID.HornetFatty)
                {
                    maxVelocity = 3f;
                    acceleration = 0.017f;
                }

                maxVelocity *= 1f - NPC.scale;
                acceleration *= 1f - NPC.scale;

                // Despawn
                if ((double)(NPC.position.Y / 16f) < Main.worldSurface && !queenBeeHornet)
                {
                    if (Main.player[NPC.target].position.Y - NPC.position.Y > 300f && NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.97f;

                    if (Main.player[NPC.target].position.Y - NPC.position.Y < 80f && NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.97f;
                }

                // Master Mode Queen Bee Hornets
                if (queenBeeHornet)
                {
                    maxVelocity *= 1.4f;
                    acceleration *= 1.8f;
                    maxVelocity += NPC.ai[2] * 1.5f;
                    acceleration += NPC.ai[2] * 0.01f;
                }
            }
            else if (NPC.type == NPCID.MossHornet)
            {
                maxVelocity = 4f;
                acceleration = 0.017f;
            }
            else if (NPC.type == NPCID.Parrot)
            {
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    maxVelocity = 6f;
                    acceleration = 0.1f;
                }
                else
                {
                    acceleration = 0.01f;
                    maxVelocity = 2f;
                }
            }
            else if (NPC.type == NPCID.Moth)
            {
                maxVelocity = 7f;
                acceleration = 0.06f;
            }
            else if (NPC.type == NPCID.MeteorHead)
            {
                maxVelocity = 2f;
                acceleration = 0.05f;
            }
            else if (NPC.type == NPCID.ServantofCthulhu)
            {
                maxVelocity = 5f + NPC.ai[2] * 2f;
                acceleration = 0.03f + NPC.ai[2] * 0.03f;
            }
            else if (NPC.type == NPCID.Bee || NPC.type == NPCID.BeeSmall)
            {
                bool notQueenBeeBee = NPC.ai[3] == 0f;
                if (notQueenBeeBee)
                {
                    NPC.ai[1] += 1f;
                    float originalFlyAwayTime = 60f;
                    float flyAwayTime = CalamityWorld.death ? 30f : 40f;
                    float originalFlyAwayVelocity = 6f;
                    float flyAwayDistance = originalFlyAwayTime * originalFlyAwayVelocity;
                    float flyAwayVelocity = flyAwayDistance / flyAwayTime;
                    float flyAwayAccel = (NPC.ai[1] - flyAwayTime) / flyAwayTime;
                    if (flyAwayAccel > 1f)
                    {
                        flyAwayAccel = 1f;
                    }
                    else
                    {
                        if (NPC.velocity.X > flyAwayVelocity)
                            NPC.velocity.X = flyAwayVelocity;

                        if (NPC.velocity.X < -flyAwayVelocity)
                            NPC.velocity.X = -flyAwayVelocity;

                        if (NPC.velocity.Y > flyAwayVelocity)
                            NPC.velocity.Y = flyAwayVelocity;

                        if (NPC.velocity.Y < -flyAwayVelocity)
                            NPC.velocity.Y = -flyAwayVelocity;
                    }

                    maxVelocity = 5f;
                    acceleration = 0.1f * flyAwayAccel;
                }
                else
                {
                    deathModeVelocityBuff = false;
                    maxVelocity = 5f + NPC.ai[2] * 2f;
                    acceleration = 0.1f + NPC.ai[2] * 0.04f;
                }
            }

            if (CalamityWorld.revenge)
            {
                maxVelocity *= 1.25f;
                acceleration *= 1.25f;
            }

            if (CalamityWorld.death && deathModeVelocityBuff)
            {
                maxVelocity *= 1.25f;
                acceleration *= 1.25f;
            }

            if (queenBeeHornet)
            {
                float deceleration = 1f - acceleration;
                if (targetDead)
                {
                    Vector2 destination = NPC.Center - Vector2.UnitY;
                    Vector2 idealVelocity = NPC.SafeDirectionTo(destination) * maxVelocity * 0.5f;
                    idealVelocity.X *= NPC.direction;
                    idealVelocity.Y *= 2.5f;
                    NPC.SimpleFlyMovement(idealVelocity, acceleration);
                    NPC.EncourageDespawn(10);
                    NPC.wet = false;
                    NPC.noTileCollide = true;
                }
                else if (NPC.Distance(targetData.Center) > 400f)
                {
                    Vector2 idealVelocity = NPC.SafeDirectionTo(targetData.Center) * maxVelocity;
                    NPC.SimpleFlyMovement(idealVelocity, acceleration);
                }
                else
                {
                    if (NPC.Distance(targetData.Center) < 160f)
                    {
                        Vector2 idealVelocity = NPC.SafeDirectionTo(targetData.Center) * maxVelocity;
                        NPC.SimpleFlyMovement(-idealVelocity, acceleration);
                    }
                    else
                        NPC.velocity *= deceleration;
                }

                if (targetData.Center.X - NPC.Center.X > 0f)
                    NPC.spriteDirection = 1;
                else
                    NPC.spriteDirection = -1;

                NPC.rotation = NPC.velocity.X * 0.1f;

                float reboundSpeed = 0.7f;
                BounceOffTiles(reboundSpeed);
            }
            else
            {
                Vector2 vector = NPC.Center;
                float targetXDist = Main.player[NPC.target].Center.X;
                float targetYDist = Main.player[NPC.target].Center.Y;
                targetXDist = (float)((int)(targetXDist / 8f) * 8);
                targetYDist = (float)((int)(targetYDist / 8f) * 8);
                vector.X = (float)((int)(vector.X / 8f) * 8);
                vector.Y = (float)((int)(vector.Y / 8f) * 8);
                targetXDist -= vector.X;
                targetYDist -= vector.Y;
                float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                float targetDistCheck = targetDistance;

                if (targetDistance == 0f)
                {
                    targetXDist = NPC.velocity.X;
                    targetYDist = NPC.velocity.Y;
                }
                else
                {
                    targetDistance = maxVelocity / targetDistance;
                    targetXDist *= targetDistance;
                    targetYDist *= targetDistance;
                }

                if (NPC.type == NPCID.Hornet || NPC.type == NPCID.MossHornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy) || NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Crimera || NPC.type == NPCID.Moth || NPC.type == NPCID.Bee || NPC.type == NPCID.BeeSmall || NPC.type == NPCID.BloodSquid)
                {
                    if (targetDistCheck > 100f || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Bee || NPC.type == NPCID.BeeSmall || NPC.type == NPCID.BloodSquid || NPC.type == NPCID.Hornet || NPC.type == NPCID.MossHornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy))
                    {
                        NPC.ai[0] += 1f;
                        if (NPC.ai[0] > 0f)
                            NPC.velocity.Y += CalamityWorld.revenge ? 0.03f : 0.023f;
                        else
                            NPC.velocity.Y -= CalamityWorld.revenge ? 0.03f : 0.023f;

                        if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                            NPC.velocity.X += CalamityWorld.revenge ? 0.03f : 0.023f;
                        else
                            NPC.velocity.X -= CalamityWorld.revenge ? 0.03f : 0.023f;

                        if (NPC.ai[0] > 200f)
                            NPC.ai[0] = -200f;
                    }

                    if (targetDistCheck < 150f && (NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Crimera || NPC.type == NPCID.BloodSquid))
                    {
                        NPC.velocity.X += targetXDist * (CalamityWorld.revenge ? 0.009f : 0.007f);
                        NPC.velocity.Y += targetYDist * (CalamityWorld.revenge ? 0.009f : 0.007f);
                    }

                    // Master Mode Queen Bee Bees
                    if (NPC.type == NPCID.Bee || NPC.type == NPCID.BeeSmall)
                    {
                        if (NPC.ai[3] == 1f)
                        {
                            float pushVelocity = 0.5f + NPC.ai[2] * 0.2f;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active)
                                {
                                    if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                                    {
                                        if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 32f * NPC.scale)
                                        {
                                            if (NPC.position.X < Main.npc[i].position.X)
                                                NPC.velocity.X -= pushVelocity;
                                            else
                                                NPC.velocity.X += pushVelocity;

                                            if (NPC.position.Y < Main.npc[i].position.Y)
                                                NPC.velocity.Y -= pushVelocity;
                                            else
                                                NPC.velocity.Y += pushVelocity;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (targetDead)
                {
                    targetXDist = (float)NPC.direction * maxVelocity / 2f;
                    targetYDist = -maxVelocity / 2f;
                }

                if (NPC.velocity.X < targetXDist)
                {
                    NPC.velocity.X += acceleration;
                    if (NPC.type != NPCID.Crimera && NPC.type != NPCID.EaterofSouls && NPC.type != NPCID.Corruptor && NPC.type != NPCID.BloodSquid && NPC.velocity.X < 0f && targetXDist > 0f)
                        NPC.velocity.X += acceleration;
                }
                else if (NPC.velocity.X > targetXDist)
                {
                    NPC.velocity.X -= acceleration;
                    if (NPC.type != NPCID.Crimera && NPC.type != NPCID.EaterofSouls && NPC.type != NPCID.Corruptor && NPC.type != NPCID.BloodSquid && NPC.velocity.X > 0f && targetXDist < 0f)
                        NPC.velocity.X -= acceleration;
                }

                if (NPC.velocity.Y < targetYDist)
                {
                    NPC.velocity.Y += acceleration;
                    if (NPC.type != NPCID.Crimera && NPC.type != NPCID.EaterofSouls && NPC.type != NPCID.Corruptor && NPC.type != NPCID.BloodSquid && NPC.velocity.Y < 0f && targetYDist > 0f)
                        NPC.velocity.Y += acceleration;
                }
                else if (NPC.velocity.Y > targetYDist)
                {
                    NPC.velocity.Y -= acceleration;
                    if (NPC.type != NPCID.Crimera && NPC.type != NPCID.EaterofSouls && NPC.type != NPCID.Corruptor && NPC.type != NPCID.BloodSquid && NPC.velocity.Y > 0f && targetYDist < 0f)
                        NPC.velocity.Y -= acceleration;
                }

                if (NPC.type == NPCID.ServantofCthulhu)
                {
                    float pushVelocity = 0.5f + NPC.ai[2] * 0.25f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                            {
                                if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 48f * NPC.scale)
                                {
                                    if (NPC.position.X < Main.npc[i].position.X)
                                        NPC.velocity.X -= pushVelocity;
                                    else
                                        NPC.velocity.X += pushVelocity;

                                    if (NPC.position.Y < Main.npc[i].position.Y)
                                        NPC.velocity.Y -= pushVelocity;
                                    else
                                        NPC.velocity.Y += pushVelocity;
                                }
                            }
                        }
                    }
                }

                if (NPC.type == NPCID.MeteorHead)
                {
                    if (targetXDist > 0f)
                    {
                        NPC.spriteDirection = 1;
                        NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist);
                    }
                    else if (targetXDist < 0f)
                    {
                        NPC.spriteDirection = -1;
                        NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist) + MathHelper.Pi;
                    }
                }
                else if (NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Crimera || NPC.type == NPCID.BloodSquid)
                {
                    NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist) - MathHelper.PiOver2;
                }
                else if (NPC.type == NPCID.Moth || NPC.type == NPCID.Hornet || NPC.type == NPCID.MossHornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.spriteDirection = 1;
                    if (NPC.velocity.X < 0f)
                        NPC.spriteDirection = -1;

                    NPC.rotation = NPC.velocity.X * 0.1f;
                }
                else
                    NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) - MathHelper.PiOver2;

                if (NPC.type == NPCID.Hornet || NPC.type == NPCID.MossHornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy) || NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.MeteorHead || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Crimera || NPC.type == NPCID.Moth || NPC.type == NPCID.Bee || NPC.type == NPCID.BeeSmall || NPC.type == NPCID.BloodSquid)
                {
                    float reboundSpeed = 0.7f;
                    if (NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Crimera)
                        reboundSpeed = 0.4f;

                    BounceOffTiles(reboundSpeed);

                    if (NPC.type == NPCID.BloodSquid)
                    {
                        int bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100);
                        Main.dust[bloodDust].velocity *= 0.5f;
                    }
                    else if (NPC.type == NPCID.MeteorHead)
                    {
                        int meteorDust = Dust.NewDust(new Vector2(NPC.position.X - NPC.velocity.X, NPC.position.Y - NPC.velocity.Y), NPC.width, NPC.height, DustID.Torch, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default(Color), 2f);
                        Dust dust = Main.dust[meteorDust];
                        dust.noGravity = true;
                        dust.velocity.X *= 0.3f;
                        dust.velocity.Y *= 0.3f;
                    }
                    else if (NPC.type != NPCID.Moth && NPC.type != NPCID.Parrot && NPC.type != NPCID.Bee && NPC.type != NPCID.BeeSmall && Main.rand.NextBool(20))
                    {
                        int dustType = 18;
                        if (NPC.type == NPCID.Crimera)
                            dustType = 5;

                        int idleDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + (float)NPC.height * 0.25f), NPC.width, (int)((float)NPC.height * 0.5f), dustType, NPC.velocity.X, 2f, 75, NPC.color, NPC.scale);
                        Dust dust = Main.dust[idleDust];
                        dust.velocity.X *= 0.5f;
                        dust.velocity.Y *= 0.1f;
                    }
                }
                else if (NPC.type != NPCID.Parrot && Main.rand.NextBool(40))
                {
                    int otherIdleDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + (float)NPC.height * 0.25f), NPC.width, (int)((float)NPC.height * 0.5f), DustID.Blood, NPC.velocity.X, 2f, 0, default(Color), 1f);
                    Dust dust = Main.dust[otherIdleDust];
                    dust.velocity.X *= 0.5f;
                    dust.velocity.Y *= 0.1f;
                }

                if ((NPC.type == NPCID.EaterofSouls || NPC.type == NPCID.Corruptor || NPC.type == NPCID.Crimera || NPC.type == NPCID.BloodSquid) && NPC.wet)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.95f;

                    NPC.velocity.Y -= CalamityWorld.revenge ? 0.4f : 0.3f;
                    if (NPC.velocity.Y < -(CalamityWorld.revenge ? 3f : 2f))
                        NPC.velocity.Y = -(CalamityWorld.revenge ? 3f : 2f);
                }

                if (NPC.type == NPCID.Moth && NPC.wet)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.95f;

                    NPC.velocity.Y -= 0.7f;
                    if (NPC.velocity.Y < -6f)
                        NPC.velocity.Y = -6f;

                    NPC.TargetClosest();
                }
            }

            if (NPC.type == NPCID.Hornet || NPC.type == NPCID.MossHornet || (NPC.type >= NPCID.HornetFatty && NPC.type <= NPCID.HornetStingy))
            {
                if (NPC.wet)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.95f;

                    NPC.velocity.Y -= 0.5f;
                    if (NPC.velocity.Y < -4f)
                        NPC.velocity.Y = -4f;

                    NPC.TargetClosest();
                }

                if (NPC.ai[1] == 301f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    NPC.ai[1] = 0f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[1] += ((NPC.type == NPCID.MossHornet || queenBeeHornet) ? 2f : 1f) + NPC.ai[2];
                    if (NPC.justHit && !queenBeeHornet)
                        NPC.ai[1] = 0f;

                    if (NPC.ai[1] >= 240f)
                    {
                        if (targetData.Type != 0 && Collision.CanHit(NPC, targetData))
                        {
                            float projSpeed = (CalamityWorld.death || Main.hardMode) ? 5f : 8f;
                            projSpeed += NPC.ai[2] * ((CalamityWorld.death || Main.hardMode) ? 2f : 4f);
                            if (queenBeeHornet)
                                projSpeed += 2f;

                            Vector2 projSpawnPosition = NPC.Center;
                            float projTargetXDist = targetData.Center.X - projSpawnPosition.X;
                            float projTargetYDist = targetData.Center.Y - projSpawnPosition.Y;
                            if ((projTargetXDist < 0f && NPC.velocity.X < 0f) || (projTargetXDist > 0f && NPC.velocity.X > 0f))
                            {
                                float projTargetDistance = (float)Math.Sqrt(projTargetXDist * projTargetXDist + projTargetYDist * projTargetYDist);
                                projTargetDistance = projSpeed / projTargetDistance;
                                projTargetXDist *= projTargetDistance;
                                projTargetYDist *= projTargetDistance;

                                // Master Mode Queen Bee Hornets deal increased damage
                                int projDamage = (int)((queenBeeHornet ? 15f : 10f) * NPC.scale);
                                if (NPC.type == NPCID.MossHornet)
                                    projDamage = (int)(30f * NPC.scale);

                                int stingerType = ProjectileID.Stinger;
                                int stingerSpawn = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), projSpawnPosition, new Vector2(projTargetXDist, projTargetYDist), stingerType, projDamage, 0f, Main.myPlayer).identity;
                                Main.projectile[stingerSpawn].timeLeft = (CalamityWorld.death || Main.hardMode) ? 600 : 300;
                                Main.projectile[stingerSpawn].Calamity().extraUpdatesToSync = (CalamityWorld.death || Main.hardMode) ? 1 : 0;
                                if (Main.dedServ)
                                {
                                    Main.projectile[stingerSpawn].netSpam = 0;
                                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, stingerSpawn);
                                }
                                NPC.ai[1] = 301f;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.ai[1] = 0f;
                        }
                        else
                            NPC.ai[1] = 0f;
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && !targetDead)
            {
                if (Main.getGoodWorld && NPC.type == NPCID.EaterofSouls)
                {
                    if (NPC.AnyNPCs(NPCID.EaterofWorldsHead))
                    {
                        if (NPC.justHit)
                            NPC.localAI[0] = 0f;

                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] == 60f)
                        {
                            if (targetData.Type != 0 && Collision.CanHit(NPC, targetData))
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2) + NPC.velocity.X), (int)(NPC.position.Y + (float)(NPC.height / 2) + NPC.velocity.Y), NPCID.VileSpitEaterOfWorlds);

                            NPC.localAI[0] = 0f;
                        }
                    }
                }

                if (NPC.type == NPCID.Corruptor)
                {
                    if (NPC.justHit || !Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        NPC.localAI[0] = 0f;

                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] == CorruptorVileSpitGateValue)
                    {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2) + NPC.velocity.X), (int)(NPC.position.Y + (float)(NPC.height / 2) + NPC.velocity.Y), NPCID.VileSpit);

                        NPC.localAI[0] = 0f;
                    }

                    if (NPC.localAI[0] > CorruptorVileSpitGateValue - CorruptorVileSpitTelegraphTime)
                    {
                        Vector2 dustCenter = NPC.Center + NPC.SafeDirectionTo(Main.player[NPC.target].Center, -Vector2.UnitY) * 25f + Main.rand.NextVector2CircularEdge(3f, 3f);
                        Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.CorruptGibs, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 80, default, 1.3f);
                        dust.noGravity = true;
                        dust.velocity *= 0.3f;
                    }
                }

                if (NPC.type == NPCID.BloodSquid)
                {
                    if (NPC.justHit || targetData.Type == 0 || !Collision.CanHit(NPC, targetData))
                        NPC.localAI[0] = 0f;

                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= BloodSquidBloodShotGateValue)
                    {
                        if (targetData.Type != 0 && Collision.CanHit(NPC, targetData))
                        {
                            if ((NPC.Center - targetData.Center).Length() < 400f)
                            {
                                Vector2 bloodShotPosition = NPC.DirectionTo(new Vector2(targetData.Center.X, targetData.Position.Y));
                                NPC.velocity = -bloodShotPosition * 5f;
                                NPC.netUpdate = true;
                                NPC.localAI[0] = 0f;
                                bloodShotPosition = NPC.DirectionTo(new Vector2(targetData.Center.X, targetData.Position.Y));
                                int proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, bloodShotPosition * (CalamityWorld.death ? 6f : 10f), ProjectileID.BloodShot, 50, 1f, Main.myPlayer).identity;
                                if (CalamityWorld.death)
                                {
                                    Main.projectile[proj].Calamity().extraUpdatesToSync = 1;
                                    Main.projectile[proj].timeLeft = 1200;
                                    if (Main.dedServ)
                                    {
                                        Main.projectile[proj].netSpam = 0;
                                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                                    }
                                }
                            }
                            else
                                NPC.localAI[0] = 0f;
                        }
                        else
                            NPC.localAI[0] = 0f;
                    }

                    if (NPC.localAI[0] > BloodSquidBloodShotGateValue - BloodSquidBloodShotTelegraphTime)
                    {
                        Vector2 dustCenter = NPC.Center + NPC.SafeDirectionTo(Main.player[NPC.target].Center, -Vector2.UnitY) * 25f + Main.rand.NextVector2CircularEdge(5f, 5f);
                        Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.Blood, 0f, 0f, 100, default, 3f);
                        dust.fadeIn = 1.7f;
                        dust.noGravity = true;
                        dust.velocity *= 0f;
                    }
                }
            }

            if (!queenBeeHornet)
            {
                if ((Main.dayTime && NPC.type != NPCID.Crimera && NPC.type != NPCID.EaterofSouls && NPC.type != NPCID.MeteorHead && NPC.type != NPCID.Bee && NPC.type != NPCID.BeeSmall && NPC.type != NPCID.Corruptor && NPC.type != NPCID.Moth && NPC.type != NPCID.Parrot && NPC.type != NPCID.BloodSquid) || Main.player[NPC.target].dead)
                {
                    NPC.velocity.Y -= acceleration * 2f;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                }
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.netUpdate = true;

            return false;
        }
    }
}
