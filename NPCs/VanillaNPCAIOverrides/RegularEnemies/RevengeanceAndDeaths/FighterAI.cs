using System;
using System.Collections.Generic;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;

public static partial class RevengeanceAndDeathAI
{
    public class FighterAI : VanillaAIOverride
    {
        // Methods for AIs that are always terminated via a return in the Fighter AI code. More specialized and general
        // functions are given summaries of their specifics
        public static void BuffedPsychoAI(NPC npc)
        {
            int psychoAlphaMax = 200;
            // Standing still
            if (npc.ai[2] == 0f)
            {
                npc.alpha = psychoAlphaMax;
                npc.TargetClosest(true);
                if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f)
                {
                    npc.ai[2] = -16f;
                }
                if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
                {
                    npc.ai[2] = -16f;
                }
            }
            // Active
            if (npc.ai[2] < 0f)
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= psychoAlphaMax / 16;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.ai[2] = 1f;
                    npc.velocity.X = (float)(npc.direction * 2);
                }
            }
        }
        public static void BuffedSwampThingAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(240))
            {
                npc.ai[2] = (float)Main.rand.Next(-480, -60);
                npc.netUpdate = true;
            }
            if (npc.ai[2] < 0f)
            {
                npc.TargetClosest(true);
                if (npc.justHit)
                {
                    npc.ai[2] = 0f;
                }
                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[2] = 0f;
                }
            }
            if (npc.ai[2] < 0f)
            {
                npc.velocity.X *= 0.9f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                {
                    npc.velocity.X = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.velocity.X = (float)npc.direction * 0.1f;
                }
            }
        }
        // Misc effects used more than once in the Fighter AI
        public static void MedusaHeadDustEffect(NPC npc, float time)
        {
            Vector2 headPosition = npc.Top + new Vector2((float)(npc.spriteDirection * 6), 6f);
            float rotationVectorMult = MathHelper.Lerp(20f, 30f, (time * 3f + 50f) / 182f);
            Main.rand.NextFloat();
            for (float i = 0f; i < 2f; i += 1f)
            {
                Vector2 rotationVector = Vector2.UnitY.RotatedByRandom(Math.PI * 2.0) * (Main.rand.NextFloat() * 0.5f + 0.5f);
                Dust dust = Dust.NewDustDirect(headPosition, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default, 1f);
                dust.position = headPosition + rotationVector * rotationVectorMult;
                dust.noGravity = true;
                dust.velocity = rotationVector * 2f;
                dust.scale = 0.5f + Main.rand.NextFloat() * 0.5f;
            }
        }

        /// <summary>
        /// Causes an NPC to run on the X axis until it hits the maximum speed and decclerates as needed.
        /// Works best with fighter AI based NPCs.
        /// </summary>
        /// <param name="npc">The NPC to manipulate</param>
        /// <param name="velocityMax">The max speed to run at.</param>
        /// <param name="acceleration">The rate at which the X velocity changes with time.</param>
        /// <param name="turnDeceleration">The X velocity deceleration multiplier when the NPC will turn to another direction.</param>
        /// <param name="extraDeceleration">Causes the NPC to slow down faster when not jumping for direction turning if true.</param>
        /// <param name="turnDeceleration">The X velocity deceleration multiplier when the NPC will turn to another direction.</param>
        /// <param name="extraDecelerationFactor">The X velocity deceleration multiplier used by <paramref name="turnDeceleration"/>.</param>
        public static void FighterRunningAI(NPC npc, float velocityMax, float acceleration, float turnDeceleration,
            bool extraDeceleration = false, float extraDecelerationFactor = 0.99f)
        {
            if (npc.velocity.X < -velocityMax || npc.velocity.X > velocityMax)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= turnDeceleration;
                }
            }
            else if (npc.velocity.X < velocityMax && npc.direction == 1)
            {
                npc.velocity.X += acceleration;

                if (extraDeceleration && npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    npc.velocity.X *= extraDecelerationFactor;

                if (npc.velocity.X > velocityMax)
                {
                    npc.velocity.X = velocityMax;
                }
            }
            else if (npc.velocity.X > -velocityMax && npc.direction == -1)
            {
                if (extraDeceleration && npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    npc.velocity.X *= extraDecelerationFactor;
                npc.velocity.X -= acceleration;
                if (npc.velocity.X < -velocityMax)
                {
                    npc.velocity.X = -velocityMax;
                }
            }
        }

        public static void TryConvertToWallClimber(NPC npc)
        {
            // Note: The Possessed rely on an AI shift rather than a transformation NPC
            // As a result, they are not included in this method

            List<int> spiders = new List<int>()
        {
            NPCID.BlackRecluse,
            NPCID.BloodCrawler,
            NPCID.DesertScorpionWalk,
            NPCID.JungleCreeper,
            NPCID.WallCreeper,
            ModContent.NPCType<AstralachneaGround>()
        };
            // These checks are not required if the npc is not a real spider
            if (!spiders.Contains(npc.type))
                return;
            int tileCoordsX = (int)npc.Center.X / 16;
            int tileCoordsY = (int)npc.Center.Y / 16;
            bool climbWalls = false;
            for (int x = tileCoordsX - 1; x <= tileCoordsX + 1; x++)
            {
                for (int y = tileCoordsY - 1; y <= tileCoordsY + 1; y++)
                {
                    if (Main.tile[x, y].WallType > WallID.None)
                    {
                        climbWalls = true;
                    }
                }
            }
            int transformType = -1;
            if (climbWalls)
            {
                if (npc.type == ModContent.NPCType<AstralachneaGround>())
                {
                    transformType = ModContent.NPCType<AstralachneaWall>();
                }
                else
                {
                    switch (npc.type)
                    {
                        case NPCID.BlackRecluse:
                            transformType = NPCID.BlackRecluseWall;
                            break;
                        case NPCID.BloodCrawler:
                            transformType = NPCID.BloodCrawlerWall;
                            break;
                        case NPCID.DesertScorpionWalk:
                            transformType = NPCID.DesertScorpionWall;
                            break;
                        case NPCID.JungleCreeper:
                            transformType = NPCID.JungleCreeperWall;
                            break;
                        case NPCID.WallCreeper:
                            transformType = NPCID.WallCreeperWall;
                            break;
                    }
                }
                if (transformType != -1)
                {
                    npc.Transform(transformType);
                }
            }
        }

        public override bool AI(Mod mod)
        {
            int npcType = NPC.type;
            if (NPC.ModNPC != null)
            {
                if (NPC.ModNPC.AIType != NPCID.None)
                    npcType = NPC.ModNPC.AIType;
            }

            if (npcType == NPCID.Psycho)
            {
                BuffedPsychoAI(NPC);
            }

            if (npcType == NPCID.SwampThing)
            {
                BuffedSwampThingAI(NPC);
            }

            if (npcType == NPCID.CreatureFromTheDeep)
            {
                // Swimming
                if (NPC.wet)
                {
                    NPC.knockBackResist = 0f;
                    NPC.ai[3] = -0.10101f;
                    NPC.noGravity = true;
                    NPC.width = 34;
                    NPC.height = 24;
                    NPC.position = NPC.Center - NPC.Size / 2f;
                    NPC.TargetClosest(true);
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = -NPC.oldVelocity.X;
                    }
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.direction = -1;
                    }
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.direction = 1;
                    }

                    // If there's nothing in the way of the player, swim towards them
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.velocity = (NPC.velocity * 19f + NPC.SafeDirectionTo(Main.player[NPC.target].Center, -Vector2.UnitY) * 10f) / 20f;
                        return false;
                    }

                    float velocityMultiplier = 10f;
                    if (NPC.velocity.Y > 0f)
                    {
                        velocityMultiplier = 6f;
                    }
                    if (NPC.velocity.Y < 0f)
                    {
                        velocityMultiplier = 16f;
                    }
                    Vector2 directionVectorNormalized = new Vector2((float)NPC.direction, -1f);
                    directionVectorNormalized.Normalize();
                    directionVectorNormalized *= velocityMultiplier;

                    // If the speed is low, make the turn speed higher
                    if (velocityMultiplier < 5f)
                    {
                        NPC.velocity = (NPC.velocity * 24f + directionVectorNormalized) / 25f;
                        return false;
                    }
                    NPC.velocity = (NPC.velocity * 9f + directionVectorNormalized) / 10f;
                    return false;
                }
                else
                {
                    NPC.knockBackResist = CalamityWorld.death ? 0.1f : 0.2f;
                    NPC.noGravity = false;
                    NPC.width = 18;
                    NPC.height = 40;
                    NPC.position = NPC.Center - NPC.Size / 2f;

                    // If was just swimming, set values to return to land
                    if (NPC.ai[3] == -0.10101f)
                    {
                        NPC.ai[3] = 0f;
                        // Adjust velocity from the one the NPC had when swimming
                        float velocityMagnitude = NPC.velocity.Length();
                        velocityMagnitude *= 2f;
                        if (velocityMagnitude > 12f)
                        {
                            velocityMagnitude = 12f;
                        }
                        NPC.velocity.Normalize();
                        NPC.velocity *= velocityMagnitude;
                        NPC.direction = NPC.spriteDirection = (NPC.velocity.X > 0).ToDirectionInt();
                    }
                }
            }

            if (npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite)
            {
                // Pissed off
                if (NPC.ai[3] < 0f)
                {
                    NPC.damage = 0;
                    NPC.velocity.X *= 0.93f;
                    if (NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                    {
                        NPC.velocity.X = 0f;
                    }
                    int targetNPC = (int)(-NPC.ai[3] - 1f);
                    int directionToTarget = Math.Sign(Main.npc[targetNPC].Center.X - NPC.Center.X);
                    if (directionToTarget != NPC.direction)
                    {
                        NPC.velocity.X = 0f;
                        NPC.direction = directionToTarget;
                        NPC.netUpdate = true;
                    }
                    if (NPC.justHit && Main.netMode != NetmodeID.MultiplayerClient && Main.npc[targetNPC].localAI[0] == 0f)
                    {
                        Main.npc[targetNPC].localAI[0] = 1f;
                    }
                    if (NPC.ai[0] < 1000f)
                    {
                        NPC.ai[0] = 1000f;
                    }
                    NPC.ai[0] += 1f;
                    if (NPC.ai[0] >= 1300f)
                    {
                        NPC.ai[0] = 1000f;
                        NPC.netUpdate = true;
                    }
                    return false;
                }
                // Not pissed off
                if (NPC.ai[0] >= 1000f)
                {
                    NPC.ai[0] = 0f;
                }
                NPC.damage = NPC.defDamage;
            }

            if (npcType == NPCID.MartianOfficer && NPC.ai[2] == 0f && NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // If Martian Officer is ready to generate shield, generate it.
                int shield = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.ForceBubble, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.ai[2] = (float)(shield + 1);
                NPC.localAI[0] = -1f;
                NPC.netUpdate = true;
                Main.npc[shield].ai[0] = (float)NPC.whoAmI;
                Main.npc[shield].netUpdate = true;
            }

            if (npcType == NPCID.MartianOfficer)
            {
                int shield = (int)NPC.ai[2] - 1;
                if (shield != -1 && Main.npc[shield].active && Main.npc[shield].type == NPCID.ForceBubble)
                {
                    NPC.dontTakeDamage = true;
                }
                else
                {
                    NPC.dontTakeDamage = false;
                    NPC.ai[2] = 0f;
                    if (NPC.localAI[0] == -1f)
                    {
                        NPC.localAI[0] = CalamityWorld.death ? 60f : 120f;
                    }
                    if (NPC.localAI[0] > 0f)
                    {
                        NPC.localAI[0] -= 1f;
                    }
                }
            }

            if (npcType == NPCID.GraniteGolem)
            {
                int activeTime = 300;
                int defendTime = 120;

                // Set damage
                NPC.damage = NPC.defDamage;

                NPC.defense = NPC.defDefense;

                if (NPC.ai[2] < 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.defense = NPC.defDefense + 15;
                    NPC.ai[2] += 1f;
                    NPC.velocity.X *= 0.9f;
                    if (Math.Abs(NPC.velocity.X) < 0.001)
                    {
                        NPC.velocity.X = 0.001f * (float)NPC.direction;
                    }
                    if (Math.Abs(NPC.velocity.Y) > 1f)
                    {
                        NPC.ai[2] += 10f;
                    }
                    if (NPC.ai[2] >= 0f)
                    {
                        NPC.netUpdate = true;
                        NPC.velocity.X += (float)NPC.direction * 0.3f;
                    }
                    return false;
                }
                if (NPC.ai[2] < activeTime)
                {
                    if (NPC.justHit)
                    {
                        NPC.ai[2] += 15f;
                    }
                    NPC.ai[2] += 1f;
                }
                else if (NPC.velocity.Y == 0f)
                {
                    NPC.ai[2] = defendTime * -1f;
                    NPC.netUpdate = true;
                }
            }

            if (npcType == NPCID.Medusa)
            {
                int afterHitTime = 90;
                int afterWaitTime = 210;
                int maxTime = 270;
                int debuffTime = (CalamityWorld.death ? 4 : 2) * 60; // 2 seconds in Rev, 4 seconds in Death
                int turnToStoneTime = 20;
                float mesudaActiveDistance = CalamityWorld.death ? 1500f : 900f;
                float medusaEffectDistance = CalamityWorld.death ? 1600f : 1000f;
                if (NPC.ai[2] > 0f)
                {
                    NPC.ai[2] -= 1f;
                }
                else if (NPC.ai[2] == 0f)
                {
                    if (((Main.player[NPC.target].Center.X < NPC.Center.X && NPC.direction < 0) || (Main.player[NPC.target].Center.X > NPC.Center.X && NPC.direction > 0)) && NPC.velocity.Y == 0f && NPC.Distance(Main.player[NPC.target].Center) < mesudaActiveDistance && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[2] = (float)(maxTime * -1f - turnToStoneTime);
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (NPC.ai[2] < 0f && NPC.ai[2] < maxTime * -1f)
                    {
                        NPC.velocity.X *= 0.9f;
                        if (NPC.velocity.Y < -2f || NPC.velocity.Y > 4f || NPC.justHit)
                        {
                            NPC.ai[2] = afterHitTime;
                        }
                        else
                        {
                            NPC.ai[2] += 1f;
                            if (NPC.ai[2] == 0f)
                            {
                                NPC.ai[2] = afterWaitTime;
                            }
                        }
                        float time = NPC.ai[2] + maxTime + turnToStoneTime;
                        if (time == 1f)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath17, NPC.Center);
                        }
                        if (time < turnToStoneTime)
                        {
                            MedusaHeadDustEffect(NPC, time);
                        }
                        Lighting.AddLight(NPC.Center, 0.9f, 0.75f, 0.1f);
                        return false;
                    }
                    if (NPC.ai[2] < 0f && NPC.ai[2] >= maxTime * -1f)
                    {
                        Lighting.AddLight(NPC.Center, 0.9f, 0.75f, 0.1f);
                        NPC.velocity.X *= 0.9f;
                        if (NPC.velocity.Y < -2f || NPC.velocity.Y > 4f || NPC.justHit)
                        {
                            NPC.ai[2] = afterHitTime;
                        }
                        else
                        {
                            NPC.ai[2] += 1f;
                            if (NPC.ai[2] == 0f)
                            {
                                NPC.ai[2] = afterWaitTime;
                            }
                        }
                        float time = NPC.ai[2] + maxTime;
                        if (time < 180f && (Main.rand.NextBool(3) || NPC.ai[2] % 3f == 0f))
                        {
                            MedusaHeadDustEffect(NPC, time);
                        }
                        if (!Main.dedServ)
                        {
                            Player player = Main.LocalPlayer;
                            if (!player.dead && player.active && player.FindBuffIndex(BuffID.Stoned) == -1)
                            {
                                if (NPC.Distance(player.Center) < medusaEffectDistance)
                                {
                                    bool canTurnPlayerToStone = NPC.Distance(player.Center) < 30f;
                                    if (!canTurnPlayerToStone)
                                    {
                                        // Used to be "float x = 0.7853982f.ToRotationVector2().X;"
                                        // cos(pi/4) should do the job too, though. If you want/need to revert this to the
                                        // code above, do so.
                                        float x = (float)Math.Cos(MathHelper.PiOver4);
                                        Vector2 vector6 = Vector2.Normalize(player.Center - NPC.Center);
                                        if (vector6.X > x || vector6.X < -x)
                                        {
                                            canTurnPlayerToStone = true;
                                        }
                                    }
                                    if (((player.Center.X < NPC.Center.X && NPC.direction < 0 && player.direction > 0) || (player.Center.X > NPC.Center.X && NPC.direction > 0 && player.direction < 0)) && canTurnPlayerToStone && (Collision.CanHitLine(NPC.Center, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(NPC.Center - Vector2.UnitY * 16f, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(NPC.Center + Vector2.UnitY * 8f, 1, 1, player.Center, 1, 1)))
                                    {
                                        player.AddBuff(BuffID.Stoned, debuffTime + (int)NPC.ai[2] * -1, true);
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
            }

            if (npcType == NPCID.GoblinSummoner)
            {
                // Shit out minion things
                if (NPC.ai[3] < 0f)
                {
                    NPC.knockBackResist = 0f;
                    NPC.defense = (int)Math.Round(NPC.defDefense * 1.3);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.direction = (NPC.velocity.X > 0).ToDirectionInt();
                    NPC.rotation = NPC.velocity.X * 0.1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[3] += 1f;
                        if (NPC.localAI[3] > (float)Main.rand.Next(20, CalamityWorld.death ? 40 : 120))
                        {
                            NPC.localAI[3] = 0f;
                            Vector2 spawnPosition = NPC.Center;
                            spawnPosition += NPC.velocity;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.ChaosBall);
                        }
                    }
                }
                else
                {
                    NPC.localAI[3] = 0f;
                    NPC.knockBackResist = 0.2f;
                    NPC.rotation *= 0.9f;
                    NPC.defense = NPC.defDefense;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }
                if (NPC.ai[3] == 1f)
                {
                    NPC.knockBackResist = 0f;
                    NPC.defense += 10;
                }
                if (NPC.ai[3] == -1f)
                {
                    NPC.TargetClosest(true);
                    float velocityMultiplier = 10f;
                    float turnValue = 40f;
                    Vector2 targetDirection = Main.player[NPC.target].Center - NPC.Center;
                    float playerDistance = targetDirection.Length();
                    velocityMultiplier += playerDistance / 200f;
                    targetDirection.Normalize();
                    targetDirection *= velocityMultiplier;
                    NPC.velocity = (NPC.velocity * (turnValue - 1f) + targetDirection) / turnValue;
                    if (playerDistance < 500f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        // Go back to normal AI
                        NPC.ai[3] = 0f;
                        NPC.ai[2] = 0f;
                    }
                    return false;
                }
                // Go up on the Y axis and slow down on the X axis
                if (NPC.ai[3] == -2f)
                {
                    NPC.velocity.Y -= 0.2f;
                    if (NPC.velocity.Y < -12f)
                    {
                        NPC.velocity.Y = -12f;
                    }
                    if (Main.player[NPC.target].Center.Y - NPC.Center.Y > 200f)
                    {
                        NPC.TargetClosest(true);
                        NPC.ai[3] = -3f;
                        if (Main.player[NPC.target].Center.X > NPC.Center.X)
                        {
                            NPC.ai[2] = 1f;
                        }
                        else
                        {
                            NPC.ai[2] = -1f;
                        }
                    }
                    NPC.velocity.X *= 0.99f;
                    return false;
                }
                // Similar to above, but more quick
                if (NPC.ai[3] == -3f)
                {
                    if (NPC.direction == 0)
                    {
                        NPC.TargetClosest(true);
                    }
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = (float)NPC.direction;
                    }
                    NPC.velocity.Y *= 0.9f;
                    NPC.velocity.X += NPC.ai[2] * 0.3f;
                    if (NPC.velocity.X > 10f)
                    {
                        NPC.velocity.X = 10f;
                    }
                    if (NPC.velocity.X < -10f)
                    {
                        NPC.velocity.X = -10f;
                    }
                    float playerDistance = Main.player[NPC.target].Center.X - NPC.Center.X;
                    if ((NPC.ai[2] < 0f && playerDistance > 300f) || (NPC.ai[2] > 0f && playerDistance < -300f))
                    {
                        NPC.ai[3] = -4f;
                        NPC.ai[2] = 0f;
                        return false;
                    }
                    if (Math.Abs(Main.player[NPC.target].Center.X - NPC.Center.X) > 800f)
                    {
                        NPC.ai[3] = -1f;
                        NPC.ai[2] = 0f;
                    }
                    return false;
                }
                else
                {
                    if (NPC.ai[3] == -4f)
                    {
                        NPC.ai[2] += 1f;
                        NPC.velocity.Y += 0.1f;
                        // Don't go very fast
                        if (NPC.velocity.Length() > 4f)
                        {
                            NPC.velocity *= 0.9f;
                        }
                        int tileAtCenterX = (int)NPC.Center.X / 16;
                        int tileAtBottom = (int)(NPC.position.Y + (float)NPC.height + 12f) / 16;
                        bool ableToRestart = false;
                        for (int i = tileAtCenterX - 1; i <= tileAtCenterX + 1; i++)
                        {
                            if (Main.tile[i, tileAtBottom].HasTile && Main.tileSolid[(int)Main.tile[i, tileAtBottom].TileType])
                            {
                                ableToRestart = true;
                            }
                        }
                        if (ableToRestart && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.ai[3] = 0f;
                            NPC.ai[2] = 0f;
                        }
                        else if (NPC.ai[2] > 300f || NPC.Center.Y > Main.player[NPC.target].Center.Y + 200f)
                        {
                            NPC.ai[3] = -1f;
                            NPC.ai[2] = 0f;
                        }
                    }
                    // Barf out the shadowflame skull things
                    else
                    {
                        if (NPC.ai[3] == 1f)
                        {
                            Vector2 spawnPosiion = NPC.Center;
                            spawnPosiion.Y -= 70f;
                            NPC.velocity.X *= 0.8f;
                            NPC.ai[2] += 1f;
                            if (NPC.ai[2] == (CalamityWorld.death ? 15f : 30f))
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosiion.X, (int)spawnPosiion.Y + 18, NPCID.ShadowFlameApparition, 0, 0f, 0f, 0f, 0f, 255);
                                }
                            }
                            else if (NPC.ai[2] >= 90f)
                            {
                                NPC.ai[3] = -2f;
                                NPC.ai[2] = 0f;
                            }
                            for (int j = 0; j < 2; j++)
                            {
                                // Randomize and normalize. You know the drill
                                Vector2 dustVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                                dustVelocity *= (float)Main.rand.Next(0, 100) * 0.1f;
                                dustVelocity.Normalize();
                                dustVelocity *= (float)Main.rand.Next(50, 90) * 0.1f;
                                int dustIdx = Dust.NewDust(spawnPosiion, 1, 1, DustID.Shadowflame, 0f, 0f, 0, default, 1f);
                                Main.dust[dustIdx].velocity = -dustVelocity * 0.3f;
                                Main.dust[dustIdx].alpha = 100;
                                if (Main.rand.NextBool())
                                {
                                    Main.dust[dustIdx].noGravity = true;
                                    Main.dust[dustIdx].scale += 0.3f;
                                }
                            }
                            return false;
                        }
                        NPC.ai[2] += 1f;
                        int maxSkullCount = 10;
                        if (NPC.velocity.Y == 0f && NPC.CountNPCS(NPCID.ShadowFlameApparition) < maxSkullCount)
                        {
                            if (NPC.ai[2] >= 180f)
                            {
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 1f;
                            }
                        }
                        else
                        {
                            if (NPC.CountNPCS(NPCID.ShadowFlameApparition) >= maxSkullCount)
                            {
                                NPC.ai[2] += 1f;
                            }
                            if (NPC.ai[2] >= 360f)
                            {
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = -2f;
                                NPC.velocity.Y -= 3f;
                            }
                        }
                        if (NPC.target >= 0 && !Main.player[NPC.target].dead && (Main.player[NPC.target].Center - NPC.Center).Length() > 800f)
                        {
                            NPC.ai[3] = -1f;
                            NPC.ai[2] = 0f;
                        }
                    }
                    if (Main.player[NPC.target].dead)
                    {
                        NPC.TargetClosest(true);
                        if (Main.player[NPC.target].dead && NPC.timeLeft > 1)
                        {
                            NPC.timeLeft = 1;
                        }
                    }
                }
            }

            if (npcType == NPCID.SolarSolenian)
            {
                NPC.reflectsProjectiles = false;
                NPC.takenDamageMultiplier = 1f;
                int chargeTime = 6;
                int yFlyTime = 10;
                float velocityMultiplier = 20f;
                if (NPC.ai[2] > 0f)
                {
                    NPC.ai[2] -= 1f;
                }
                if (NPC.ai[2] == 0f)
                {
                    if (((Main.player[NPC.target].Center.X < NPC.Center.X && NPC.direction < 0) || (Main.player[NPC.target].Center.X > NPC.Center.X && NPC.direction > 0)) && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[2] = -1f;
                        NPC.netUpdate = true;
                        NPC.TargetClosest(true);
                    }
                }
                else
                {
                    if (NPC.ai[2] < 0f && NPC.ai[2] > chargeTime * -1f)
                    {
                        NPC.ai[2] -= 1f;
                        NPC.velocity.X *= 0.9f;
                        return false;
                    }
                    if (NPC.ai[2] == chargeTime * -1f)
                    {
                        NPC.ai[2] -= 1f;
                        NPC.TargetClosest(true);
                        Vector2 vectorToPlayer = NPC.SafeDirectionTo(Main.player[NPC.target].Top - Vector2.UnitY * 30f, Vector2.Normalize(new Vector2(NPC.spriteDirection, -1f)));

                        NPC.velocity = vectorToPlayer * velocityMultiplier;
                        NPC.netUpdate = true;
                        return false;
                    }
                    if (NPC.ai[2] < chargeTime * -1f)
                    {
                        NPC.ai[2] -= 1f;
                        if (NPC.velocity.Y == 0f)
                        {
                            NPC.ai[2] = CalamityWorld.death ? 60f : 90f;
                        }
                        else if (NPC.ai[2] < (float)(chargeTime * -1f - yFlyTime))
                        {
                            NPC.velocity.Y += 0.15f;
                            if (NPC.velocity.Y > 24f)
                            {
                                NPC.velocity.Y = 24f;
                            }
                        }
                        NPC.reflectsProjectiles = true;
                        NPC.takenDamageMultiplier = CalamityWorld.death ? 2f : 3f;
                        if (NPC.justHit)
                        {
                            NPC.ai[2] = CalamityWorld.death ? 60f : 90f;
                            NPC.netUpdate = true;
                        }
                        return false;
                    }
                }
            }

            if (npcType == NPCID.SolarDrakomire)
            {
                int timeToReset = 42;
                int timeToBreathFire = 18;
                if (NPC.justHit)
                {
                    NPC.ai[2] = CalamityWorld.death ? 30f : 60f;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[2] > 0f)
                {
                    NPC.ai[2] -= 1f;
                }
                if (NPC.ai[2] == 0f)
                {
                    int solarFlareCount = 0;
                    int maxFlareCount = 6;
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (Main.npc[k].active && Main.npc[k].type == NPCID.SolarFlare)
                        {
                            solarFlareCount++;
                        }
                    }
                    if (solarFlareCount > maxFlareCount)
                    {
                        NPC.ai[2] = CalamityWorld.death ? 30f : 60f;
                    }
                    else if (((Main.player[NPC.target].Center.X < NPC.Center.X && NPC.direction < 0) || (Main.player[NPC.target].Center.X > NPC.Center.X && NPC.direction > 0)) && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[2] = -1f;
                        NPC.netUpdate = true;
                        NPC.TargetClosest(true);
                    }
                }
                else if (NPC.ai[2] < 0f && NPC.ai[2] > (float)(-(float)timeToReset))
                {
                    NPC.ai[2] -= 1f;
                    if (NPC.ai[2] == (float)(-(float)timeToReset))
                    {
                        NPC.ai[2] = (float)(180 + 10 * Main.rand.Next(10));
                    }
                    NPC.velocity.X *= 0.8f;
                    if (NPC.ai[2] == (float)(-(float)timeToBreathFire) || NPC.ai[2] == (float)(-(float)timeToBreathFire - 8) || NPC.ai[2] == (float)(-(float)timeToBreathFire - 16))
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            Vector2 spawnPosition = NPC.Center + Vector2.UnitX * (float)NPC.spriteDirection * 40f;
                            Dust dust = Main.dust[Dust.NewDust(spawnPosition, 0, 0, DustID.SolarFlare, 0f, 0f, 0, default, 1f)];
                            Vector2 velocity = Vector2.UnitY.RotatedByRandom(Math.PI * 2.0);
                            dust.position = spawnPosition + velocity * 4f;
                            dust.velocity = velocity * 2f + Vector2.UnitX * Main.rand.NextFloat() * (float)NPC.spriteDirection * 3f;
                            dust.scale = 0.3f + velocity.X * (float)(-(float)NPC.spriteDirection);
                            dust.fadeIn = 0.7f;
                            dust.noGravity = true;
                        }
                        if (NPC.velocity.X > -0.5f && NPC.velocity.X < 0.5f)
                        {
                            NPC.velocity.X = 0f;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + NPC.spriteDirection * 45, (int)NPC.Center.Y + 8, NPCID.SolarFlare, 0, 0f, 0f, 0f, 0f, NPC.target);
                        }
                    }
                    return false;
                }
            }

            if (npcType == NPCID.VortexLarva)
            {
                if (NPC.CountNPCS(NPCID.VortexHornet) < 6)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= (CalamityWorld.death ? 90f : CalamityWorld.revenge ? 150f : 300f))
                    {
                        int centerTileX = (int)NPC.Center.X / 16 - 1;
                        int centerTileY = (int)NPC.Center.Y / 16 - 1;
                        if (!Collision.SolidTiles(centerTileX, centerTileX + 2, centerTileY, centerTileY + 1) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.Transform(NPCID.VortexHornet);
                            NPC.life = NPC.lifeMax;
                            NPC.localAI[0] = 0f;
                            return false;
                        }
                    }
                    int maxValue;
                    if (NPC.localAI[0] < (CalamityWorld.revenge ? 30f : 60f))
                    {
                        maxValue = 16;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 45f : CalamityWorld.revenge ? 60f : 120f))
                    {
                        maxValue = 8;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 60f : CalamityWorld.revenge ? 90f : 180f))
                    {
                        maxValue = 4;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 75f : CalamityWorld.revenge ? 120f : 240f))
                    {
                        maxValue = 2;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 90f : CalamityWorld.revenge ? 150f : 300f))
                    {
                        maxValue = 1;
                    }
                    else
                    {
                        maxValue = 1;
                    }
                    if (Main.rand.NextBool(maxValue))
                    {
                        Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1f)];
                        dust.noGravity = true;
                        dust.scale = 1f;
                        dust.noLight = true;
                        dust.velocity = NPC.DirectionFrom(dust.position) * dust.velocity.Length();
                        dust.position -= dust.velocity * 5f;
                        dust.position.X += (float)(NPC.direction * 6);
                        dust.position.Y += 4f;
                    }
                }
            }

            if (npcType == NPCID.VortexHornet)
            {
                if (NPC.CountNPCS(NPCID.VortexHornetQueen) < 3)
                {
                    NPC.localAI[0] += 1f;
                    NPC.localAI[0] += Math.Abs(NPC.velocity.X) / 2f;
                    if (NPC.localAI[0] >= (CalamityWorld.death ? 300f : CalamityWorld.revenge ? 600f : 1200f) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int centerTileX = (int)NPC.Center.X / 16 - 2;
                        int centerTileY = (int)NPC.Center.Y / 16 - 3;
                        if (!Collision.SolidTiles(centerTileX, centerTileX + 4, centerTileY, centerTileY + 4))
                        {
                            NPC.Transform(NPCID.VortexHornetQueen);
                            NPC.life = NPC.lifeMax;
                            NPC.localAI[0] = 0f;
                            return false;
                        }
                    }
                    int maxValue2;
                    if (NPC.localAI[0] < (CalamityWorld.death ? 60f : CalamityWorld.revenge ? 120f : 240f))
                    {
                        maxValue2 = 32;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 120f : CalamityWorld.revenge ? 240f : 480f))
                    {
                        maxValue2 = 16;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 180f : CalamityWorld.revenge ? 360f : 720f))
                    {
                        maxValue2 = 6;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 240f : CalamityWorld.revenge ? 480f : 960f))
                    {
                        maxValue2 = 2;
                    }
                    else if (NPC.localAI[0] < (CalamityWorld.death ? 300f : CalamityWorld.revenge ? 600f : 1200f))
                    {
                        maxValue2 = 1;
                    }
                    else
                    {
                        maxValue2 = 1;
                    }
                    if (Main.rand.NextBool(maxValue2))
                    {
                        Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1f)];
                        dust.noGravity = true;
                        dust.scale = 1f;
                        dust.noLight = true;
                    }
                }
            }

            bool jump = false;
            if (NPC.velocity.X == 0f)
            {
                jump = true;
            }
            if (NPC.justHit)
            {
                jump = false;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npcType == NPCID.Lihzahrd && (double)NPC.life <= (double)NPC.lifeMax * 0.9)
            {
                NPC.Transform(NPCID.LihzahrdCrawler);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npcType == NPCID.Nutcracker && (double)NPC.life <= (double)NPC.lifeMax * 0.9)
            {
                NPC.Transform(NPCID.NutcrackerSpinning);
            }

            // This variable seems to have a lot of purposes.
            // I wasn't sure what I could name it that isn't very vague
            int aiGateValue = 60;
            if (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>())
            {
                aiGateValue = 180;
                if (NPC.ai[3] == -30f)
                {
                    NPC.velocity *= 0f;
                    NPC.ai[3] = 0f;
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    float distX = NPC.oldPos[2].X + (float)NPC.width * 0.5f - NPC.Center.X;
                    float distY = NPC.oldPos[2].Y + (float)NPC.height * 0.5f - NPC.Center.Y;
                    float distance = (float)Math.Sqrt((distX * distX + distY * distY));
                    distance = 2f / distance;
                    distX *= distance;
                    distY *= distance;
                    for (int m = 0; m < 20; m++)
                    {
                        int dustIdx = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UndergroundHallowedEnemies, distX, distY, 200, default, 2f);
                        Main.dust[dustIdx].noGravity = true;
                        Dust dust = Main.dust[dustIdx];
                        dust.velocity.X *= 2f;
                    }
                    for (int n = 0; n < 20; n++)
                    {
                        int dustIdx = Dust.NewDust(NPC.oldPos[2], NPC.width, NPC.height, DustID.UndergroundHallowedEnemies, -distX, -distY, 200, default, 2f);
                        Main.dust[dustIdx].noGravity = true;
                        Dust dust = Main.dust[dustIdx];
                        dust.velocity.X *= 2f;
                    }
                }
            }

            bool canIncrementAI3 = false;
            bool reset = true;
            if (npcType == NPCID.Yeti || npcType == NPCID.CorruptBunny || npcType == NPCID.Crab || npcType == NPCID.Clown || npcType == NPCID.SkeletonArcher || npcType == NPCID.GoblinArcher || npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()
                || npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.CorruptPenguin || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.IcyMerman || npcType == NPCID.PirateDeadeye
                || npcType == NPCID.PirateCrossbower || npcType == NPCID.PirateCaptain || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.SeaSnail || npcType == NPCID.FlyingSnake
                || npcType == NPCID.IceGolem || npcType == NPCID.Eyezor || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.Paladin || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton
                || npcType == NPCID.SkeletonCommando || npcType == NPCID.Scarecrow1 || npcType == NPCID.Scarecrow2 || npcType == NPCID.Scarecrow3 || npcType == NPCID.Scarecrow4 || npcType == NPCID.Scarecrow5 || npcType == NPCID.Nutcracker
                || npcType == NPCID.NutcrackerSpinning || npcType == NPCID.ElfArcher || npcType == NPCID.Krampus || npcType == NPCID.CultistArcherBlue || (npcType >= 430 && npcType <= 436)
                || (npcType == NPCID.CultistArcherWhite || npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianOfficer || npcType == NPCID.MartianEngineer || npcType == NPCID.Scutlix
                || (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4)) || (npcType == NPCID.Psycho || npcType == NPCID.CrimsonBunny || npcType == NPCID.SwampThing || npcType == NPCID.ThePossessed || npcType == NPCID.DrManFly
                || npcType == NPCID.GoblinSummoner || npcType == NPCID.CrimsonPenguin || npcType == NPCID.Medusa || npcType == NPCID.GreekSkeleton || npcType == NPCID.GraniteGolem || npcType == NPCID.StardustSoldier || npcType == NPCID.NebulaSoldier
                || npcType == NPCID.StardustSpiderBig || (npcType >= 494 && npcType <= 506)) || (npcType == NPCID.VortexRifleman || npcType == NPCID.VortexHornet || npcType == NPCID.VortexHornetQueen || npcType == NPCID.VortexLarva
                || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || npcType == NPCID.MartianWalker || (npcType >= 524 && npcType <= 527)) || npcType == NPCID.DesertLamiaLight
                || npcType == NPCID.DesertLamiaDark || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast)
            {
                reset = false;
            }

            bool ableToAlterAI3 = false;
            if (npcType == NPCID.VortexRifleman || npcType == NPCID.GoblinSummoner)
            {
                ableToAlterAI3 = true;
            }

            bool npcTimer = NPC.ai[2] <= 0f;
            if (npcType <= NPCID.RayGunner)
            {
                if (npcType <= NPCID.PirateCaptain)
                {
                    if (npcType - 110 > 1 && npcType != NPCID.IcyMerman && npcType - NPCID.PirateDeadeye > 2)
                    {
                        goto PrepareToShoot;
                    }
                }
                else if (npcType - NPCID.SkeletonSniper > 2 && npcType != NPCID.ElfArcher && npcType - NPCID.CultistArcherBlue > 3)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType <= NPCID.NebulaSoldier)
            {
                if (npcType != NPCID.StardustSpiderSmall && npcType != NPCID.StardustSoldier && npcType != NPCID.NebulaSoldier)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType <= NPCID.Psycho)
            {
                if (npcType != NPCID.VortexHornetQueen && npcType != NPCID.Psycho)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType - 498 > 8 && npcType != NPCID.MartianWalker)
            {
                goto PrepareToShoot;
            }

