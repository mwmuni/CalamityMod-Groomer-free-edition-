using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;

public static partial class RevengeanceAndDeathAI
{
    public class WormAI : VanillaAIOverride
    {
        private void SpawnBodyAndTailSegments(int segments, int bodyType, int tailType, ref int currentSegment)
        {
            for (int i = 0; i < segments; i++)
            {
                int segmentType = bodyType;
                if (i == segments - 1)
                {
                    segmentType = tailType;
                }
                int segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), segmentType, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[segment].ai[3] = (float)NPC.whoAmI;
                Main.npc[segment].realLife = NPC.whoAmI;
                Main.npc[segment].ai[1] = (float)currentSegment;
                Main.npc[currentSegment].ai[0] = (float)segment;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0, 0, 0);
                currentSegment = segment;
            }
        }

        public override bool AI(Mod mod)
        {
            if (NPC.type == NPCID.LeechHead && NPC.localAI[1] == 0f)
            {
                NPC.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                int dustVelocity = 1;
                if (NPC.velocity.X < 0f)
                {
                    dustVelocity = -1;
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y - 20f), NPC.width + 40, NPC.height + 40, DustID.Blood, (float)(dustVelocity * 8), -1f, 0, default(Color), 1f);
                }
            }

            if (NPC.type >= NPCID.BloodEelHead && NPC.type <= NPCID.BloodEelTail)
            {
                NPC.position += NPC.netOffset;
                NPC.dontTakeDamage = (NPC.alpha > 0);
                if (NPC.type == NPCID.BloodEelHead || (NPC.type != NPCID.BloodEelHead && Main.npc[(int)NPC.ai[1]].alpha < 85))
                {
                    if (NPC.dontTakeDamage)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100);
                        }
                    }

                    NPC.alpha -= 42;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }

                if (NPC.alpha == 0 && Main.rand.NextBool(5))
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100);

                NPC.position -= NPC.netOffset;
            }
            else if (NPC.type == NPCID.StardustWormHead && NPC.ai[1] == 0f)
            {
                NPC.ai[1] = Main.rand.Next(-2, 0);
                NPC.netUpdate = true;
            }

            bool wormHead = NPC.type == NPCID.GiantWormHead || NPC.type == NPCID.BoneSerpentHead || NPC.type == NPCID.DiggerHead || NPC.type == NPCID.LeechHead || NPC.type == NPCID.DuneSplicerHead || (!Main.player[NPC.target].ZoneUndergroundDesert && NPC.type == NPCID.TombCrawlerHead);
            float acceleration = NPC.type == NPCID.TombCrawlerHead ? 0.1f : 0.2f;

            NPC.defense = (int)Math.Round(NPC.defDefense * 1.3);
            if (NPC.ai[3] > 0f)
                NPC.realLife = (int)NPC.ai[3];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || (wormHead && (double)Main.player[NPC.target].position.Y < Main.worldSurface * 16.0))
            {
                NPC.TargetClosest(true);
            }

            if (Main.player[NPC.target].dead || (wormHead && (double)Main.player[NPC.target].position.Y < Main.worldSurface * 16.0))
            {
                if (NPC.timeLeft > 300)
                {
                    NPC.timeLeft = 300;
                }
                if (wormHead)
                {
                    NPC.velocity.Y = NPC.velocity.Y + acceleration;
                }
            }

            if (NPC.type == NPCID.BloodEelHead && Main.dayTime)
            {
                NPC.EncourageDespawn(60);
                NPC.velocity.Y += 1f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.type == NPCID.WyvernHead && NPC.ai[0] == 0f)
                {
                    int maxParts = CalamityWorld.death ? 30 : 21;
                    NPC.ai[3] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int currentNPC = NPC.whoAmI;
                    for (int k = 0; k < maxParts; k++)
                    {
                        int wyvernSegmentType = NPCID.WyvernBody;
                        if (k == 1 || k == 12)
                        {
                            wyvernSegmentType = NPCID.WyvernLegs;
                        }
                        else if (k == maxParts - 3)
                        {
                            wyvernSegmentType = NPCID.WyvernBody2;
                        }
                        else if (k == maxParts - 2)
                        {
                            wyvernSegmentType = NPCID.WyvernBody3;
                        }
                        else if (k == maxParts - 1)
                        {
                            wyvernSegmentType = NPCID.WyvernTail;
                        }
                        int wyvernSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), wyvernSegmentType, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[wyvernSegment].ai[3] = (float)NPC.whoAmI;
                        Main.npc[wyvernSegment].realLife = NPC.whoAmI;
                        Main.npc[wyvernSegment].ai[1] = (float)currentNPC;
                        Main.npc[currentNPC].ai[0] = (float)wyvernSegment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, wyvernSegment, 0f, 0f, 0f, 0, 0, 0);
                        currentNPC = wyvernSegment;
                    }
                }

                if (NPC.type == NPCID.TombCrawlerHead && NPC.ai[0] == 0f)
                {
                    NPC.ai[3] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int currentTombCrawler = NPC.whoAmI;
                    int tombCrawlerSegments = Main.rand.Next(11, CalamityWorld.death ? 25 : 15);
                    SpawnBodyAndTailSegments(tombCrawlerSegments, NPCID.TombCrawlerBody, NPCID.TombCrawlerTail, ref currentTombCrawler);
                }

                if (NPC.type == NPCID.SolarCrawltipedeHead && NPC.ai[0] == 0f)
                {
                    NPC.ai[3] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int projTargetDistance = NPC.whoAmI;
                    int crawltipedeSegments = CalamityWorld.death ? 70 : 50;
                    SpawnBodyAndTailSegments(crawltipedeSegments, NPCID.SolarCrawltipedeBody, NPCID.SolarCrawltipedeTail, ref projTargetDistance);
                }

                if (NPC.type == NPCID.BloodEelHead && NPC.ai[0] == 0f)
                {
                    NPC.ai[3] = NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int bloodEelSegment = 0;
                    int currentBloodEel = NPC.whoAmI;
                    int bloodEelSegments = CalamityWorld.death ? 44 : 34;
                    for (int p = 0; p < bloodEelSegments; p++)
                    {
                        int bloodEelSegmentType = NPCID.BloodEelBody;
                        if (p == bloodEelSegments - 1)
                            bloodEelSegmentType = NPCID.BloodEelTail;

                        bloodEelSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), bloodEelSegmentType, NPC.whoAmI);
                        Main.npc[bloodEelSegment].ai[3] = NPC.whoAmI;
                        Main.npc[bloodEelSegment].realLife = NPC.whoAmI;
                        Main.npc[bloodEelSegment].ai[1] = currentBloodEel;
                        Main.npc[bloodEelSegment].CopyInteractions(NPC);
                        Main.npc[currentBloodEel].ai[0] = bloodEelSegment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bloodEelSegment);
                        currentBloodEel = bloodEelSegment;
                    }
                }
                else if ((NPC.type == NPCID.GiantWormHead || NPC.type == NPCID.GiantWormBody || NPC.type == NPCID.DevourerHead || NPC.type == NPCID.DevourerBody || NPC.type == NPCID.BoneSerpentHead || NPC.type == NPCID.BoneSerpentBody || NPC.type == NPCID.LeechHead || NPC.type == NPCID.LeechBody) && NPC.ai[0] == 0f)
                {
                    if (NPC.type == NPCID.GiantWormHead || NPC.type == NPCID.DevourerHead || NPC.type == NPCID.BoneSerpentHead || NPC.type == NPCID.LeechHead)
                    {
                        NPC.ai[3] = (float)NPC.whoAmI;
                        NPC.realLife = NPC.whoAmI;

                        switch (NPC.type)
                        {
                            case NPCID.DevourerHead:
                                NPC.ai[2] = (float)Main.rand.Next(13, CalamityWorld.death ? 30 : 19);
                                break;
                            case NPCID.GiantWormHead:
                                NPC.ai[2] = (float)Main.rand.Next(25, CalamityWorld.death ? 50 : 31);
                                break;
                            case NPCID.BoneSerpentHead:
                                NPC.ai[2] = (float)Main.rand.Next(16, CalamityWorld.death ? 33 : 23);
                                break;
                            case NPCID.DiggerHead:
                                NPC.ai[2] = (float)Main.rand.Next(12, CalamityWorld.death ? 27 : 18);
                                break;
                            case NPCID.SeekerHead:
                                NPC.ai[2] = (float)Main.rand.Next(27, CalamityWorld.death ? 45 : 33);
                                break;
                            case NPCID.LeechHead:
                                NPC.ai[2] = (float)Main.rand.Next(CalamityWorld.death ? 3 : 5, CalamityWorld.death ? 5 : 8);
                                break;
                            case NPCID.DuneSplicerHead:
                                NPC.ai[2] = (float)Main.rand.Next(15, CalamityWorld.death ? 35 : 24);
                                break;
                        }

                        NPC.ai[0] = (float)NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), NPC.type + 1, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }
                    else if ((NPC.type == NPCID.GiantWormBody || NPC.type == NPCID.DevourerBody || NPC.type == NPCID.BoneSerpentBody || NPC.type == NPCID.LeechBody) && NPC.ai[2] > 0f)
                    {
                        NPC.ai[0] = (float)NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), NPC.type, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }
                    else
                    {
                        NPC.ai[0] = (float)NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), NPC.type + 1, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }

                    Main.npc[(int)NPC.ai[0]].ai[3] = NPC.ai[3];
                    Main.npc[(int)NPC.ai[0]].realLife = NPC.realLife;
                    Main.npc[(int)NPC.ai[0]].ai[1] = (float)NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
                {
                    if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPC.aiStyle)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                }
                if (NPC.ai[0] > 0f && NPC.ai[0] < (float)Main.npc.Length)
                {
                    if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPC.aiStyle)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                }

                if (!NPC.active && Main.dedServ)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
            {
                tilePositionX = 0;
            }
            if (tileWidthPosX > Main.maxTilesX)
            {
                tileWidthPosX = Main.maxTilesX;
            }
            if (tilePositionY < 0)
            {
                tilePositionY = 0;
            }
            if (tileWidthPosY > Main.maxTilesY)
            {
                tileWidthPosY = Main.maxTilesY;
            }

            bool flying = false;
            if (NPC.type >= NPCID.WyvernHead && NPC.type <= NPCID.WyvernTail)
            {
                flying = true;
            }
            if (NPC.type == NPCID.StardustWormHead && NPC.ai[1] == -1f)
            {
                flying = true;
            }
            if (NPC.type >= NPCID.SolarCrawltipedeHead && NPC.type <= NPCID.SolarCrawltipedeTail)
            {
                flying = true;
            }
            if (NPC.type >= NPCID.BloodEelHead && NPC.type <= NPCID.BloodEelTail)
            {
                flying = true;
            }
            if (!flying)
            {
                for (int x = tilePositionX; x < tileWidthPosX; x++)
                {
                    for (int y = tilePositionY; y < tileWidthPosY; y++)
                    {
                        if (Main.tile[x, y] != null && ((Main.tile[x, y].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[x, y].TileType] || (Main.tileSolidTop[(int)Main.tile[x, y].TileType] && Main.tile[x, y].TileFrameY == 0))) || Main.tile[x, y].LiquidAmount > 64))
                        {
                            Vector2 flyingPos;
                            flyingPos.X = (float)(x * 16);
                            flyingPos.Y = (float)(y * 16);
                            if (NPC.position.X + (float)NPC.width > flyingPos.X && NPC.position.X < flyingPos.X + 16f && NPC.position.Y + (float)NPC.height > flyingPos.Y && NPC.position.Y < flyingPos.Y + 16f)
                            {
                                flying = true;
                                if (Main.rand.NextBool(100) && NPC.type != NPCID.LeechHead && Main.tile[x, y].HasUnactuatedTile)
                                {
                                    WorldGen.KillTile(x, y, true, true, false);
                                }
                            }
                        }
                    }
                }
            }

            if (!flying && (NPC.type == NPCID.GiantWormHead || NPC.type == NPCID.DevourerHead || NPC.type == NPCID.BoneSerpentHead || NPC.type == NPCID.LeechHead || NPC.type == NPCID.TombCrawlerHead))
            {
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int noFlyZone = 1000;
                bool outsideNoFlyZone = true;
                for (int f = 0; f < Main.maxPlayers; f++)
                {
                    if (Main.player[f].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[f].position.X - noFlyZone, (int)Main.player[f].position.Y - noFlyZone, noFlyZone * 2, noFlyZone * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            outsideNoFlyZone = false;
                            break;
                        }
                    }
                }
                if (outsideNoFlyZone)
                {
                    flying = true;
                }
            }

            if ((NPC.type >= NPCID.WyvernHead && NPC.type <= NPCID.WyvernTail) || (NPC.type >= NPCID.BloodEelHead && NPC.type <= NPCID.BloodEelTail))
            {
                if (NPC.velocity.X < 0f)
                {
                    NPC.spriteDirection = 1;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.spriteDirection = -1;
                }
            }

            if (NPC.type == NPCID.SolarCrawltipedeTail)
            {
                if (NPC.justHit)
                {
                    NPC.localAI[3] = 3f;
                }
                if (NPC.localAI[2] > 0f)
                {
                    NPC.localAI[2] -= 16f;
                    if (NPC.localAI[2] == 0f)
                    {
                        NPC.localAI[2] = -128f;
                    }
                }
                else if (NPC.localAI[2] < 0f)
                {
                    NPC.localAI[2] += 16f;
                }
                else if (NPC.localAI[3] > 0f)
                {
                    NPC.localAI[2] = 128f;
                    NPC.localAI[3] -= 1f;
                }
            }

            if (NPC.type == NPCID.SolarCrawltipedeHead)
            {
                Vector2 crawltipedeDustPos = NPC.Center + (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 8f;
                Vector2 crawltipedeDustRotation = NPC.rotation.ToRotationVector2() * 16f;
                Dust crawltipedeDust = Main.dust[Dust.NewDust(crawltipedeDustPos + crawltipedeDustRotation, 0, 0, DustID.Torch, NPC.velocity.X, NPC.velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 3f)];
                crawltipedeDust.noGravity = true;
                crawltipedeDust.noLight = true;
                crawltipedeDust.position -= new Vector2(4f);
                crawltipedeDust.fadeIn = 1f;
                crawltipedeDust.velocity = Vector2.Zero;
                Dust crawltipedeDust2 = Main.dust[Dust.NewDust(crawltipedeDustPos - crawltipedeDustRotation, 0, 0, DustID.Torch, NPC.velocity.X, NPC.velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 3f)];
                crawltipedeDust2.noGravity = true;
                crawltipedeDust2.noLight = true;
                crawltipedeDust2.position -= new Vector2(4f);
                crawltipedeDust2.fadeIn = 1f;
                crawltipedeDust2.velocity = Vector2.Zero;
            }

            float wormSpeed = 10f;
            float wormAccel = 0.09f;
            if (NPC.type == NPCID.DiggerHead)
            {
                wormSpeed = 6.5f;
                wormAccel = 0.05f;
            }
            if (NPC.type == NPCID.GiantWormHead)
            {
                wormSpeed = 7.5f;
                wormAccel = 0.06f;
            }
            if (NPC.type == NPCID.TombCrawlerHead)
            {
                wormSpeed = 8f;
                wormAccel = 0.13f;
            }
            if (NPC.type == NPCID.DuneSplicerHead)
            {
                if (!Main.player[NPC.target].dead && Main.player[NPC.target].ZoneSandstorm)
                {
                    wormSpeed = 16f;
                    wormAccel = 0.35f;
                }
                else
                {
                    wormAccel = 0.25f;
                }
            }
            if (NPC.type == NPCID.WyvernHead)
            {
                wormSpeed = 11f;
                wormAccel = 0.3f;
            }
            if (NPC.type == NPCID.StardustWormHead)
            {
                wormSpeed = 9f;
                wormAccel = 0.25f;
            }
            if (NPC.type == NPCID.LeechHead && Main.wofNPCIndex >= 0)
            {
                float lifeRatio = (float)Main.npc[Main.wofNPCIndex].life / (float)Main.npc[Main.wofNPCIndex].lifeMax;
                if (lifeRatio < 0.75f)
                {
                    wormSpeed += 1f;
                    wormAccel += 0.1f;
                }
                if (lifeRatio < 0.5f)
                {
                    wormSpeed += 1f;
                    wormAccel += 0.1f;
                }
                if (lifeRatio < 0.25f)
                {
                    wormSpeed += 2f;
                    wormAccel += 0.1f;
                }
            }
            if (NPC.type == NPCID.BloodEelHead)
            {
                wormSpeed = 18f;
                wormAccel = 0.6f;
            }

            if (CalamityWorld.death)
            {
                wormSpeed *= 1.25f;
                wormAccel *= 1.25f;
            }

            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float wormTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float wormTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);

            if (NPC.type == NPCID.SolarCrawltipedeHead)
            {
                wormSpeed = 12f;
                wormAccel = 0.32f;
                int crawltipedeTargetY = -1;
                int targetTileX = (int)(Main.player[NPC.target].Center.X / 16f);
                int targetTileY = (int)(Main.player[NPC.target].Center.Y / 16f);
                for (int i = targetTileX - 2; i <= targetTileX + 2; i++)
                {
                    for (int j = targetTileY; j <= targetTileY + 15; j++)
                    {
                        if (WorldGen.SolidTile2(i, j))
                        {
                            crawltipedeTargetY = j;
                            break;
                        }
                    }
                    if (crawltipedeTargetY > 0)
                    {
                        break;
                    }
                }
                if (crawltipedeTargetY > 0)
                {
                    crawltipedeTargetY *= 16;
                    float crawltipedeYTarget = (float)(crawltipedeTargetY - 800);
                    if (Main.player[NPC.target].position.Y > crawltipedeYTarget)
                    {
                        wormTargetY = crawltipedeYTarget;
                        if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) < 500f)
                        {
                            if (NPC.velocity.X > 0f)
                            {
                                wormTargetX = Main.player[NPC.target].Center.X + 600f;
                            }
                            else
                            {
                                wormTargetX = Main.player[NPC.target].Center.X - 600f;
                            }
                        }
                    }
                }
                else
                {
                    wormSpeed = 28f;
                    wormAccel = 0.8f;
                }
                float maxWormSpeed = wormSpeed * 1.3f;
                float minWormSpeed = wormSpeed * 0.7f;
                float velocityCheck = NPC.velocity.Length();
                if (velocityCheck > 0f)
                {
                    if (velocityCheck > maxWormSpeed)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= maxWormSpeed;
                    }
                    else if (velocityCheck < minWormSpeed)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= minWormSpeed;
                    }
                }
                if (crawltipedeTargetY > 0)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (Main.npc[k].active && Main.npc[k].type == NPC.type && k != NPC.whoAmI)
                        {
                            Vector2 targetDirection = Main.npc[k].Center - NPC.Center;
                            if (targetDirection.Length() < 400f)
                            {
                                targetDirection.Normalize();
                                targetDirection *= 1000f;
                                wormTargetX -= targetDirection.X;
                                wormTargetY -= targetDirection.Y;
                            }
                        }
                    }
                }
                else
                {
                    for (int l = 0; l < Main.maxNPCs; l++)
                    {
                        if (Main.npc[l].active && Main.npc[l].type == NPC.type && l != NPC.whoAmI)
                        {
                            Vector2 idleDirection = Main.npc[l].Center - NPC.Center;
                            if (idleDirection.Length() < 60f)
                            {
                                idleDirection.Normalize();
                                idleDirection *= 200f;
                                wormTargetX -= idleDirection.X;
                                wormTargetY -= idleDirection.Y;
                            }
                        }
                    }
                }
            }

            wormTargetX = (float)((int)(wormTargetX / 16f) * 16);
            wormTargetY = (float)((int)(wormTargetY / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            wormTargetX -= segmentPosition.X;
            wormTargetY -= segmentPosition.Y;
            float wormTargetDist = (float)Math.Sqrt((double)(wormTargetX * wormTargetX + wormTargetY * wormTargetY));

            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    wormTargetX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - segmentPosition.X;
                    wormTargetY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - segmentPosition.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)Math.Atan2((double)wormTargetY, (double)wormTargetX) + MathHelper.PiOver2;
                wormTargetDist = (float)Math.Sqrt((double)(wormTargetX * wormTargetX + wormTargetY * wormTargetY));
                int segmentWidth = NPC.width;
                if (NPC.type >= NPCID.WyvernHead && NPC.type <= NPCID.WyvernTail)
                {
                    segmentWidth = 42;
                }
                if (NPC.type >= NPCID.SolarCrawltipedeHead && NPC.type <= NPCID.SolarCrawltipedeTail)
                {
                    segmentWidth += 6;
                }
                if (NPC.type >= NPCID.BloodEelHead && NPC.type <= NPCID.BloodEelTail)
                {
                    segmentWidth = 24;
                }
                wormTargetDist = (wormTargetDist - (float)segmentWidth) / wormTargetDist;
                wormTargetX *= wormTargetDist;
                wormTargetY *= wormTargetDist;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + wormTargetX;
                NPC.position.Y = NPC.position.Y + wormTargetY;
                if ((NPC.type >= NPCID.WyvernHead && NPC.type <= NPCID.WyvernTail) || (NPC.type >= NPCID.BloodEelHead && NPC.type <= NPCID.BloodEelTail))
                {
                    if (wormTargetX < 0f)
                    {
                        NPC.spriteDirection = 1;
                    }
                    else if (wormTargetX > 0f)
                    {
                        NPC.spriteDirection = -1;
                    }
                }
            }
            else
            {
                if (!flying)
                {
                    NPC.TargetClosest(true);
                    NPC.velocity.Y = NPC.velocity.Y + 0.11f;
                    if (NPC.velocity.Y > wormSpeed)
                    {
                        NPC.velocity.Y = wormSpeed;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)wormSpeed * 0.4)
                    {
                        if (NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - wormAccel * 1.1f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X + wormAccel * 1.1f;
                        }
                    }
                    else if (NPC.velocity.Y == wormSpeed)
                    {
                        if (NPC.velocity.X < wormTargetX)
                        {
                            NPC.velocity.X = NPC.velocity.X + wormAccel;
                        }
                        else if (NPC.velocity.X > wormTargetX)
                        {
                            NPC.velocity.X = NPC.velocity.X - wormAccel;
                        }
                    }
                    else if (NPC.velocity.Y > 4f)
                    {
                        if (NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + wormAccel * 0.9f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - wormAccel * 0.9f;
                        }
                    }
                }
                else
                {
                    if (NPC.type != NPCID.WyvernHead && NPC.type != NPCID.LeechHead && NPC.type != NPCID.SolarCrawltipedeHead && NPC.type != NPCID.BloodEelHead && NPC.soundDelay == 0)
                    {
                        float soundDelay = wormTargetDist / 40f;
                        if (soundDelay < 10f)
                        {
                            soundDelay = 10f;
                        }
                        if (soundDelay > 20f)
                        {
                            soundDelay = 20f;
                        }
                        NPC.soundDelay = (int)soundDelay;
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                    }

                    wormTargetDist = (float)Math.Sqrt((double)(wormTargetX * wormTargetX + wormTargetY * wormTargetY));
                    float absoluteTargetX = Math.Abs(wormTargetX);
                    float absoluteTargetY = Math.Abs(wormTargetY);
                    float timeToReachTarget = wormSpeed / wormTargetDist;
                    wormTargetX *= timeToReachTarget;
                    wormTargetY *= timeToReachTarget;

                    bool wormShouldFlee = false;
                    if (NPC.type == NPCID.DevourerHead && ((!Main.player[NPC.target].ZoneCorrupt && !Main.player[NPC.target].ZoneCrimson) || Main.player[NPC.target].dead))
                    {
                        wormShouldFlee = true;
                    }
                    if ((NPC.type == NPCID.TombCrawlerHead && (double)Main.player[NPC.target].position.Y < Main.worldSurface * 16.0 && !Main.player[NPC.target].ZoneSandstorm && !Main.player[NPC.target].ZoneUndergroundDesert) || Main.player[NPC.target].dead)
                    {
                        wormShouldFlee = true;
                    }
                    if ((NPC.type == NPCID.DuneSplicerHead && (double)Main.player[NPC.target].position.Y < Main.worldSurface * 16.0 && !Main.player[NPC.target].ZoneSandstorm && !Main.player[NPC.target].ZoneUndergroundDesert) || Main.player[NPC.target].dead)
                    {
                        wormShouldFlee = true;
                    }
                    if (wormShouldFlee)
                    {
                        bool definitelyFlee = true;
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            if (Main.player[p].active && !Main.player[p].dead && Main.player[p].ZoneCorrupt)
                            {
                                definitelyFlee = false;
                            }
                        }
                        if (definitelyFlee)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && (double)(NPC.position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
                            {
                                NPC.active = false;
                                int q = (int)NPC.ai[0];
                                while (q > 0 && q < Main.maxNPCs && Main.npc[q].active && Main.npc[q].aiStyle == NPC.aiStyle)
                                {
                                    int differentSegment = (int)Main.npc[q].ai[0];
                                    Main.npc[q].active = false;
                                    NPC.life = 0;
                                    if (Main.dedServ)
                                    {
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, q, 0f, 0f, 0f, 0, 0, 0);
                                    }
                                    q = differentSegment;
                                }
                                if (Main.dedServ)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                            wormTargetX = 0f;
                            wormTargetY = wormSpeed;
                        }
                    }

                    bool shouldSwoopDown = false;
                    if (NPC.type == NPCID.WyvernHead)
                    {
                        if (((NPC.velocity.X > 0f && wormTargetX < 0f) || (NPC.velocity.X < 0f && wormTargetX > 0f) || (NPC.velocity.Y > 0f && wormTargetY < 0f) || (NPC.velocity.Y < 0f && wormTargetY > 0f)) && Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) > wormAccel / 2f && wormTargetDist < 300f)
                        {
                            shouldSwoopDown = true;

                            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < wormSpeed)
                                NPC.velocity *= 1.1f;
                        }
                        if (NPC.position.Y > Main.player[NPC.target].position.Y || (double)(Main.player[NPC.target].position.Y / 16f) > Main.worldSurface || Main.player[NPC.target].dead)
                        {
                            shouldSwoopDown = true;

                            if (Math.Abs(NPC.velocity.X) < wormSpeed / 2f)
                            {
                                if (NPC.velocity.X == 0f)
                                    NPC.velocity.X = NPC.velocity.X - NPC.direction;

                                NPC.velocity.X = NPC.velocity.X * 1.1f;
                            }
                            else if (NPC.velocity.Y > -wormSpeed)
                                NPC.velocity.Y = NPC.velocity.Y - wormAccel;
                        }
                    }

                    if (NPC.type == NPCID.BloodEelHead)
                    {
                        if (((NPC.velocity.X > 0f && wormTargetX < 0f) || (NPC.velocity.X < 0f && wormTargetX > 0f) || (NPC.velocity.Y > 0f && wormTargetY < 0f) || (NPC.velocity.Y < 0f && wormTargetY > 0f)) && Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) > wormAccel / 2f && wormTargetDist < 120f)
                        {
                            shouldSwoopDown = true;
                            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < wormSpeed)
                                NPC.velocity *= 1.1f;
                        }
                        if (NPC.position.Y > Main.player[NPC.target].position.Y || Main.player[NPC.target].dead)
                        {
                            shouldSwoopDown = true;
                            if (Math.Abs(NPC.velocity.X) < wormSpeed / 2f)
                            {
                                if (NPC.velocity.X == 0f)
                                    NPC.velocity.X -= NPC.direction;

                                NPC.velocity.X *= 1.1f;
                            }
                            else if (NPC.velocity.Y > 0f - wormSpeed)
                                NPC.velocity.Y -= wormAccel;
                        }
                    }

                    if (!shouldSwoopDown)
                    {
                        if ((NPC.velocity.X > 0f && wormTargetX > 0f) || (NPC.velocity.X < 0f && wormTargetX < 0f) || (NPC.velocity.Y > 0f && wormTargetY > 0f) || (NPC.velocity.Y < 0f && wormTargetY < 0f))
                        {
                            if (NPC.velocity.X < wormTargetX)
                                NPC.velocity.X = NPC.velocity.X + wormAccel;
                            else if (NPC.velocity.X > wormTargetX)
                                NPC.velocity.X = NPC.velocity.X - wormAccel;

                            if (NPC.velocity.Y < wormTargetY)
                                NPC.velocity.Y = NPC.velocity.Y + wormAccel;
                            else if (NPC.velocity.Y > wormTargetY)
                                NPC.velocity.Y = NPC.velocity.Y - wormAccel;

                            if ((double)Math.Abs(wormTargetY) < (double)wormSpeed * 0.2 && ((NPC.velocity.X > 0f && wormTargetX < 0f) || (NPC.velocity.X < 0f && wormTargetX > 0f)))
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y = NPC.velocity.Y + wormAccel * 2f;
                                else
                                    NPC.velocity.Y = NPC.velocity.Y - wormAccel * 2f;
                            }

                            if ((double)Math.Abs(wormTargetX) < (double)wormSpeed * 0.2 && ((NPC.velocity.Y > 0f && wormTargetY < 0f) || (NPC.velocity.Y < 0f && wormTargetY > 0f)))
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X = NPC.velocity.X + wormAccel * 2f;
                                else
                                    NPC.velocity.X = NPC.velocity.X - wormAccel * 2f;
                            }
                        }
                        else if (absoluteTargetX > absoluteTargetY)
                        {
                            if (NPC.velocity.X < wormTargetX)
                                NPC.velocity.X = NPC.velocity.X + wormAccel * 1.1f;
                            else if (NPC.velocity.X > wormTargetX)
                                NPC.velocity.X = NPC.velocity.X - wormAccel * 1.1f;

                            if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)wormSpeed * 0.5)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y = NPC.velocity.Y + wormAccel;
                                else
                                    NPC.velocity.Y = NPC.velocity.Y - wormAccel;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < wormTargetY)
                                NPC.velocity.Y = NPC.velocity.Y + wormAccel * 1.1f;
                            else if (NPC.velocity.Y > wormTargetY)
                                NPC.velocity.Y = NPC.velocity.Y - wormAccel * 1.1f;

                            if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)wormSpeed * 0.5)
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X = NPC.velocity.X + wormAccel;
                                else
                                    NPC.velocity.X = NPC.velocity.X - wormAccel;
                            }
                        }
                    }
                }

                NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + MathHelper.PiOver2;
                if (NPC.type == NPCID.DevourerHead || NPC.type == NPCID.GiantWormHead || NPC.type == NPCID.BoneSerpentHead || NPC.type == NPCID.DiggerHead || NPC.type == NPCID.SeekerHead || NPC.type == NPCID.LeechHead || NPC.type == NPCID.DuneSplicerHead || NPC.type == NPCID.TombCrawlerHead)
                {
                    if (flying)
                    {
                        if (NPC.localAI[0] != 1f)
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 0f;
                    }

                    if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                    {
                        NPC.netUpdate = true;
                    }
                }
            }
            return false;
        }
    }
}