// If anyone can give me an explanation of the real noticable differences between
//& and && (same with | and ||) with booleans,
// I'd greatly appreciate it
PrepareToShoot:
            if (!ableToAlterAI3 & npcTimer)
            {
                if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
                {
                    canIncrementAI3 = true;
                }
                if ((NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= (float)aiGateValue) | canIncrementAI3)
                {
                    NPC.ai[3] += 1f;
                }
                else if (Math.Abs(NPC.velocity.X) > 0.9 && NPC.ai[3] > 0f)
                {
                    NPC.ai[3] -= 1f;
                }
                if (NPC.ai[3] > (float)(aiGateValue * 10))
                {
                    NPC.ai[3] = 0f;
                }
                if (NPC.justHit)
                {
                    NPC.ai[3] = 0f;
                }
                if (NPC.ai[3] == (float)aiGateValue)
                {
                    NPC.netUpdate = true;
                }
            }

            if (npcType == NPCID.Nailhead && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.localAI[3] > 0f)
                    NPC.localAI[3] -= 1f;

                if (NPC.justHit && NPC.localAI[3] <= 0f)
                {
                    NPC.localAI[3] = CalamityWorld.death ? 45f : CalamityWorld.revenge ? 60f : 75f;

                    float nailVelocity = CalamityWorld.death ? 12f : CalamityWorld.revenge ? 10f : 8f;
                    int type = ProjectileID.Nail;
                    int damage = (int)(NPC.damage * 0.15);
                    Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                    destination = destination.SafeNormalize(-Vector2.UnitY);
                    destination *= nailVelocity;
                    int numProj = Main.rand.Next(3, 6);
                    float rotation = MathHelper.ToRadians(numProj * 15);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - Vector2.UnitY * (NPC.width / 4), perturbedSpeed, type, damage, 1f, Main.maxPlayers);
                    }
                }
            }

            if (npcType == NPCID.Butcher)
            {
                if (NPC.velocity.Y < -0.3f || NPC.velocity.Y > 0.3f)
                {
                    NPC.knockBackResist = 0f;
                }
                else
                {
                    NPC.knockBackResist = 0.1f;
                }
            }

            if (npcType == NPCID.ThePossessed)
            {
                NPC.knockBackResist = 0.25f;
                if (NPC.ai[2] == 1f)
                {
                    NPC.knockBackResist = 0f;
                }
                bool spiderAI = false;
                int centerTileX = (int)NPC.Center.X / 16;
                int centerTileY = (int)NPC.Center.Y / 16;
                for (int x = centerTileX - 1; x <= centerTileX + 1; x++)
                {
                    for (int y = centerTileY - 1; y <= centerTileY + 1; y++)
                    {
                        if (Main.tile[x, y] != null && Main.tile[x, y].WallType > WallID.None)
                        {
                            spiderAI = true;
                            break;
                        }
                    }
                    if (spiderAI)
                    {
                        break;
                    }
                }
                if (NPC.ai[2] == 0f & spiderAI)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.Y = -4.6f;
                        NPC.velocity.X *= 1.5f;
                    }
                    else if (NPC.velocity.Y > 0f)
                    {
                        NPC.ai[2] = 1f;
                    }
                }
                if (spiderAI && NPC.ai[2] == 1f && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                {
                    Vector2 distanceVector = Main.player[NPC.target].Center - NPC.Center;
                    float distanceMagnitude = distanceVector.Length();
                    distanceVector = distanceVector.SafeNormalize(-Vector2.UnitY);
                    distanceVector *= 6f + distanceMagnitude / 300f;
                    NPC.velocity = (NPC.velocity * 29f + distanceVector) / 30f;
                    NPC.noGravity = true;
                    NPC.ai[2] = 1f;
                    return false;
                }
                NPC.noGravity = false;
                NPC.ai[2] = 0f;
            }

            if (npcType == NPCID.Fritz && NPC.velocity.Y == 0f && (Main.player[NPC.target].Center - NPC.Center).Length() < 150f && Math.Abs(NPC.velocity.X) > 3f && ((NPC.velocity.X < 0f && NPC.Center.X > Main.player[NPC.target].Center.X) || (NPC.velocity.X > 0f && NPC.Center.X < Main.player[NPC.target].Center.X)))
            {
                NPC.velocity.X *= 2f;
                NPC.velocity.Y -= 4.5f;
                if (NPC.Center.Y - Main.player[NPC.target].Center.Y > 20f)
                {
                    NPC.velocity.Y -= 0.5f;
                }
                if (NPC.Center.Y - Main.player[NPC.target].Center.Y > 40f)
                {
                    NPC.velocity.Y -= 1f;
                }
                if (NPC.Center.Y - Main.player[NPC.target].Center.Y > 80f)
                {
                    NPC.velocity.Y -= 1.5f;
                }
                if (NPC.Center.Y - Main.player[NPC.target].Center.Y > 100f)
                {
                    NPC.velocity.Y -= 1.5f;
                }
                if (Math.Abs(NPC.velocity.X) > 9f)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = -9f;
                    }
                    else
                    {
                        NPC.velocity.X = 9f;
                    }
                }
            }

            if (NPC.ai[3] < (float)aiGateValue && (Main.eclipse || !Main.dayTime || (double)NPC.position.Y > Main.worldSurface * 16.0 || (Main.invasionType == InvasionID.GoblinArmy && (npcType == NPCID.Yeti || npcType == NPCID.ElfArcher)) || (Main.invasionType == InvasionID.GoblinArmy && (npcType == NPCID.GoblinPeon || npcType == NPCID.GoblinThief || npcType == NPCID.GoblinWarrior || npcType == NPCID.GoblinArcher || npcType == NPCID.GoblinSummoner)) || (npcType == NPCID.GoblinScout || (Main.invasionType == InvasionID.PirateInvasion && npcType >= 212 && npcType <= 216)) || (Main.invasionType == InvasionID.MartianMadness && (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianOfficer || npcType == NPCID.GrayGrunt || npcType == NPCID.MartianEngineer || npcType == NPCID.GigaZapper || npcType == NPCID.Scutlix || npcType == NPCID.MartianWalker)) || (npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.Crab || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy || npcType == NPCID.SkeletonArcher || npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>() || npcType == NPCID.CorruptPenguin || npcType == NPCID.FaceMonster || npcType == NPCID.SnowFlinx || npcType == NPCID.Lihzahrd || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.IcyMerman || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.SeaSnail || npcType == NPCID.BloodCrawler || npcType == NPCID.IceGolem || npcType == NPCID.ZombieMushroom || npcType == NPCID.ZombieMushroomHat || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton || npcType == NPCID.SkeletonCommando || npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite || npcType == NPCID.CrimsonBunny || npcType == NPCID.CrimsonPenguin || npcType == NPCID.NebulaSoldier || (npcType == NPCID.StardustSoldier && (NPC.ai[1] >= 180f || NPC.ai[1] < 90f))) || (npcType == NPCID.StardustSpiderBig || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexSoldier || npcType == NPCID.VortexHornet || npcType == NPCID.VortexLarva || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || (npcType >= 524 && npcType <= 527)) || npcType == NPCID.DesertLamiaLight || npcType == NPCID.DesertLamiaDark || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast))
            {
                if ((npcType == NPCID.Zombie || npcType == NPCID.ZombieXmas || npcType == NPCID.ZombieSweater || npcType == NPCID.Skeleton || (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4) || npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.SkeletonArcher || npcType == NPCID.BaldZombie || npcType == NPCID.UndeadViking || npcType == NPCID.ZombieEskimo || npcType == NPCID.Frankenstein || npcType == NPCID.PincushionZombie || npcType == NPCID.SlimedZombie || npcType == NPCID.SwampZombie || npcType == NPCID.TwiggyZombie || npcType == NPCID.ArmoredViking || npcType == NPCID.FemaleZombie || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.ZombieRaincoat || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton || npcType == NPCID.SkeletonCommando || npcType == NPCID.ZombieSuperman || npcType == NPCID.ZombiePixie || npcType == NPCID.ZombieDoctor || npcType == NPCID.GreekSkeleton || npcType == ModContent.NPCType<BucketZombie>()) && Main.rand.NextBool(1000))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, NPC.Center);
                }
                if (npcType == NPCID.BloodZombie && Main.rand.NextBool(800))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, NPC.Center); //There was a npcType thing afterwards but its not really useable now. Hilarious, frankly
                }
                if ((npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy) && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Mummy, NPC.Center);
                }
                if (npcType == NPCID.Vampire && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);
                }
                if (npcType == NPCID.Frankenstein && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie6, NPC.Center);
                }
                if (npcType == NPCID.FaceMonster && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie8, NPC.Center);
                }
                if (npcType >= 269 && npcType <= 280 && Main.rand.NextBool(1000))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, NPC.Center);
                }
                NPC.TargetClosest(true);
            }
            else if (NPC.ai[2] <= 0f ||
                (npcType != NPCID.SkeletonArcher &&
                npcType != NPCID.GoblinArcher &&
                npcType != NPCID.IcyMerman &&
                npcType != NPCID.PirateDeadeye &&
                npcType != NPCID.PirateCrossbower &&
                npcType != NPCID.PirateCaptain &&
                npcType != NPCID.SkeletonSniper &&
                npcType != NPCID.TacticalSkeleton &&
                npcType != NPCID.SkeletonCommando &&
                npcType != NPCID.ElfArcher &&
                npcType != NPCID.BrainScrambler &&
                npcType != NPCID.RayGunner &&
                npcType != NPCID.MartianOfficer &&
                npcType != NPCID.GrayGrunt &&
                npcType != NPCID.MartianEngineer &&
                npcType != NPCID.GigaZapper &&
                npcType != NPCID.Scutlix &&
                npcType != NPCID.ThePossessed &&
                npcType != NPCID.SwampThing &&
                npcType != NPCID.Psycho &&
                npcType != NPCID.GoblinSummoner &&
                npcType != NPCID.StardustSoldier &&
                npcType != NPCID.StardustSpiderSmall &&
                npcType != NPCID.NebulaSoldier &&
                npcType != NPCID.VortexRifleman &&
                npcType != NPCID.VortexHornetQueen &&
                npcType != NPCID.SolarDrakomire &&
                npcType != NPCID.SolarSolenian &&
                npcType != NPCID.MartianWalker))
            {
                if (Main.dayTime && (double)(NPC.position.Y / 16f) < Main.worldSurface && NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                if (NPC.velocity.X == 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] += 1f;
                        if (NPC.ai[0] >= 2f)
                        {
                            NPC.direction *= -1;

                            NPC.spriteDirection = NPC.direction;

                            NPC.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    NPC.ai[0] = 0f;
                }
                if (NPC.direction == 0)
                {
                    NPC.direction = 1;
                }
            }

            if (npcType == NPCID.Vampire || npcType == NPCID.NutcrackerSpinning)
            {
                if (npcType == NPCID.Vampire && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
                {
                    NPC.velocity.X *= 0.95f;
                }
                if (NPC.velocity.X < -8f || NPC.velocity.X > 8f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity *= 0.8f;
                    }
                }
                else if (NPC.velocity.X < 8f && NPC.direction == 1)
                {
                    if (NPC.velocity.Y == 0f && NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X *= 0.99f;
                    }
                    NPC.velocity.X += 0.09f;
                    if (NPC.velocity.X > 8f)
                    {
                        NPC.velocity.X = 8f;
                    }
                }
                else if (NPC.velocity.X > -8f && NPC.direction == -1)
                {
                    if (NPC.velocity.Y == 0f && NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X *= 0.99f;
                    }
                    NPC.velocity.X -= 0.09f;
                    if (NPC.velocity.X < -8f)
                    {
                        NPC.velocity.X = -8f;
                    }
                }
            }
            else if (npcType == NPCID.LihzahrdCrawler)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 8f : 6f, 0.12f, 0.8f, true, 0.8f);
            }
            else if (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>() || npcType == NPCID.SwampThing || npcType == NPCID.PirateCorsair || npcType == NPCID.MushiLadybug || npcType == NPCID.DesertLamiaLight || npcType == NPCID.DesertLamiaDark)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 6f : 4f, 0.09f, 0.8f, true, 0.8f);
            }
            else if (npcType == NPCID.CreatureFromTheDeep || npcType == NPCID.GoblinThief || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Werewolf || npcType == NPCID.BlackRecluse || npcType == NPCID.Frankenstein || npcType == NPCID.Nymph || npcType == NPCID.ArmoredViking || npcType == NPCID.PirateDeckhand || npcType == NPCID.AnomuraFungus || npcType == NPCID.Splinterling || npcType == NPCID.Yeti || npcType == NPCID.Nutcracker || npcType == NPCID.Krampus || (npcType >= 524 && npcType <= 527) || npcType == NPCID.DesertScorpionWalk)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 3f : 2.5f, 0.09f, 0.8f);
            }
            else if (npcType == NPCID.Clown)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 5f : 4f, 0.06f, 0.8f);
            }
            else if (npcType == NPCID.Skeleton || npcType == NPCID.SporeSkeleton || npcType == NPCID.GoblinPeon || npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.GoblinScout || npcType == NPCID.PossessedArmor || npcType == NPCID.WallCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.UndeadViking || npcType == NPCID.CorruptPenguin || npcType == NPCID.SnowFlinx || npcType == NPCID.Lihzahrd || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.FlyingSnake || npcType == NPCID.FaceMonster || npcType == NPCID.ZombieMushroom || npcType == NPCID.ZombieElf || npcType == NPCID.ZombieElfBeard || npcType == NPCID.ZombieElfGirl || npcType == NPCID.GingerbreadMan || npcType == NPCID.GrayGrunt || npcType == NPCID.GigaZapper || npcType == NPCID.Fritz || npcType == NPCID.Nailhead || npcType == NPCID.Psycho || npcType == NPCID.CrimsonBunny || npcType == NPCID.ThePossessed || npcType == NPCID.CrimsonPenguin || npcType == NPCID.Medusa || npcType == NPCID.GraniteGolem || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexSoldier)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.AngryBonesBig)
                {
                    maxVelocity = 2f;
                }
                else if (npcType == NPCID.AngryBonesBigMuscle)
                {
                    maxVelocity = 1.75f;
                }
                else if (npcType == NPCID.AngryBonesBigHelmet)
                {
                    maxVelocity = 1.25f;
                }
                else if (npcType == NPCID.HeadacheSkeleton)
                {
                    maxVelocity = 1.1f;
                }
                else if (npcType == NPCID.MisassembledSkeleton)
                {
                    maxVelocity = 0.9f;
                }
                else if (npcType == NPCID.PantlessSkeleton)
                {
                    maxVelocity = 1.2f;
                }
                else if (npcType == NPCID.ZombieElf)
                {
                    maxVelocity = 1.75f;
                }
                else if (npcType == NPCID.ZombieElfBeard)
                {
                    maxVelocity = 1.25f;
                }
                else if (npcType == NPCID.ZombieElfGirl)
                {
                    maxVelocity = 2f;
                }
                else if (npcType == NPCID.GrayGrunt)
                {
                    maxVelocity = 1.8f;
                }
                else if (npcType == NPCID.GigaZapper)
                {
                    maxVelocity = 2.25f;
                }
                else if (npcType == NPCID.Fritz)
                {
                    maxVelocity = 4f;
                }
                else if (npcType == NPCID.Nailhead)
                {
                    maxVelocity = CalamityWorld.revenge ? 0.75f : 0.6f;
                }
                else if (npcType == NPCID.Psycho)
                {
                    maxVelocity = 3.75f;
                }
                else if (npcType == NPCID.ThePossessed)
                {
                    maxVelocity = 3.25f;
                }
                else if (npcType == NPCID.Medusa)
                {
                    maxVelocity = 1.5f + (1f - (float)NPC.life / (float)NPC.lifeMax) * 2f;
                }
                else if (npcType == NPCID.VortexRifleman)
                {
                    maxVelocity = CalamityWorld.revenge ? 6f : 4.8f;
                }
                else if (npcType == NPCID.VortexSoldier)
                {
                    maxVelocity = 4f;
                }
                if (npcType == NPCID.Skeleton || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.GingerbreadMan)
                {
                    maxVelocity *= 1f + (1f - NPC.scale);
                }
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                bool extraSlowdown = NPC.velocity.Y == 0f && npcType == NPCID.Fritz && ((NPC.direction > 0 && NPC.velocity.X < 0f) || (NPC.direction < 0 && NPC.velocity.X > 0f));
                FighterRunningAI(NPC, maxVelocity, 0.09f, 0.9f, extraSlowdown, 0.9f);
            }
            else if (npcType >= NPCID.RustyArmoredBonesAxe && npcType <= NPCID.HellArmoredBonesSword)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.RustyArmoredBonesAxe)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.RustyArmoredBonesFlail)
                {
                    maxVelocity = 1f;
                }
                if (npcType == NPCID.RustyArmoredBonesSword)
                {
                    maxVelocity = 1.5f;
                }
                if (npcType == NPCID.RustyArmoredBonesSwordNoArmor)
                {
                    maxVelocity = 3f;
                }
                if (npcType == NPCID.BlueArmoredBones)
                {
                    maxVelocity = 1.25f;
                }
                if (npcType == NPCID.BlueArmoredBonesMace)
                {
                    maxVelocity = 3f;
                }
                if (npcType == NPCID.BlueArmoredBonesNoPants)
                {
                    maxVelocity = 3.25f;
                }
                if (npcType == NPCID.BlueArmoredBonesSword)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.HellArmoredBones)
                {
                    maxVelocity = 2.75f;
                }
                if (npcType == NPCID.HellArmoredBonesSpikeShield)
                {
                    maxVelocity = 1.8f;
                }
                if (npcType == NPCID.HellArmoredBonesMace)
                {
                    maxVelocity = 1.3f;
                }
                if (npcType == NPCID.HellArmoredBonesSword)
                {
                    maxVelocity = 2.5f;
                }
                maxVelocity *= 1f + (1f - NPC.scale);
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                FighterRunningAI(NPC, maxVelocity, 0.09f, 0.8f, false);
            }
            else if (npcType >= 305 && npcType <= 314)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.Scarecrow1 || npcType == NPCID.Scarecrow6)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.Scarecrow2 || npcType == NPCID.Scarecrow7)
                {
                    maxVelocity = 1.25f;
                }
                if (npcType == NPCID.Scarecrow3 || npcType == NPCID.Scarecrow8)
                {
                    maxVelocity = 2.25f;
                }
                if (npcType == NPCID.Scarecrow4 || npcType == NPCID.Scarecrow9)
                {
                    maxVelocity = 1.5f;
                }
                if (npcType == NPCID.Scarecrow5 || npcType == NPCID.Scarecrow10)
                {
                    maxVelocity = 1f;
                }
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                if (npcType < 310) //Pogo stick Scarecrows
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.X *= 0.85f;
                        if (NPC.velocity.X > -0.3f && NPC.velocity.X < 0.3f)
                        {
                            NPC.velocity.Y = -9f; //-7f normally
                            NPC.velocity.X = maxVelocity * (float)NPC.direction;
                        }
                    }
                    else if (NPC.spriteDirection == NPC.direction)
                    {
                        NPC.velocity.X = (NPC.velocity.X * 10f + maxVelocity * NPC.direction) / 11f;
                    }
                }
                else
                {
                    FighterRunningAI(NPC, maxVelocity, 0.09f, 0.8f, false);
                }
            }
            else if (npcType == NPCID.Crab || npcType == NPCID.SeaSnail || npcType == NPCID.VortexLarva)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 4f : CalamityWorld.revenge ? 1f : 0.5f, CalamityWorld.revenge ? 0.06f : 0.05f, 0.7f);
            }
            else if (npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy)
            {
                float maxVelocity = 3f;
                float acceleration = 0.15f;
                if (npcType == NPCID.DarkMummy)
                {
                    maxVelocity *= 1.5f;
                }
                if (CalamityWorld.death)
                {
                    maxVelocity *= 1.25f;
                }
                FighterRunningAI(NPC, maxVelocity, acceleration, 0.7f);
            }
            else if (npcType == NPCID.BoneLee)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 7f : CalamityWorld.revenge ? 6f : 5f, CalamityWorld.revenge ? 0.3f : 0.2f, 0.7f);
            }
            else if (npcType == NPCID.IceGolem)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 4.5f : CalamityWorld.revenge ? 3f : 2f, CalamityWorld.revenge ? 0.3f : 0.2f, 0.7f);
            }
            else if (npcType == NPCID.Eyezor)
            {
                FighterRunningAI(NPC, CalamityWorld.death ? 5f : CalamityWorld.revenge ? 3.5f : 2.5f, CalamityWorld.revenge ? 0.3f : 0.2f, 0.8f);
            }
            else if (npcType == NPCID.MartianEngineer)
            {
                if (NPC.ai[2] > 0f)
                {
                    if (NPC.velocity.Y == 0f)
                        NPC.velocity.X *= 0.8f;
                }
                else
                    FighterRunningAI(NPC, CalamityWorld.death ? 5f : 3.5f, 0.2f, 0.8f);
            }
            else if (npcType == NPCID.Butcher)
            {
                float acceleration = 0.2f;
                if (Math.Abs(NPC.velocity.X) > 2f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 2.5)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 3f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 3.5)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 4f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 4.5f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 5f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(NPC.velocity.X) > 5.5)
                {
                    acceleration *= 0.8f;
                }
                FighterRunningAI(NPC, CalamityWorld.death ? 10f : 7f, acceleration, 0.8f);
            }
            else if (npcType == NPCID.GiantWalkingAntlion || npcType == NPCID.WalkingAntlion || npcType == NPCID.LarvaeAntlion)
            {
                float xAdditive = CalamityWorld.death ? 3.5f : 3f;
                float turnValue = 90f;
                float absoluteVelocityX = Math.Abs(NPC.velocity.X);
                if (absoluteVelocityX > 2.75f)
                {
                    xAdditive = CalamityWorld.death ? 7f : 6f;
                    turnValue += 100f;
                }
                else if (absoluteVelocityX > 2.25)
                {
                    xAdditive = CalamityWorld.death ? 5f : 4.25f;
                    turnValue += 80f;
                }
                if (Math.Abs(NPC.velocity.Y) < 0.5)
                {
                    if (NPC.velocity.X > 0f && NPC.direction < 0)
                    {
                        NPC.velocity *= 0.9f;
                    }
                    if (NPC.velocity.X < 0f && NPC.direction > 0)
                    {
                        NPC.velocity *= 0.9f;
                    }
                }
                if (Math.Abs(NPC.velocity.Y) > 0.3f)
                {
                    turnValue *= 3f;
                }
                if (NPC.velocity.X <= 0f && NPC.direction < 0)
                {
                    NPC.velocity.X = (NPC.velocity.X * turnValue - xAdditive) / (turnValue + 1f);
                }
                else if (NPC.velocity.X >= 0f && NPC.direction > 0)
                {
                    NPC.velocity.X = (NPC.velocity.X * turnValue + xAdditive) / (turnValue + 1f);
                }
                else if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) > 20f && Math.Abs(NPC.velocity.Y) <= 0.3f)
                {
                    NPC.velocity.X *= 0.99f;
                    NPC.velocity.X += NPC.direction * 0.025f;
                }
            }
            else if (npcType == NPCID.Scutlix || npcType == NPCID.VortexHornet || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || npcType == NPCID.SolarSpearman || npcType == NPCID.DesertBeast)
            {
                float maxVelocity = 5f;
                float acceleration = 0.25f;
                float turnMultiplier = 0.7f;
                if (npcType == NPCID.VortexHornet)
                {
                    maxVelocity = 6f;
                    acceleration = 0.2f;
                    turnMultiplier = 0.8f;
                }
                else if (npcType == NPCID.SolarDrakomire)
                {
                    maxVelocity = 4f;
                    acceleration = 0.1f;
                    turnMultiplier = 0.95f;
                }
                else if (npcType == NPCID.SolarSolenian)
                {
                    maxVelocity = 6f;
                    acceleration = 0.15f;
                    turnMultiplier = 0.85f;
                }
                else if (npcType == NPCID.SolarSpearman)
                {
                    maxVelocity = 5f;
                    acceleration = 0.1f;
                    turnMultiplier = 0.95f;
                }
                else if (npcType == NPCID.DesertBeast)
                {
                    maxVelocity = 5f;
                    acceleration = 0.15f;
                    turnMultiplier = 0.98f;
                }
                if (CalamityWorld.revenge)
                {
                    maxVelocity *= 1.25f;
                    acceleration *= 1.25f;
                }
                if (CalamityWorld.death)
                {
                    maxVelocity *= 1.25f;
                    acceleration *= 1.25f;
                }
                FighterRunningAI(NPC, maxVelocity, acceleration, turnMultiplier);
            }
            else if ((npcType >= NPCID.ArmedZombie && npcType <= NPCID.ArmedZombieCenx) || npcType == NPCID.Crawdad || npcType == NPCID.Crawdad2)
            {
                if (NPC.ai[2] == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float maxVelocity = CalamityWorld.death ? 2.5f : 1.5f;
                    maxVelocity *= 1f + (1f - NPC.scale);
                    FighterRunningAI(NPC, maxVelocity, 0.09f, 0.8f, true, 0.8f);
                    if (NPC.velocity.Y == 0f && (!Main.dayTime || (double)NPC.position.Y > Main.worldSurface * 16.0) && !Main.player[NPC.target].dead)
                    {
                        Vector2 playerDistance = NPC.Center - Main.player[NPC.target].Center;
                        int slowdownDistance = 50;
                        if (npcType >= NPCID.Crawdad && npcType <= NPCID.Crawdad2)
                        {
                            slowdownDistance = 42;
                        }
                        if (playerDistance.Length() < slowdownDistance && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                        {
                            NPC.velocity.X *= 0.7f;
                            NPC.ai[2] = 1f;
                        }
                    }
                }
                else
                {
                    // Set damage
                    NPC.damage = (int)Math.Round(NPC.defDamage * 1.4);

                    NPC.ai[3] = 1f;
                    NPC.velocity.X *= 0.9f;
                    if (Math.Abs(NPC.velocity.X) < 0.1f)
                    {
                        NPC.velocity.X = 0f;
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 20f || NPC.velocity.Y != 0f || (Main.dayTime && (double)NPC.position.Y < Main.worldSurface * 16.0))
                    {
                        NPC.ai[2] = 0f;
                    }
                }
            }
            else if (npcType != NPCID.SkeletonArcher &&
                npcType != NPCID.GoblinArcher &&
                npcType != NPCID.IcyMerman &&
                npcType != NPCID.PirateDeadeye &&
                npcType != NPCID.PirateCrossbower &&
                npcType != NPCID.PirateCaptain &&
                npcType != NPCID.Paladin &&
                npcType != NPCID.SkeletonSniper &&
                npcType != NPCID.TacticalSkeleton &&
                npcType != NPCID.SkeletonCommando &&
                npcType != NPCID.ElfArcher &&
                npcType != NPCID.CultistArcherBlue &&
                npcType != NPCID.CultistArcherWhite &&
                npcType != NPCID.BrainScrambler &&
                npcType != NPCID.RayGunner &&
                (npcType < NPCID.BoneThrowingSkeleton || npcType > NPCID.BoneThrowingSkeleton4) &&
                npcType != NPCID.DrManFly &&
                npcType != NPCID.GreekSkeleton &&
                npcType != NPCID.StardustSoldier &&
                npcType != NPCID.StardustSpiderSmall &&
                (npcType < NPCID.Salamander || npcType > NPCID.Salamander9) &&
                npcType != NPCID.NebulaSoldier &&
                npcType != NPCID.VortexSoldier &&
                npcType != NPCID.MartianWalker)
            {
                float velocityMax = 1f;
                if (npcType == NPCID.PincushionZombie)
                {
                    velocityMax = 1.1f;
                }
                if (npcType == NPCID.SlimedZombie)
                {
                    velocityMax = 0.9f;
                }
                if (npcType == NPCID.SwampZombie)
                {
                    velocityMax = 1.2f;
                }
                if (npcType == NPCID.TwiggyZombie)
                {
                    velocityMax = 0.8f;
                }
                if (npcType == NPCID.BaldZombie)
                {
                    velocityMax = 0.95f;
                }
                if (npcType == NPCID.FemaleZombie)
                {
                    velocityMax = 0.87f;
                }
                if (npcType == NPCID.ZombieRaincoat)
                {
                    velocityMax = 1.05f;
                }
                if (npcType == ModContent.NPCType<BucketZombie>())
                {
                    velocityMax = 0.85f;
                }
                if (npcType == NPCID.BloodZombie)
                {
                    float playerDistance = (Main.player[NPC.target].Center - NPC.Center).Length();
                    playerDistance *= 0.0025f;
                    if (playerDistance > 1.5)
                    {
                        playerDistance = 1.5f;
                    }
                    if (Main.expertMode)
                    {
                        velocityMax = 3f - playerDistance;
                    }
                    else
                    {
                        velocityMax = 2.5f - playerDistance;
                    }
                    velocityMax *= 0.8f;
                }
                if (npcType == NPCID.BloodZombie || npcType == NPCID.Zombie || npcType == NPCID.BaldZombie || npcType == NPCID.PincushionZombie || npcType == NPCID.SlimedZombie || npcType == NPCID.SwampZombie || npcType == NPCID.TwiggyZombie || npcType == NPCID.FemaleZombie || npcType == NPCID.ZombieRaincoat || npcType == NPCID.ZombieXmas || npcType == NPCID.ZombieSweater || npcType == ModContent.NPCType<BucketZombie>())
                {
                    velocityMax *= 1f + (1f - NPC.scale);
                }
                if (CalamityWorld.revenge)
                    velocityMax *= 1.25f;
                if (CalamityWorld.death)
                    velocityMax *= 1.25f;
                FighterRunningAI(NPC, velocityMax, 0.09f, 0.8f, true, 0.8f);
            }

            if (npcType >= 277 && npcType <= 280)
            {
                Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.2f, 0.1f, 0f);
            }
            else if (npcType == NPCID.MartianWalker)
            {
                Lighting.AddLight(NPC.Top + new Vector2(0f, 20f), 0.3f, 0.3f, 0.7f);
            }
            else if (npcType == NPCID.DesertGhoulCorruption)
            {
                Vector3 rgb = new Vector3(0.7f, 1f, 0.2f) * 0.5f;
                Lighting.AddLight(NPC.Top + new Vector2(0f, 15f), rgb);
            }
            else if (npcType == NPCID.DesertGhoulCrimson)
            {
                Vector3 rgb2 = new Vector3(1f, 1f, 0.5f) * 0.4f;
                Lighting.AddLight(NPC.Top + new Vector2(0f, 15f), rgb2);
            }
            else if (npcType == NPCID.DesertGhoulHallow)
            {
                Vector3 rgb3 = new Vector3(0.6f, 0.3f, 1f) * 0.4f;
                Lighting.AddLight(NPC.Top + new Vector2(0f, 15f), rgb3);
            }
            else if (npcType == NPCID.SolarDrakomire)
            {
                NPC.hide = false;
                // I'd assume the Drakomire is drawn by the rider if it's present
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == NPCID.SolarDrakomireRider && n.ai[0] == (float)NPC.whoAmI)
                    {
                        NPC.hide = true;
                        break;
                    }
                }
            }
            else if (npcType == NPCID.MushiLadybug)
            {
                if (NPC.velocity.Y != 0f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    if (Main.player[NPC.target].Center.X < NPC.position.X && NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X *= 0.95f;
                    }
                    else if (Main.player[NPC.target].Center.X > NPC.position.X + (float)NPC.width && NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X *= 0.95f;
                    }
                    if (Main.player[NPC.target].Center.X < NPC.position.X && NPC.velocity.X > -5f)
                    {
                        NPC.velocity.X -= 0.1f;
                    }
                    else if (Main.player[NPC.target].Center.X > NPC.position.X + (float)NPC.width && NPC.velocity.X < 5f)
                    {
                        NPC.velocity.X += 0.1f;
                    }
                }
                else if (Main.player[NPC.target].Center.Y + 50f < NPC.position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.velocity.Y = -9f;
                }
            }
            else if (npcType == NPCID.VortexRifleman)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.ai[2] = 0f;
                }
                if (NPC.velocity.Y != 0f && NPC.ai[2] == 1f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = -NPC.direction;
                    if (Collision.CanHit(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                    {
                        float idealLocationX = Main.player[NPC.target].Center.X - (float)(NPC.direction * 600) - NPC.Center.X;
                        float idealLocationY = Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y;
                        if (idealLocationX < 0f && NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X *= 0.9f;
                        }
                        else if (idealLocationX > 0f && NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X *= 0.9f;
                        }
                        if (idealLocationX < 0f && NPC.velocity.X > -7f)
                        {
                            NPC.velocity.X -= 0.12f;
                        }
                        else if (idealLocationX > 0f && NPC.velocity.X < 7f)
                        {
                            NPC.velocity.X += 0.12f;
                        }
                        if (NPC.velocity.X > 8f)
                        {
                            NPC.velocity.X = 8f;
                        }
                        if (NPC.velocity.X < -8f)
                        {
                            NPC.velocity.X = -8f;
                        }
                        if (idealLocationY < -20f && NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y *= 0.8f;
                        }
                        else if (idealLocationY > 20f && NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y *= 0.8f;
                        }
                        if (idealLocationY < -20f && NPC.velocity.Y > -7f)
                        {
                            NPC.velocity.Y -= 0.35f;
                        }
                        else if (idealLocationY > 20f && NPC.velocity.Y < 7f)
                        {
                            NPC.velocity.Y += 0.35f;
                        }
                    }
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 position = NPC.Center + new Vector2((float)(NPC.direction * -14), -8f) - Vector2.One * 4f;
                        Vector2 velocity = new Vector2((float)(NPC.direction * -6), 12f) * 0.2f + Utils.RandomVector2(Main.rand, -1f, 1f) * 0.1f;
                        Dust dust = Main.dust[Dust.NewDust(position, 8, 8, DustID.Vortex, velocity.X, velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 0.5f)];
                        dust.noGravity = true;
                        dust.velocity = velocity;
                        dust.customData = NPC;
                    }
                    // Adjust velocity based on the other storm drivers who want to pump your face full of bullets
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.whoAmI != NPC.whoAmI && n.type == npcType && Math.Abs(NPC.position.X - n.position.X) + Math.Abs(NPC.position.Y - n.position.Y) < (float)NPC.width)
                        {
                            if (NPC.position.X < n.position.X)
                            {
                                NPC.velocity.X -= 0.05f;
                            }
                            else
                            {
                                NPC.velocity.X += 0.05f;
                            }
                            if (NPC.position.Y < n.position.Y)
                            {
                                NPC.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                NPC.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[NPC.target].Center.Y + 100f < NPC.position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.velocity.Y = -7f;
                    NPC.ai[2] = 1f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[2]++;
                    bool closeToPlayer = NPC.Distance(Main.player[NPC.target].Center) < 600f && Math.Abs(NPC.SafeDirectionTo(Main.player[NPC.target].Center).Y) < 0.5f;
                    float vortexShotgunGateValue = CalamityWorld.death ? 240f : CalamityWorld.revenge ? 360f : 480f;
                    if (NPC.localAI[2] >= vortexShotgunGateValue && closeToPlayer && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                    {
                        NPC.localAI[2] = 0f;
                        Vector2 spawnPosition = NPC.Center + new Vector2(NPC.direction * 30f, 2f);
                        float vortexLaserVelocity = CalamityWorld.death ? 7f : CalamityWorld.revenge ? 6f : 5f;
                        Vector2 baseLaserVelocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center, Vector2.UnitX * NPC.direction) * vortexLaserVelocity;

                        int damage = Main.expertMode ? 50 : 75;
                        float maxSpread = CalamityWorld.death ? 0.7f : CalamityWorld.revenge ? 0.6f : 0.5f;
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 randomizedVelocity = baseLaserVelocity + Utils.RandomVector2(Main.rand, -maxSpread, maxSpread);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, randomizedVelocity, ProjectileID.VortexLaser, damage, 1f, Main.myPlayer);
                        }
                    }
                }
            }
            else if (npcType == NPCID.VortexHornet)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.ai[2] = 0f;
                    NPC.rotation = 0f;
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.1f;
                }
                if (NPC.velocity.Y != 0f && NPC.ai[2] == 1f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = -NPC.direction;
                    if (Collision.CanHit(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                    {
                        float playerDistX = Main.player[NPC.target].Center.X - NPC.Center.X;
                        float playerDistY = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                        if (playerDistX < 0f && NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X *= 0.98f;
                        }
                        else if (playerDistX > 0f && NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X *= 0.98f;
                        }
                        if (playerDistX < -20f && NPC.velocity.X > -(CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X -= CalamityWorld.revenge ? 0.025f : 0.015f;
                        }
                        else if (playerDistX > 20f && NPC.velocity.X < (CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X += CalamityWorld.revenge ? 0.025f : 0.015f;
                        }
                        if (NPC.velocity.X > (CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X = CalamityWorld.revenge ? 8f : 6f;
                        }
                        if (NPC.velocity.X < -(CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X = -(CalamityWorld.revenge ? 8f : 6f);
                        }
                        if (playerDistY < -20f && NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y *= 0.98f;
                        }
                        else if (playerDistY > 20f && NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y *= 0.98f;
                        }
                        if (playerDistY < -20f && NPC.velocity.Y > -(CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.Y -= CalamityWorld.revenge ? 0.25f : 0.15f;
                        }
                        else if (playerDistY > 20f && NPC.velocity.Y < (CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.Y += CalamityWorld.revenge ? 0.25f : 0.15f;
                        }
                    }
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.whoAmI != NPC.whoAmI && n.type == npcType && Math.Abs(NPC.position.X - n.position.X) + Math.Abs(NPC.position.Y - n.position.Y) < (float)NPC.width)
                        {
                            if (NPC.position.X < n.position.X)
                            {
                                NPC.velocity.X -= 0.05f;
                            }
                            else
                            {
                                NPC.velocity.X += 0.05f;
                            }
                            if (NPC.position.Y < n.position.Y)
                            {
                                NPC.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                NPC.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[NPC.target].Center.Y + 100f < NPC.position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.velocity.Y = -(CalamityWorld.revenge ? 7f : 5f);
                    NPC.ai[2] = 1f;
                }
            }
            else if (npcType == NPCID.VortexHornetQueen)
            {
                if (NPC.ai[1] > 0f && NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y *= 0.85f;
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.Y = -0.4f;
                    }
                }
                if (NPC.velocity.Y != 0f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    if (Collision.CanHit(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                    {
                        float distanceToLocationX = Main.player[NPC.target].Center.X - (float)(NPC.direction * (CalamityWorld.revenge ? 450 : 300)) - NPC.Center.X;
                        if (distanceToLocationX < 40f && NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X *= 0.98f;
                        }
                        else if (distanceToLocationX > 40f && NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X *= 0.98f;
                        }
                        if (distanceToLocationX < 40f && NPC.velocity.X > -(CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X -= CalamityWorld.revenge ? 0.25f : 0.2f;
                        }
                        else if (distanceToLocationX > 40f && NPC.velocity.X < (CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X += CalamityWorld.revenge ? 0.25f : 0.2f;
                        }
                        if (NPC.velocity.X > (CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X = CalamityWorld.revenge ? 8f : 6f;
                        }
                        if (NPC.velocity.X < -(CalamityWorld.revenge ? 8f : 6f))
                        {
                            NPC.velocity.X = -(CalamityWorld.revenge ? 8f : 6f);
                        }
                    }
                }
                else if (Main.player[NPC.target].Center.Y + 100f < NPC.position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.velocity.Y = -(CalamityWorld.revenge ? 8f : 6f);
                }
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.whoAmI != NPC.whoAmI && n.type == npcType && Math.Abs(NPC.position.X - n.position.X) + Math.Abs(NPC.position.Y - n.position.Y) < (float)NPC.width)
                    {
                        if (NPC.position.X < n.position.X)
                        {
                            NPC.velocity.X -= 0.1f;
                        }
                        else
                        {
                            NPC.velocity.X += 0.1f;
                        }
                        if (NPC.position.Y < n.position.Y)
                        {
                            NPC.velocity.Y -= 0.1f;
                        }
                        else
                        {
                            NPC.velocity.Y += 0.1f;
                        }
                    }
                }
                if (Main.rand.NextBool(6) && NPC.ai[1] <= 20f)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.Center + new Vector2((float)((NPC.spriteDirection == 1) ? 8 : -20), -20f), 8, 8, DustID.Vortex, NPC.velocity.X, NPC.velocity.Y, 100, default, 1f)];
                    dust.velocity = dust.velocity / 4f + NPC.velocity / 2f;
                    dust.scale = 0.6f;
                    dust.noLight = true;
                }
                if (NPC.ai[1] >= 57f)
                {
                    int dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                    161,
                    229
                    });
                    Dust dust = Main.dust[Dust.NewDust(NPC.Center + new Vector2((float)((NPC.spriteDirection == 1) ? 8 : -20), -20f), 8, 8, dustType, NPC.velocity.X, NPC.velocity.Y, 100, default, 1f)];
                    dust.velocity = dust.velocity / 4f + NPC.SafeDirectionTo(Main.player[NPC.target].Top);
                    dust.scale = 1.2f;
                    dust.noLight = true;
                }
                if (Main.rand.NextBool(6))
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.Center, 2, 2, DustID.Vortex, 0f, 0f, 0, default, 1f)];
                    dust.position = NPC.Center + new Vector2((float)((NPC.spriteDirection == 1) ? 26 : -26), 24f);
                    dust.velocity.X = 0f;
                    if (dust.velocity.Y < 0f)
                    {
                        dust.velocity.Y = 0f;
                    }
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.noLight = true;
                }
            }
            else if (npcType == NPCID.SnowFlinx)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.rotation = 0f;
                    NPC.localAI[0] = 0f;
                }
                else if (NPC.localAI[0] == 1f)
                {
                    NPC.rotation += NPC.velocity.X * 0.05f;
                }
            }
            else if (npcType == NPCID.VortexLarva)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.rotation = 0f;
                }
                else
                {
                    NPC.rotation += NPC.velocity.X * 0.08f;
                }
            }

            // Turn into a bat
            if (npcType == NPCID.Vampire && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.Distance(Main.player[NPC.target].Center) > 300f)
                {
                    NPC.Transform(NPCID.VampireBat);
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.velocity.Y == 0f)
                TryConvertToWallClimber(NPC);

            bool prehardmodeSpiders = (NPC.type == NPCID.WallCreeper || NPC.type == NPCID.WallCreeperWall || NPC.type == NPCID.BloodCrawler || NPC.type == NPCID.BloodCrawlerWall) && CalamityWorld.revenge;
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode && NPC.target >= 0 && (npcType == NPCID.BlackRecluse || npcType == NPCID.BlackRecluseWall || NPC.type == NPCID.JungleCreeper || NPC.type == NPCID.JungleCreeperWall || prehardmodeSpiders) && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
            {
                NPC.localAI[0] += 1f;
                if (NPC.justHit)
                    NPC.localAI[0] = 0f;

                float webSpitGateValue = CalamityWorld.death ? SpiderWebSpitGateValue_Death : CalamityWorld.revenge ? SpiderWebSpitGateValue_Rev : SpiderWebSpitGateValue;

                // Emit web dust from mouth when about to fire
                Vector2 mouth = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -22f : 12f), NPC.Center.Y - 4f);
                if (NPC.localAI[0] > webSpitGateValue - SpiderWebSpitTelegraphTime)
                {
                    Dust dust = Dust.NewDustDirect(mouth, 1, 1, DustID.Web, 0f, 0f, 100, default, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }

                if (NPC.localAI[0] >= webSpitGateValue)
                {
                    NPC.localAI[0] = 0f;
                    Vector2 velocity = (Main.player[NPC.target].Center - mouth).SafeNormalize(-Vector2.UnitY) * (prehardmodeSpiders ? 6f : 10f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), mouth, velocity, ProjectileID.WebSpit, 18, 0f, Main.myPlayer);
                }
            }
            else if (npcType == NPCID.BlackRecluse || npcType == NPCID.BlackRecluseWall || NPC.type == NPCID.JungleCreeper || NPC.type == NPCID.JungleCreeperWall || prehardmodeSpiders)
                NPC.localAI[0] = 0f;

            if (npcType == NPCID.IceGolem)
            {
                if (NPC.justHit || NPC.confused || NPC.velocity.Y != 0f || Main.player[NPC.target].dead || Main.player[NPC.target].frozen || !((NPC.direction > 0 && NPC.Center.X < Main.player[NPC.target].Center.X) || (NPC.direction < 0 && NPC.Center.X > Main.player[NPC.target].Center.X)) || !Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    NPC.ai[2] = 0f;

                NPC.ai[2] += 1f;
                Vector2 eyeLocation = NPC.Center + Vector2.UnitX * (NPC.direction == -1 ? -14f : 6f) - Vector2.UnitY * 40f;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] >= (CalamityWorld.death ? IceGolemFrostBeamGateValue_Death : CalamityWorld.revenge ? IceGolemFrostBeamGateValue_Rev : IceGolemFrostBeamGateValue) && NPC.velocity.Y == 0f && !Main.player[NPC.target].dead && !Main.player[NPC.target].frozen && ((NPC.direction > 0 && NPC.Center.X < Main.player[NPC.target].Center.X) || (NPC.direction < 0 && NPC.Center.X > Main.player[NPC.target].Center.X)) && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.netUpdate = true;
                    float iceLaserVelocity = CalamityWorld.death ? 12f : CalamityWorld.revenge ? 10f : 8f;
                    Vector2 spawnPosition = eyeLocation;
                    Vector2 velocity = (Main.player[NPC.target].Center - spawnPosition).SafeNormalize(-Vector2.UnitY) * iceLaserVelocity;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition + velocity.SafeNormalize(-Vector2.UnitY) * 100f, velocity, ProjectileID.FrostBeam, 32, 0f, Main.myPlayer);
                    NPC.ai[2] = 0f;
                }

                // Emit ice dust from eye when about to fire
                if (NPC.ai[2] > (CalamityWorld.death ? IceGolemFrostBeamGateValue_Death : CalamityWorld.revenge ? IceGolemFrostBeamGateValue_Rev : IceGolemFrostBeamGateValue) - IceGolemFrostBeamTelegraphTime)
                {
                    Dust dust = Dust.NewDustDirect(eyeLocation, 1, 1, DustID.Frost, 0f, 0f, 200, default, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }
            }

            if (npcType == NPCID.Eyezor)
            {
                if (NPC.justHit || NPC.confused || NPC.velocity.Y != 0f || Main.player[NPC.target].dead || !((NPC.direction > 0 && NPC.Center.X < Main.player[NPC.target].Center.X) || (NPC.direction < 0 && NPC.Center.X > Main.player[NPC.target].Center.X)) || !Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    NPC.ai[2] = 0f;

                NPC.ai[2] += 1f;
                Vector2 eyeLocation = new Vector2(NPC.position.X + (NPC.direction == -1 ? -12f : 6f) + (float)NPC.width * 0.5f, NPC.position.Y + 6f);
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] >= (CalamityWorld.death ? EyezorLaserGateValue_Death : CalamityWorld.revenge ? EyezorLaserGateValue_Rev : EyezorLaserGateValue) && NPC.velocity.Y == 0f && !Main.player[NPC.target].dead && ((NPC.direction > 0 && NPC.Center.X < Main.player[NPC.target].Center.X) || (NPC.direction < 0 && NPC.Center.X > Main.player[NPC.target].Center.X)) && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    float eyeLaserVelocity = CalamityWorld.death ? 6f : CalamityWorld.revenge ? 5f : 4f;
                    Vector2 spawnPosition = eyeLocation;
                    Vector2 velocity = (Main.player[NPC.target].Center - spawnPosition).SafeNormalize(-Vector2.UnitY) * eyeLaserVelocity;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition + velocity.SafeNormalize(-Vector2.UnitY) * 80f, velocity, ProjectileID.EyeLaser, 40, 0f, Main.myPlayer);
                    NPC.ai[2] = 0f;
                }

                // Emit demon dust from eye when about to fire
                if (NPC.ai[2] > (CalamityWorld.death ? EyezorLaserGateValue_Death : CalamityWorld.revenge ? EyezorLaserGateValue_Rev : EyezorLaserGateValue) - EyezorLaserTelegraphTime)
                {
                    Dust dust = Dust.NewDustDirect(eyeLocation, 1, 1, DustID.Shadowflame, 0f, 0f, 100, default, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }
            }

            if (npcType == NPCID.MartianEngineer)
            {
                if (NPC.confused)
                {
                    NPC.ai[2] = -40f;
                }
                else
                {
                    if (NPC.ai[2] < 40f)
                    {
                        NPC.ai[2] += 1f;
                    }
                    if (NPC.ai[2] > 0f && NPC.CountNPCS(NPCID.MartianTurret) >= 4 * NPC.CountNPCS(NPCID.MartianEngineer))
                    {
                        NPC.ai[2] = 0f;
                    }
                    if (NPC.justHit)
                    {
                        NPC.ai[2] = -20f;
                    }
                    if (NPC.ai[2] == 20f)
                    {
                        int centerTileX = (int)NPC.position.X / 16;
                        int centerTileY = (int)NPC.position.Y / 16;
                        int centerTileX2 = (int)NPC.position.X / 16;
                        int centerTileY2 = (int)NPC.position.Y / 16;
                        int maxTurretDistX = 5;
                        int maxTurretDistanceY = 2;
                        int tryCounter = 0;
                        bool createdTurret = false;
                        while (!createdTurret && tryCounter < 100)
                        {
                            tryCounter++;
                            int turretSpawnX = Main.rand.Next(centerTileX - maxTurretDistX, centerTileX + maxTurretDistX);
                            for (int y = Main.rand.Next(centerTileY - maxTurretDistX, centerTileY + maxTurretDistX); y < centerTileY + maxTurretDistX; y++)
                            {
                                if ((y < centerTileY - maxTurretDistanceY || y > centerTileY + maxTurretDistanceY || turretSpawnX < centerTileX - maxTurretDistanceY || turretSpawnX > centerTileX + maxTurretDistanceY) && (y < centerTileY2 || y > centerTileY2 || turretSpawnX < centerTileX2 || turretSpawnX > centerTileX2) && Main.tile[turretSpawnX, y].HasUnactuatedTile)
                                {
                                    bool notLava = true;
                                    if (Main.tile[turretSpawnX, y - 1].LiquidType == LiquidID.Lava)
                                    {
                                        notLava = false;
                                    }
                                    if (notLava && Main.tileSolid[(int)Main.tile[turretSpawnX, y].TileType] && !Collision.SolidTiles(turretSpawnX - 1, turretSpawnX + 1, y - 4, y - 1))
                                    {
                                        int turretIdx = NPC.NewNPC(NPC.GetSource_FromAI(), turretSpawnX * 16 - NPC.width / 2, y * 16, NPCID.MartianTurret, 0, 0f, 0f, 0f, 0f, 255);
                                        Main.npc[turretIdx].position.Y = (float)(y * 16 - Main.npc[turretIdx].height);
                                        createdTurret = true;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (NPC.ai[2] == 40f)
                    {
                        NPC.ai[2] = -90f;
                    }
                }
            }

            if (npcType == NPCID.GigaZapper)
            {
                if (NPC.confused)
                {
                    NPC.ai[2] = -40f;
                }
                else
                {
                    if (NPC.ai[2] < 20f)
                    {
                        NPC.ai[2] += 1f;
                    }
                    if (NPC.justHit)
                    {
                        NPC.ai[2] = -20f;
                    }
                    if (NPC.ai[2] == 20f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[2] = (float)(-10 + Main.rand.Next(3) * -10);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 8f, (float)(NPC.direction * 8), 0f, ProjectileID.GigaZapperSpear, 25, 1f, Main.myPlayer);
                    }
                }
            }

            if (npcType == NPCID.SkeletonArcher ||
                npcType == NPCID.GoblinArcher ||
                npcType == NPCID.IcyMerman ||
                npcType == NPCID.PirateDeadeye ||
                npcType == NPCID.PirateCrossbower ||
                npcType == NPCID.PirateCaptain ||
                npcType == NPCID.Paladin ||
                npcType == NPCID.SkeletonSniper ||
                npcType == NPCID.TacticalSkeleton ||
                npcType == NPCID.SkeletonCommando ||
                npcType == NPCID.ElfArcher ||
                npcType == NPCID.CultistArcherBlue ||
                npcType == NPCID.CultistArcherWhite ||
                npcType == NPCID.BrainScrambler ||
                npcType == NPCID.RayGunner ||
                (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4) ||
                (npcType == NPCID.DrManFly ||
                npcType == NPCID.GreekSkeleton ||
                npcType == NPCID.StardustSoldier ||
                npcType == NPCID.StardustSpiderBig ||
                (npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9)) ||
                npcType == NPCID.NebulaSoldier ||
                npcType == NPCID.VortexHornetQueen ||
                npcType == NPCID.MartianWalker)
            {
                bool npcAllowedToShoot = npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianWalker;
                bool isAlienQueen = npcType == NPCID.VortexHornetQueen;
                bool canShootAtTarget = true;
                int stardustGateValue = -1;
                int stardustGateValueAdd = -1;
                if (npcType == NPCID.StardustSoldier)
                {
                    npcAllowedToShoot = true;
                    stardustGateValue = 90;
                    stardustGateValueAdd = 90;
                    if (NPC.ai[1] <= 150f)
                    {
                        canShootAtTarget = false;
                    }
                }
                if (NPC.confused)
                {
                    NPC.ai[2] = 0f;
                }
                else
                {
                    if (NPC.ai[1] > 0f)
                    {
                        NPC.ai[1] -= 1f;
                    }
                    if (NPC.justHit)
                    {
                        NPC.ai[1] = 30f;
                        NPC.ai[2] = 0f;
                    }
                    int attackTimeMax = npcType switch
                    {
                        NPCID.CultistArcherBlue or NPCID.CultistArcherWhite or NPCID.BrainScrambler or NPCID.RayGunner or NPCID.PirateCrossbower => 80,
                        NPCID.MartianWalker => 15,
                        NPCID.ElfArcher => 110,
                        NPCID.SkeletonSniper => 200,
                        NPCID.TacticalSkeleton => 120,
                        NPCID.SkeletonCommando => 90,
                        NPCID.GoblinArcher or NPCID.NebulaSoldier => 180,
                        NPCID.IcyMerman => 50,
                        NPCID.GreekSkeleton => 100,
                        NPCID.PirateDeadeye => 40,
                        NPCID.Paladin => 30,
                        NPCID.StardustSoldier => 300,
                        NPCID.StardustSpiderBig or NPCID.VortexHornetQueen => 60,
                        _ => 70
                    };
                    bool priateCaptainBoost = false;
                    if (npcType == NPCID.PirateCaptain)
                    {
                        if (NPC.localAI[2] >= 20f)
                        {
                            priateCaptainBoost = true;
                        }
                        if (priateCaptainBoost)
                        {
                            attackTimeMax = 60;
                        }
                        else
                        {
                            attackTimeMax = 8;
                        }
                    }

                    if (CalamityWorld.revenge)
                        attackTimeMax = (int)(attackTimeMax * 0.75);

                    int modifiedAttackTime = attackTimeMax / 2;
                    if (npcType == NPCID.NebulaSoldier)
                    {
                        modifiedAttackTime = attackTimeMax - 1;
                    }
                    if (npcType == NPCID.VortexHornetQueen)
                    {
                        modifiedAttackTime = attackTimeMax - 1;
                    }
                    if (NPC.ai[2] > 0f)
                    {
                        if (canShootAtTarget)
                        {
                            NPC.TargetClosest(true);
                        }
                        if (NPC.ai[1] == (float)modifiedAttackTime)
                        {
                            if (npcType == NPCID.PirateCaptain)
                            {
                                NPC.localAI[2] += 1f;
                            }
                            float projSpeed = npcType switch
                            {
                                _ when npcType >= 498 && npcType <= 506 => CalamityWorld.death ? 4f : 7f,
                                NPCID.StardustSoldier => 1f,
                                NPCID.DrManFly => CalamityWorld.death ? 4.5f : 7.5f,
                                NPCID.GreekSkeleton => CalamityWorld.death ? 5f : 8f,
                                _ when npcType >= 449 && npcType <= 452 => CalamityWorld.death ? 4f : 7f,
                                NPCID.StardustSpiderBig => 4f,
                                NPCID.MartianWalker => CalamityWorld.death ? 5f : 8f,
                                NPCID.RayGunner or NPCID.IcyMerman => CalamityWorld.death ? 4f : 7f,
                                NPCID.PirateCrossbower => CalamityWorld.death ? 9f : 16f,
                                NPCID.PirateDeadeye => CalamityWorld.death ? 8f : 14f,
                                NPCID.SkeletonCommando => CalamityWorld.death ? 2.5f : 4f,
                                NPCID.Paladin or NPCID.GoblinArcher => CalamityWorld.death ? 5f : 9f,
                                _ => CalamityWorld.death ? 6f : 11f
                            };

                            if (CalamityWorld.revenge)
                                projSpeed *= 1.25f;

                            Vector2 spawnPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                spawnPosition.Y -= 14f;
                            }
                            if (npcType == NPCID.IcyMerman)
                            {
                                spawnPosition.Y -= 10f;
                            }
                            if (npcType == NPCID.Paladin)
                            {
                                spawnPosition.Y -= 10f;
                            }
                            if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner)
                            {
                                spawnPosition.Y += 6f;
                            }
                            if (npcType == NPCID.MartianWalker)
                            {
                                spawnPosition.Y = NPC.position.Y + 20f;
                            }
                            if (npcType >= 498 && npcType <= 506)
                            {
                                spawnPosition.Y -= 8f;
                            }
                            if (npcType == NPCID.VortexHornetQueen)
                            {
                                spawnPosition += new Vector2((float)(NPC.spriteDirection * 2), -12f);
                                projSpeed = CalamityWorld.death ? 6f : CalamityWorld.revenge ? 9f : 7f;
                            }
                            float distX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - spawnPosition.X;
                            float projOffset = Math.Abs(distX) * 0.1f;
                            if (npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton)
                            {
                                projOffset = 0f;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                projOffset = Math.Abs(distX) * 0.08f;
                            }
                            if (npcType == NPCID.PirateDeadeye || (npcType == NPCID.PirateCaptain && !priateCaptainBoost))
                            {
                                projOffset = 0f;
                            }
                            if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianWalker)
                            {
                                projOffset = 0f;
                            }
                            if (npcType >= 449 && npcType <= 452)
                            {
                                projOffset = Math.Abs(distX) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                projOffset = Math.Abs(distX) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                projOffset = Math.Abs(distX) * (float)Main.rand.Next(-10, 11) * 0.0035f;
                            }
                            if (npcType >= 498 && npcType <= 506)
                            {
                                projOffset = Math.Abs(distX) * (float)Main.rand.Next(1, 11) * 0.0025f;
                            }
                            float distY = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - spawnPosition.Y - projOffset;
                            float magnitude = (float)Math.Sqrt((distX * distX + distY * distY));
                            NPC.netUpdate = true;
                            magnitude = projSpeed / magnitude;
                            distX *= magnitude;
                            distY *= magnitude;
                            var (projectileType, damage) = npcType switch
                            {
                                NPCID.PirateCaptain when priateCaptainBoost => (ProjectileID.CannonballHostile, 100),
                                NPCID.PirateCaptain => (ProjectileID.BulletDeadeye, 30),
                                NPCID.TacticalSkeleton => (ProjectileID.BulletDeadeye, 50),
                                NPCID.PirateCrossbower => (ProjectileID.FlamingArrow, 40),
                                NPCID.PirateDeadeye => (ProjectileID.BulletDeadeye, 25),
                                NPCID.SkeletonCommando => (ProjectileID.RocketSkeleton, 60),
                                NPCID.Paladin => (ProjectileID.PaladinsHammerHostile, 60),
                                NPCID.SkeletonSniper => (ProjectileID.SniperBullet, 100),
                                NPCID.VortexHornetQueen => (ProjectileID.VortexAcid, Main.expertMode ? 45 : 60),
                                NPCID.NebulaSoldier => (ProjectileID.NebulaBolt, Main.expertMode ? 45 : 60),
                                NPCID.StardustSoldier => (ProjectileID.StardustSoldierLaser, Main.expertMode ? 45 : 60),
                                NPCID.DrManFly => (ProjectileID.DrManFlyFlask, 50),
                                NPCID.IcyMerman => (ProjectileID.IcewaterSpit, 37),
                                NPCID.GreekSkeleton => (ProjectileID.JavelinHostile, 18),
                                _ when npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9 => (ProjectileID.SalamanderSpit, 14),
                                _ when npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4 => (ProjectileID.SkeletonBone, 20),
                                NPCID.MartianWalker => (ProjectileID.MartianWalkerLaser, 35),
                                NPCID.RayGunner => (ProjectileID.RayGunnerLaser, 30),
                                NPCID.BrainScrambler => (ProjectileID.BrainScramblerBolt, 24),
                                NPCID.CultistArcherBlue or NPCID.CultistArcherWhite => (ProjectileID.WoodenArrowHostile, 40),
                                NPCID.GoblinArcher => (ProjectileID.WoodenArrowHostile, 11),
                                NPCID.ElfArcher => (ProjectileID.FlamingArrow, 45),
                                _ => (ProjectileID.FlamingArrow, 35)
                            };
                            if (npcType == NPCID.PirateCaptain && priateCaptainBoost)
                            {
                                NPC.localAI[2] = 0f;
                            }
                            spawnPosition.X += distX;
                            spawnPosition.Y += distY;
                            if (Main.expertMode && npcType == NPCID.Paladin)
                            {
                                damage = (int)(damage * 0.75);
                            }
                            if (Main.expertMode && npcType >= 381 && npcType <= 392)
                            {
                                damage = (int)(damage * 0.8);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npcType == NPCID.TacticalSkeleton)
                                {
                                    Vector2 bulletSpawnPosition = NPC.Center;
                                    for (int num147 = 0; num147 < 4; num147++)
                                    {
                                        distX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - spawnPosition.X;
                                        distY = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - spawnPosition.Y;
                                        magnitude = (float)Math.Sqrt((distX * distX + distY * distY));
                                        magnitude = (CalamityWorld.death ? 8f : CalamityWorld.revenge ? 7f : 6f) / magnitude;
                                        int shotgunSpread = 20;
                                        distX += (float)Main.rand.Next(-shotgunSpread, shotgunSpread + 1);
                                        distY += (float)Main.rand.Next(-shotgunSpread, shotgunSpread + 1);
                                        distX *= magnitude;
                                        distY *= magnitude;
                                        Vector2 bulletVelocity = new Vector2(distX, distY);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), bulletSpawnPosition + bulletVelocity.SafeNormalize(-Vector2.UnitY) * 30f, bulletVelocity, projectileType, damage, 0f, Main.myPlayer);
                                    }
                                }
                                else if (npcType == NPCID.StardustSoldier)
                                {
                                    int proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPosition, new Vector2(distX, distY), projectileType, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI).identity;
                                    if (CalamityWorld.death)
                                    {
                                        Main.projectile[proj].Calamity().extraUpdatesToSync = 1;
                                        Main.projectile[proj].timeLeft = 480;
                                        if (Main.dedServ)
                                        {
                                            Main.projectile[proj].netSpam = 0;
                                            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                                        }
                                    }
                                }
                                else if (npcType == NPCID.NebulaSoldier)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        int proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (float)(NPC.spriteDirection * 4), NPC.Center.Y + 6f), new Vector2((float)(-3 + 2 * i) * 0.15f, (float)(-(float)Main.rand.Next(0, 3)) * 0.2f - 0.1f), projectileType, damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI).identity;
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
                                }
                                else if (npcType == NPCID.StardustSpiderBig)
                                {
                                    int idx = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.StardustSpiderSmall, NPC.whoAmI);
                                    Main.npc[idx].velocity = new Vector2(distX, -6f + distY);
                                }
                                else
                                {
                                    int proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPosition, new Vector2(distX, distY), projectileType, damage, 0f, Main.myPlayer).identity;
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
                            }
                            if (Math.Abs(distY) > Math.Abs(distX) * 2f)
                            {
                                if (distY > 0f)
                                {
                                    NPC.ai[2] = 1f;
                                }
                                else
                                {
                                    NPC.ai[2] = 5f;
                                }
                            }
                            else if (Math.Abs(distX) > Math.Abs(distY) * 2f)
                            {
                                NPC.ai[2] = 3f;
                            }
                            else if (distY > 0f)
                            {
                                NPC.ai[2] = 2f;
                            }
                            else
                            {
                                NPC.ai[2] = 4f;
                            }
                        }
                        if ((NPC.velocity.Y != 0f && !isAlienQueen) || NPC.ai[1] <= 0f)
                        {
                            NPC.ai[2] = 0f;
                            NPC.ai[1] = 0f;
                        }
                        else if (!npcAllowedToShoot || (stardustGateValue != -1 && NPC.ai[1] >= (float)stardustGateValue && NPC.ai[1] < (float)(stardustGateValue + stardustGateValueAdd) && (!isAlienQueen || NPC.velocity.Y == 0f)))
                        {
                            NPC.velocity.X *= 0.9f;
                            NPC.spriteDirection = NPC.direction;
                        }
                    }

                    if (npcType == NPCID.DrManFly && !Main.eclipse)
                    {
                        npcAllowedToShoot = true;
                    }
                    else if ((NPC.ai[2] <= 0f | npcAllowedToShoot) && (NPC.velocity.Y == 0f | isAlienQueen) && NPC.ai[1] <= 0f && !Main.player[NPC.target].dead)
                    {
                        bool canAttack = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                        if (npcType == NPCID.MartianWalker)
                        {
                            canAttack = Collision.CanHitLine(NPC.Top + new Vector2(0f, 20f), 0, 0, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                        }
                        if (Main.player[NPC.target].stealth == 0f && Main.player[NPC.target].itemAnimation == 0)
                        {
                            canAttack = false;
                        }
                        if (canAttack)
                        {
                            Vector2 projSpawnPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                            float distX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - projSpawnPosition.X;
                            float distXAbsolute = Math.Abs(distX) * 0.1f;
                            float distY = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - projSpawnPosition.Y - distXAbsolute;
                            distX += (float)Main.rand.Next(-40, 41);
                            distY += (float)Main.rand.Next(-40, 41);
                            float playerDistance = (float)Math.Sqrt(distX * distX + distY * distY);
                            float maxAttackDistance = 700f;
                            if (npcType == NPCID.PirateDeadeye)
                            {
                                maxAttackDistance = 550f;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                maxAttackDistance = 800f;
                            }
                            if (npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9)
                            {
                                maxAttackDistance = 190f;
                            }
                            if (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4)
                            {
                                maxAttackDistance = 200f;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                maxAttackDistance = 400f;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                maxAttackDistance = 400f;
                            }
                            if (CalamityWorld.death)
                            {
                                maxAttackDistance *= 1.25f;
                            }
                            if (playerDistance < maxAttackDistance)
                            {
                                NPC.netUpdate = true;
                                NPC.velocity.X *= 0.5f;
                                playerDistance = 10f / playerDistance;
                                distX *= playerDistance;
                                distY *= playerDistance;
                                NPC.ai[2] = 3f;
                                NPC.ai[1] = (float)attackTimeMax;
                                if (Math.Abs(distY) > Math.Abs(distX) * 2f)
                                {
                                    if (distY > 0f)
                                    {
                                        NPC.ai[2] = 1f;
                                    }
                                    else
                                    {
                                        NPC.ai[2] = 5f;
                                    }
                                }
                                else if (Math.Abs(distX) > Math.Abs(distY) * 2f)
                                {
                                    NPC.ai[2] = 3f;
                                }
                                else if (distY > 0f)
                                {
                                    NPC.ai[2] = 2f;
                                }
                                else
                                {
                                    NPC.ai[2] = 4f;
                                }
                            }
                        }
                    }

                    if (NPC.ai[2] <= 0f || (npcAllowedToShoot && (stardustGateValue == -1 || NPC.ai[1] < (float)stardustGateValue || NPC.ai[1] >= (float)(stardustGateValue + stardustGateValueAdd))))
                    {
                        float maxVelocity = 1f;
                        float acceleration = 0.07f;
                        float decelerationFactor = 0.8f;
                        if (npcType == NPCID.PirateDeadeye)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.09f;
                        }
                        else if (npcType == NPCID.PirateCrossbower)
                        {
                            maxVelocity = 1.5f;
                            acceleration = 0.08f;
                        }
                        else if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        else if (npcType == NPCID.MartianWalker)
                        {
                            maxVelocity = 4f;
                            acceleration = 1f;
                            decelerationFactor = 0.7f;
                        }
                        else if (npcType == NPCID.StardustSoldier)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        else if (npcType == NPCID.StardustSpiderBig)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        else if (NPC.type == NPCID.VortexHornetQueen)
                        {
                            maxVelocity = 4f;
                            acceleration = 0.6f;
                            decelerationFactor = 0.95f;
                        }
                        if (CalamityWorld.revenge)
                        {
                            maxVelocity *= 1.5f;
                            acceleration *= 1.5f;
                        }
                        bool forceDeceleration = false;
                        if ((npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner) && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 300f && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                        {
                            forceDeceleration = true;
                            NPC.ai[3] = 0f;
                        }
                        if (npcType == NPCID.MartianWalker && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 400f && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                        {
                            forceDeceleration = true;
                            NPC.ai[3] = 0f;
                        }
                        // The extra OR conditional is the reason the special method I created above isn't used.
                        // It has enough parameters. Another one for 1 specific purpose isn't worth it anymore
                        if ((NPC.velocity.X < -maxVelocity || NPC.velocity.X > maxVelocity) | forceDeceleration)
                        {
                            if (NPC.velocity.Y == 0f)
                            {
                                NPC.velocity *= decelerationFactor;
                            }
                        }
                        else if (NPC.velocity.X < maxVelocity && NPC.direction == 1)
                        {
                            NPC.velocity.X += acceleration;
                            if (NPC.velocity.X > maxVelocity)
                            {
                                NPC.velocity.X = maxVelocity;
                            }
                        }
                        else if (NPC.velocity.X > -maxVelocity && NPC.direction == -1)
                        {
                            NPC.velocity.X -= acceleration;
                            if (NPC.velocity.X < -maxVelocity)
                            {
                                NPC.velocity.X = -maxVelocity;
                            }
                        }
                    }

                    if (npcType == NPCID.MartianWalker)
                    {
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= 6f)
                        {
                            NPC.localAI[2] = 0f;
                            NPC.localAI[3] = Main.player[NPC.target].DirectionFrom(NPC.Top + new Vector2(0f, 20f)).ToRotation();
                        }
                    }
                }
            }

            if (npcType == NPCID.Clown && Main.netMode != NetmodeID.MultiplayerClient && !Main.player[NPC.target].dead)
            {
                if (NPC.justHit)
                    NPC.ai[2] = 0f;

                NPC.ai[2] += 1f;
                float bombDelay = CalamityWorld.death ? 60f : 180f;
                if (NPC.ai[2] > bombDelay)
                {
                    Vector2 spawnPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f - (float)(NPC.direction * 24), NPC.position.Y + 4f);
                    if (!Main.rand.NextBool(5) || NPC.AnyNPCs(NPCID.ChatteringTeethBomb))
                    {
                        int velocityX = 3 * NPC.direction;
                        int velocityY = -5;
                        int clownBomb = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), spawnPosition, new Vector2(velocityX, velocityY), ProjectileID.HappyBomb, 0, 0f, Main.myPlayer, 0f, 0f).identity;
                        Main.projectile[clownBomb].timeLeft = 300;
                        if (CalamityWorld.death)
                        {
                            Main.projectile[clownBomb].Calamity().extraUpdatesToSync = 1;
                            Main.projectile[clownBomb].timeLeft = 600;
                            if (Main.dedServ)
                            {
                                Main.projectile[clownBomb].netSpam = 0;
                                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, clownBomb);
                            }
                        }
                        NPC.ai[2] = 0f;
                    }
                    else
                    {
                        NPC.ai[2] = -bombDelay * 2;
                        int chatteringTeethBomb = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.ChatteringTeethBomb);
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, chatteringTeethBomb, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }

            bool canOpenDoors = false;
            if (NPC.velocity.Y == 0f)
            {
                int j = (int)(NPC.position.Y + (float)NPC.height + 7f) / 16;
                int npcLeft = (int)NPC.position.X / 16;
                int npcRight = (int)(NPC.position.X + (float)NPC.width) / 16;
                for (int i = npcLeft; i <= npcRight; i++)
                {
                    if (Main.tile[i, j].HasUnactuatedTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        canOpenDoors = true;
                        break;
                    }
                }
            }

            if (npcType == NPCID.VortexLarva)
            {
                canOpenDoors = false;
            }

            if (NPC.velocity.Y >= 0f)
            {
                int velocitySign = 0;
                if (NPC.velocity.X < 0f)
                {
                    velocitySign = -1;
                }
                if (NPC.velocity.X > 0f)
                {
                    velocitySign = 1;
                }
                Vector2 positionDelta = NPC.position;
                positionDelta.X += NPC.velocity.X;
                int x = (int)((positionDelta.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 1) * velocitySign)) / 16f);
                int y = (int)((positionDelta.Y + (float)NPC.height - 1f) / 16f);
                if (x * 16 < positionDelta.X + (float)NPC.width &&
                    (x + 1) * 16 > positionDelta.X && ((Main.tile[x, y].HasUnactuatedTile &&
                    !Main.tile[x, y].TopSlope && !Main.tile[x, y - 1].TopSlope &&
                    Main.tileSolid[(int)Main.tile[x, y].TileType] &&
                    !Main.tileSolidTop[(int)Main.tile[x, y].TileType]) ||
                    (Main.tile[x, y - 1].IsHalfBlock && Main.tile[x, y - 1].HasUnactuatedTile)) &&
                    (!Main.tile[x, y - 1].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 1].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 1].TileType] ||
                    (Main.tile[x, y - 1].IsHalfBlock &&
                    (!Main.tile[x, y - 4].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 4].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 4].TileType]))) &&
                    (!Main.tile[x, y - 2].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 2].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 2].TileType]) &&
                    (!Main.tile[x, y - 3].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 3].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 3].TileType]) &&
                    (!Main.tile[x - velocitySign, y - 3].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x - velocitySign, y - 3].TileType]))
                {
                    float yAdjust = y * 16f;
                    if (Main.tile[x, y].IsHalfBlock)
                    {
                        yAdjust += 8f;
                    }
                    if (Main.tile[x, y - 1].IsHalfBlock)
                    {
                        yAdjust -= 8f;
                    }
                    if (yAdjust < positionDelta.Y + (float)NPC.height)
                    {
                        float gfxOffRelativeToDelta = positionDelta.Y + (float)NPC.height - yAdjust;
                        float yOffsetFloor = 16.1f;
                        if (npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.JungleCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.DesertScorpionWalk)
                        {
                            yOffsetFloor += 8f;
                        }
                        if (gfxOffRelativeToDelta <= yOffsetFloor)
                        {
                            NPC.gfxOffY += NPC.position.Y + (float)NPC.height - yAdjust;
                            NPC.position.Y = yAdjust - (float)NPC.height;
                            if (gfxOffRelativeToDelta < 9f)
                            {
                                NPC.stepSpeed = 1f;
                            }
                            else
                            {
                                NPC.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }

            if (canOpenDoors)
            {
                int x = (int)((NPC.Center.X + (float)(15 * NPC.direction)) / 16f);
                int y = (int)((NPC.position.Y + (float)NPC.height - 15f) / 16f);
                if (npcType == NPCID.Clown || npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.JungleCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.Paladin || npcType == NPCID.Scutlix || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexHornet || npcType == NPCID.VortexHornetQueen || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast)
                {
                    x = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 16) * NPC.direction)) / 16f);
                }
                if ((Main.tile[x, y - 1].HasUnactuatedTile &&
                    (Main.tile[x, y - 1].TileType == TileID.ClosedDoor ||
                    Main.tile[x, y - 1].TileType == TileID.TallGateClosed)) & reset)
                {
                    NPC.ai[2] += 1f;
                    NPC.ai[3] = 0f;
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.velocity.X = -0.5f * NPC.direction;
                        int timerIncrement = 5;
                        if (Main.tile[x, y - 1].TileType == TileID.TallGateClosed)
                        {
                            timerIncrement = 2;
                        }
                        NPC.ai[1] += timerIncrement;
                        // Special increments
                        if (npcType == NPCID.GoblinThief)
                        {
                            NPC.ai[1] += 1f;
                        }
                        if (npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet)
                        {
                            NPC.ai[1] += 6f;
                        }
                        NPC.ai[2] = 0f;
                        bool readyToOpenDoor = false;
                        if (NPC.ai[1] >= 10f)
                        {
                            readyToOpenDoor = true;
                            NPC.ai[1] = 10f;
                        }
                        if (npcType == NPCID.Butcher)
                        {
                            readyToOpenDoor = true;
                        }
                        WorldGen.KillTile(x, y - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !readyToOpenDoor) && readyToOpenDoor && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npcType == NPCID.GoblinPeon)
                            {
                                WorldGen.KillTile(x, y - 1, false, false, false);
                                if (Main.dedServ)
                                {
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, (float)x, (float)(y - 1), 0f, 0, 0, 0);
                                }
                            }
                            else
                            {
                                if (Main.tile[x, y - 1].TileType == TileID.ClosedDoor)
                                {
                                    bool canOpenDoor = WorldGen.OpenDoor(x, y - 1, NPC.direction);
                                    if (!canOpenDoor)
                                    {
                                        NPC.ai[3] = (float)aiGateValue;
                                        NPC.netUpdate = true;
                                    }
                                    if (Main.dedServ & canOpenDoor)
                                    {
                                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, (float)x, (float)(y - 1), (float)NPC.direction, 0, 0, 0);
                                    }
                                }
                                if (Main.tile[x, y - 1].TileType == TileID.TallGateClosed)
                                {
                                    bool canOpenTallGate = WorldGen.ShiftTallGate(x, y - 1, false);
                                    if (!canOpenTallGate)
                                    {
                                        NPC.ai[3] = (float)aiGateValue;
                                        NPC.netUpdate = true;
                                    }
                                    if (Main.dedServ & canOpenTallGate)
                                    {
                                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, (float)x, (float)(y - 1), 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    int alteredDirection = NPC.spriteDirection;
                    if (npcType == NPCID.VortexRifleman)
                    {
                        alteredDirection *= -1;
                    }
                    if ((NPC.velocity.X < 0f && alteredDirection == -1) || (NPC.velocity.X > 0f && alteredDirection == 1))
                    {
                        if (NPC.height >= 32 && Main.tile[x, y - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 2].TileType])
                        {
                            if (Main.tile[x, y - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 3].TileType])
                            {
                                NPC.velocity.Y = -9f;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity.Y = -8f;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (Main.tile[x, y - 1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 1].TileType])
                        {
                            NPC.velocity.Y = -7f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.position.Y + (float)NPC.height - (float)(y * 16) > 20f && Main.tile[x, y].HasUnactuatedTile && !Main.tile[x, y].TopSlope && Main.tileSolid[(int)Main.tile[x, y].TileType])
                        {
                            NPC.velocity.Y = -6f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.directionY < 0 && npcType != 67 && (!Main.tile[x, y + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x, y + 1].TileType]) && (!Main.tile[x + NPC.direction, y + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x + NPC.direction, y + 1].TileType]))
                        {
                            NPC.velocity.Y = -9f;
                            NPC.velocity.X *= 2f;
                            NPC.netUpdate = true;
                        }
                        else if (reset)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }
                        if ((NPC.velocity.Y == 0f & jump) && NPC.ai[3] == 1f)
                        {
                            NPC.velocity.Y = -6f;
                        }
                    }
                    if ((npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Werewolf || npcType == NPCID.CorruptPenguin || npcType == NPCID.Nymph || npcType == NPCID.GrayGrunt || npcType == NPCID.GigaZapper || npcType == NPCID.CrimsonBunny || npcType == NPCID.CrimsonPenguin || (npcType >= 524 && npcType <= 527)) && NPC.velocity.Y == 0f && Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) < 100f && Math.Abs(NPC.position.Y + (float)(NPC.height / 2) - (Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))) < 50f && ((NPC.direction > 0 && NPC.velocity.X >= 1f) || (NPC.direction < 0 && NPC.velocity.X <= -1f)))
                    {
                        NPC.velocity.X *= 3f;
                        if (NPC.velocity.X > 4f)
                        {
                            NPC.velocity.X = 4f;
                        }
                        if (NPC.velocity.X < -4f)
                        {
                            NPC.velocity.X = -4f;
                        }
                        NPC.velocity.Y = -5f;
                        NPC.netUpdate = true;
                    }
                    if ((npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()) && NPC.velocity.Y < 0f)
                    {
                        NPC.velocity.Y *= 1.1f;
                    }
                    if (npcType == NPCID.BoneLee && NPC.velocity.Y == 0f && Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) < 150f && Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) < 50f && ((NPC.direction > 0 && NPC.velocity.X >= 1f) || (NPC.direction < 0 && NPC.velocity.X <= -1f)))
                    {
                        NPC.velocity.X = (float)((CalamityWorld.death ? 9 : CalamityWorld.revenge ? 8 : 7) * NPC.direction);
                        NPC.velocity.Y = -(CalamityWorld.death ? 4f : CalamityWorld.revenge ? 3.5f : 3f);
                        NPC.netUpdate = true;
                    }
                    if (npcType == NPCID.BoneLee && NPC.velocity.Y < 0f)
                    {
                        NPC.velocity.X *= CalamityWorld.death ? 1.2f : CalamityWorld.revenge ? 1.15f : 1.1f;
                        NPC.velocity.Y *= CalamityWorld.death ? 1.1f : CalamityWorld.revenge ? 1.075f : 1.05f;
                    }
                    if (npcType == NPCID.Butcher && NPC.velocity.Y < 0f)
                    {
                        NPC.velocity.X *= 1.35f;
                        NPC.velocity.Y *= 1.15f;
                    }
                }
            }
            else if (reset)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }

            // Teleport (Chaos elementals)
            if (Main.netMode != NetmodeID.MultiplayerClient && (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()) && NPC.ai[3] >= (float)aiGateValue)
            {
                int targetTileX = (int)Main.player[NPC.target].Center.X / 16;
                int targetTileY = (int)Main.player[NPC.target].Center.Y / 16;
                Vector2 chosenTile = Vector2.Zero;
                if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, targetTileX, targetTileY, 20, 9))
                {
                    NPC.position.X = chosenTile.X * 16f - (float)(NPC.width / 2);
                    NPC.position.Y = chosenTile.Y * 16f - (float)NPC.height;
                    NPC.ai[3] = -30f;
                    NPC.netUpdate = true;
                }
            }
            return false;
        }
    }
}
