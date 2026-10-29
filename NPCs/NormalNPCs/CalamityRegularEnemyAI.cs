using System;
using CalamityMod.Dusts;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public static class CalamityRegularEnemyAI
    {
        #region Gem Crawler AI
        public static void GemCrawlerAI(NPC npc, Mod mod, float speedDetect, float speedAdditive)
        {
            var turnAroundDelay = 30;
            var unusedFlag = false;
            var isRunning = false;
            var shouldTurnAround = false;
            if (npc.velocity.Y == 0f && (npc.velocity.X > 0f && npc.direction > 0 || npc.velocity.X < 0f && npc.direction < 0))
            {
                isRunning = true;
                npc.ai[3] += 1f;
            }
            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= turnAroundDelay) | isRunning)
            {
                npc.ai[3] += 1f;
                shouldTurnAround = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > turnAroundDelay * 10)
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == turnAroundDelay)
            {
                npc.netUpdate = true;
            }
            var npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            var xDist = Main.player[npc.target].Center.X - npcPos.X;
            var yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            var targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !shouldTurnAround)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] < turnAroundDelay)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = -npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            var maxVelocity = speedDetect; //5
            var acceleration = speedAdditive; //0.05
            if (!unusedFlag && (npc.velocity.Y == 0f || npc.wet || npc.velocity.X <= 0f && npc.direction > 0 || npc.velocity.X >= 0f && npc.direction < 0))
            {
                if (npc.velocity.X < -maxVelocity || npc.velocity.X > maxVelocity)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < maxVelocity && npc.direction == -1)
                {
                    npc.velocity.X = npc.velocity.X + acceleration;
                    if (npc.velocity.X > maxVelocity)
                    {
                        npc.velocity.X = maxVelocity;
                    }
                }
                else if (npc.velocity.X > -maxVelocity && npc.direction == 1)
                {
                    npc.velocity.X = npc.velocity.X - acceleration;
                    if (npc.velocity.X < -maxVelocity)
                    {
                        npc.velocity.X = -maxVelocity;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                var faceDirection = 0;
                if (npc.velocity.X < 0f)
                {
                    faceDirection = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    faceDirection = 1;
                }
                var position = npc.position;
                position.X += npc.velocity.X;
                var x = (int)((position.X + npc.width / 2 + (npc.width / 2 + 1) * faceDirection) / 16f);
                var y = (int)((position.Y + npc.height - 1f) / 16f);
                if (x * 16 < position.X + npc.width && x * 16 + 16 > position.X && (Main.tile[x, y].HasUnactuatedTile && !Main.tile[x, y].TopSlope && !Main.tile[x, y - 1].TopSlope && Main.tileSolid[Main.tile[x, y].TileType] && !Main.tileSolidTop[Main.tile[x, y].TileType] || Main.tile[x, y - 1].IsHalfBlock && Main.tile[x, y - 1].HasUnactuatedTile) && (!Main.tile[x, y - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[x, y - 1].TileType] || Main.tileSolidTop[Main.tile[x, y - 1].TileType] || Main.tile[x, y - 1].IsHalfBlock && (!Main.tile[x, y - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[x, y - 4].TileType] || Main.tileSolidTop[Main.tile[x, y - 4].TileType])) && (!Main.tile[x, y - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[x, y - 2].TileType] || Main.tileSolidTop[Main.tile[x, y - 2].TileType]) && (!Main.tile[x, y - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[x, y - 3].TileType] || Main.tileSolidTop[Main.tile[x, y - 3].TileType]) && (!Main.tile[x - faceDirection, y - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[x - faceDirection, y - 3].TileType]))
                {
                    var npcBottom = (float)(y * 16);
                    if (Main.tile[x, y].IsHalfBlock)
                    {
                        npcBottom += 8f;
                    }
                    if (Main.tile[x, y - 1].IsHalfBlock)
                    {
                        npcBottom -= 8f;
                    }
                    if (npcBottom < position.Y + npc.height)
                    {
                        var percentageTileRisen = position.Y + npc.height - npcBottom;
                        if (percentageTileRisen <= 16.1f)
                        {
                            npc.gfxOffY += npc.position.Y + npc.height - npcBottom;
                            npc.position.Y = npcBottom - npc.height;
                            if (percentageTileRisen < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Dungeon Spirit AI
        public static void DungeonSpiritAI(NPC npc, Mod mod, float speed, float rotation, bool lantern = false)
        {
            npc.TargetClosest(true);
            var npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            var xDist = Main.player[npc.target].Center.X - npcPos.X;
            var yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            var targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            var homingSpeed = speed;

            if (lantern)
            {
                if (npc.localAI[0] < 85f)
                {
                    homingSpeed = 0.1f;
                    targetDist = homingSpeed / targetDist;
                    xDist *= targetDist;
                    yDist *= targetDist;
                    npc.velocity = (npc.velocity * 100f + new Vector2(xDist, yDist)) / 101f;
                    npc.localAI[0] += 1f;
                    return;
                }

                npc.dontTakeDamage = false;
            }

            targetDist = homingSpeed / targetDist;
            xDist *= targetDist;
            yDist *= targetDist;
            npc.velocity.X = (npc.velocity.X * 100f + xDist) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + yDist) / 101f;

            if (lantern)
            {
                npc.rotation = npc.velocity.X * 0.08f;
                npc.spriteDirection = npc.direction > 0 ? 1 : -1;
            }
            else
                npc.rotation = (float)Math.Atan2((double)yDist, (double)xDist) + rotation;
        }
        #endregion

        #region Unicorn AI
        public static void UnicornAI(NPC npc, Mod mod, bool spin, float bounciness, float speedDetect, float speedAdditive, float bouncy1 = -8.5f, float bouncy2 = -7.5f, float bouncy3 = -7f, float bouncy4 = -6f, float bouncy5 = -8f)
        {
            var DogPhase1 = npc.type == ModContent.NPCType<Rimehound>() && npc.life > npc.lifeMax * (CalamityWorld.death ? 0.9 : CalamityWorld.revenge ? 0.7 : 0.5);
            var DogPhase2 = npc.type == ModContent.NPCType<Rimehound>() && npc.life <= npc.lifeMax * (CalamityWorld.death ? 0.9 : CalamityWorld.revenge ? 0.7 : 0.5);
            var turnAroundDelay = 30;
            var isRunning = false;
            var shouldTurnAround = false;
            if (npc.velocity.Y == 0f && (npc.velocity.X > 0f && npc.direction < 0 || npc.velocity.X < 0f && npc.direction > 0))
            {
                isRunning = true;
                npc.ai[3] += 1f;
            }
            var turnAroundDelayMult = DogPhase1 ? 10 : 4;
            if (!DogPhase1)
            {
                var noYVelocity = npc.velocity.Y == 0f;
                for (var i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < npc.width)
                    {
                        if (npc.position.X < Main.npc[i].position.X)
                        {
                            npc.velocity.X -= 0.05f;
                        }
                        else
                        {
                            npc.velocity.X += 0.05f;
                        }
                        if (npc.position.Y < Main.npc[i].position.Y)
                        {
                            npc.velocity.Y -= 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y += 0.05f;
                        }
                    }
                }
                if (noYVelocity)
                {
                    npc.velocity.Y = 0f;
                }
            }
            if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= turnAroundDelay || isRunning)
            {
                npc.ai[3] += 1f;
                shouldTurnAround = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > turnAroundDelay * turnAroundDelayMult)
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == turnAroundDelay)
            {
                npc.netUpdate = true;
            }
            var npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            var xDist = Main.player[npc.target].Center.X - npcPos.X;
            var yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            var targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !shouldTurnAround)
            {
                npc.ai[3] = 0f;
            }
            if (!DogPhase1)
            {
                if (npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && (npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f || npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f))
                {
                    npc.velocity.Y -= bounciness;
                    if (npc.type == ModContent.NPCType<DespairStone>())
                    {
                        SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                        for (var k = 0; k < 10; k++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, -1f, 0, default, 1f);
                        }
                        if (Main.zenithWorld)
                        {
                            var screenShakePower = 2 * Utils.GetLerpValue(1300f, 0f, npc.Distance(Main.LocalPlayer.Center), true);
                            Main.LocalPlayer.SetScreenshake(screenShakePower);
                        }
                    }
                    if (npc.type == ModContent.NPCType<Bohldohr>())
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit7, npc.Center);
                    }
                    if (DogPhase2)
                    {
                        for (var k = 0; k < 5; k++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Water, 0f, -1f, 0, default, 1f);
                        }
                    }
                    if (npc.type == ModContent.NPCType<AquaticUrchin>())
                    {
                        for (var k = 0; k < 5; k++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Water, 0f, -1f, 0, default, 1f);
                        }
                    }
                }
            }
            if (npc.ai[3] < turnAroundDelay)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            if (npc.velocity.Y == 0f || npc.wet || npc.velocity.X <= 0f && npc.direction < 0 || npc.velocity.X >= 0f && npc.direction > 0)
            {
                if (Math.Sign(npc.velocity.X) != npc.direction && !DogPhase1)
                {
                    npc.velocity.X *= 0.92f;
                }
                var sandstormPush = MathHelper.Lerp(0.6f, 1f, Math.Abs(Main.windSpeedCurrent)) * Math.Sign(Main.windSpeedCurrent);
                if (!Main.player[npc.target].ZoneSandstorm)
                {
                    sandstormPush = 0f;
                }
                var maxVelocity = speedDetect;
                var acceleration = speedAdditive;
                if (npc.velocity.X < -maxVelocity || npc.velocity.X > maxVelocity)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < maxVelocity && npc.direction == 1)
                {
                    npc.velocity.X += acceleration;
                    if (npc.velocity.X > maxVelocity)
                    {
                        npc.velocity.X = maxVelocity;
                    }
                }
                else if (npc.velocity.X > -maxVelocity && npc.direction == -1)
                {
                    npc.velocity.X -= acceleration;
                    if (npc.velocity.X < -maxVelocity)
                    {
                        npc.velocity.X = -maxVelocity;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                var faceDirection = 0;
                if (npc.velocity.X < 0f)
                {
                    faceDirection = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    faceDirection = 1;
                }
                var position = npc.position;
                position.X += npc.velocity.X;
                var x = (int)((position.X + npc.width / 2 + (npc.width / 2 + 1) * faceDirection) / 16f);
                var y = (int)((position.Y + npc.height - 1f) / 16f);

                // 21JAN2023: Ozzatron -- this is probably the single most unmaintainable if statement I've ever seen
                // I have broken all the individual statements out even though I do not fully understand the code.
                var t_xy = CalamityUtils.ParanoidTileRetrieval(x, y);
                var t_xy1 = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                var t_xy2 = CalamityUtils.ParanoidTileRetrieval(x, y - 2);
                var t_xy3 = CalamityUtils.ParanoidTileRetrieval(x, y - 3);
                var t_xOffY3 = CalamityUtils.ParanoidTileRetrieval(x - faceDirection, y - 3); // 3 down, offset 1 in the direction of the NPC's movement
                var t_xy4 = CalamityUtils.ParanoidTileRetrieval(x, y - 4);
                var positionCheck = x * 16 < position.X + npc.width && x * 16 + 16 > position.X;
                var tileSolidityCheck1 = t_xy.HasUnactuatedTile && !t_xy.TopSlope && !t_xy1.TopSlope && Main.tileSolid[t_xy.TileType] && !Main.tileSolidTop[t_xy.TileType];
                var oneBelowIsSolidHalf = t_xy1.IsHalfBlock && t_xy1.HasUnactuatedTile;
                var canFallThrough = !t_xy1.HasUnactuatedTile || !Main.tileSolid[t_xy1.TileType] || Main.tileSolidTop[t_xy1.TileType] || t_xy1.IsHalfBlock && (!t_xy4.HasUnactuatedTile || !Main.tileSolid[t_xy4.TileType] || Main.tileSolidTop[t_xy4.TileType]);
                var twoDownIsNonSolid = !t_xy2.HasUnactuatedTile || !Main.tileSolid[t_xy2.TileType] || Main.tileSolidTop[t_xy2.TileType];
                var threeDownIsNonSolid = !t_xy3.HasUnactuatedTile || !Main.tileSolid[t_xy3.TileType] || Main.tileSolidTop[t_xy3.TileType];
                // Notice it doesn't check for platforms in ther offset position. This is why walking AIs twirl on 1-wide platforms in hellevators. They have to turn around before this check succeeds.
                var threeDownOffsetIsNonSolid = !t_xOffY3.HasUnactuatedTile || !Main.tileSolid[t_xOffY3.TileType];
                if (positionCheck && (tileSolidityCheck1 || oneBelowIsSolidHalf) && canFallThrough && twoDownIsNonSolid && threeDownIsNonSolid && threeDownOffsetIsNonSolid)
                {
                    var tilePixelPosition = (float)(y * 16);
                    if (Main.tile[x, y].IsHalfBlock)
                    {
                        tilePixelPosition += 8f;
                    }
                    if (Main.tile[x, y - 1].IsHalfBlock)
                    {
                        tilePixelPosition -= 8f;
                    }
                    if (tilePixelPosition < position.Y + npc.height)
                    {
                        var percentageTileRisen = position.Y + npc.height - tilePixelPosition;
                        if ((double)percentageTileRisen <= 16.1)
                        {
                            npc.gfxOffY += npc.position.Y + npc.height - tilePixelPosition;
                            npc.position.Y = tilePixelPosition - npc.height;
                            if (percentageTileRisen < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (npc.velocity.Y == 0f)
            {
                var npcTileX = (int)((npc.position.X + npc.width / 2 + (npc.width / 2 + 2) * npc.direction + npc.velocity.X * 5f) / 16f);
                var npcTileY = (int)((npc.position.Y + npc.height - 15f) / 16f);
                var spriteDirection = npc.spriteDirection;
                spriteDirection *= -1;
                if (npc.velocity.X < 0f && spriteDirection == -1 || npc.velocity.X > 0f && spriteDirection == 1)
                {
                    if (Main.tile[npcTileX, npcTileY - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[npcTileX, npcTileY - 2].TileType])
                    {
                        if (Main.tile[npcTileX, npcTileY - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[npcTileX, npcTileY - 3].TileType])
                        {
                            npc.velocity.Y = bouncy1;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity.Y = bouncy2;
                            npc.netUpdate = true;
                        }
                    }
                    else if (Main.tile[npcTileX, npcTileY - 1].HasUnactuatedTile && !Main.tile[npcTileX, npcTileY - 1].TopSlope && Main.tileSolid[Main.tile[npcTileX, npcTileY - 1].TileType])
                    {
                        npc.velocity.Y = bouncy3;
                        npc.netUpdate = true;
                    }
                    else if (npc.position.Y + npc.height - npcTileY * 16 > 20f && Main.tile[npcTileX, npcTileY].HasUnactuatedTile && !Main.tile[npcTileX, npcTileY].TopSlope && Main.tileSolid[Main.tile[npcTileX, npcTileY].TileType])
                    {
                        npc.velocity.Y = bouncy4;
                        npc.netUpdate = true;
                    }
                    else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > 3f) && (!Main.tile[npcTileX, npcTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[npcTileX, npcTileY + 1].TileType]) && (!Main.tile[npcTileX, npcTileY + 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[npcTileX, npcTileY + 2].TileType]) && (!Main.tile[npcTileX + npc.direction, npcTileY + 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[npcTileX + npc.direction, npcTileY + 3].TileType]))
                    {
                        npc.velocity.Y = bouncy5;
                        npc.netUpdate = true;
                    }
                }
            }
            if (spin)
            {
                npc.rotation += npc.velocity.X * 0.05f;
                npc.spriteDirection = -npc.direction;
            }
        }
        #endregion

        #region Swimming AI
        // Passiveness:
        // 0 = Catfish, Flounder, Frogfish, Viperfish, Moray Eel, Laserfish
        // 1 = Blinded Angler
        // 2 = Prism-Back, Toxic Minnow
        // 3 = Sea Minnow
        public static void PassiveSwimmingAI(NPC npc, Mod mod, int passiveness, float detectRange, float xSpeed, float ySpeed, float speedLimitX, float speedLimitY, float rotation, bool spriteFacesLeft = true)
        {
            if (spriteFacesLeft)
                npc.spriteDirection = npc.direction > 0 ? 1 : -1;
            else
                npc.spriteDirection = npc.direction > 0 ? -1 : 1;

            npc.noGravity = true;
            if (npc.direction == 0)
                npc.TargetClosest();

            var target = Main.player[npc.target];
            if (npc.justHit && passiveness != 3)
                npc.chaseable = true;

            if (npc.wet)
            {
                var hasWetTarget = npc.chaseable;
                npc.TargetClosest(false);
                target = Main.player[npc.target];

                // Player detection behavior
                if (passiveness != 2)
                {
                    if (npc.type == ModContent.NPCType<Frogfish>())
                    {
                        if (target.wet && !target.dead)
                        {
                            hasWetTarget = true;
                            npc.chaseable = true; // Once the enemy has detected the player, let minions fuck it up
                        }
                    }

                    if (npc.type == ModContent.NPCType<Sulflounder>())
                    {
                        if (!target.dead)
                        {
                            hasWetTarget = true;
                            npc.chaseable = true; // Once the enemy has detected the player, let minions fuck it up
                        }
                    }
                    else if (target.wet && !target.dead && (target.Center - npc.Center).Length() < detectRange && Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                    {
                        hasWetTarget = true;
                        npc.chaseable = true; // Once the enemy has detected the player, let minions fuck it up
                    }
                    else
                    {
                        if (passiveness == 1)
                            hasWetTarget = false;
                    }
                }

                if ((target.dead || !Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height)) && hasWetTarget)
                    hasWetTarget = false;

                // Swim back and forth
                if (!hasWetTarget || passiveness == 2)
                {
                    if (passiveness == 0)
                    {
                        npc.TargetClosest(false);
                        target = Main.player[npc.target];
                    }

                    if (npc.collideX)
                    {
                        npc.velocity.X *= -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }

                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }

                if (hasWetTarget && passiveness != 2)
                {
                    npc.TargetClosest();
                    target = Main.player[npc.target];

                    // Swim away from the player
                    if (passiveness == 3)
                    {
                        npc.velocity.X = npc.velocity.X - npc.direction * xSpeed;
                        npc.velocity.Y = npc.velocity.Y - npc.directionY * ySpeed;
                    }
                    // Swim toward the player
                    else
                    {
                        npc.velocity.X = npc.velocity.X + npc.direction * (CalamityWorld.death ? 2f * xSpeed : CalamityWorld.revenge ? 1.5f * xSpeed : xSpeed);
                        npc.velocity.Y = npc.velocity.Y + npc.directionY * (CalamityWorld.death ? 2f * ySpeed : CalamityWorld.revenge ? 1.5f * ySpeed : ySpeed);
                    }

                    var velocityCapX = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitX : CalamityWorld.revenge ? 1.5f * speedLimitX : speedLimitX;
                    var velocityCapY = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitY : CalamityWorld.revenge ? 1.5f * speedLimitY : speedLimitY;
                    npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -velocityCapX, velocityCapX);
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -velocityCapY, velocityCapY);

                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    // Laserfish shoot the player
                    if (npc.type == ModContent.NPCType<Laserfish>())
                    {
                        npc.localAI[0] += CalamityWorld.death ? 2f : CalamityWorld.revenge ? 1.5f : 1f;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
                        {
                            npc.localAI[0] = 0f;
                            if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                            {
                                var speed = 5f;
                                var vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height / 2);
                                var velX = target.Center.X - vector.X + Main.rand.NextFloat(-20f, 20f);
                                var velY = target.Center.Y - vector.Y + Main.rand.NextFloat(-20f, 20f);
                                var dist = (float)Math.Sqrt((double)(velX * velX + velY * velY));
                                dist = speed / dist;
                                velX *= dist;
                                velY *= dist;
                                var damage = Main.masterMode ? 34 : Main.expertMode ? 40 : 50;
                                var beam = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (npc.spriteDirection == 1 ? 25f : -25f), npc.Center.Y + (target.position.Y > npc.Center.Y ? 5f : -5f), velX, velY, ProjectileID.EyeBeam, damage, 0f, Main.myPlayer);
                                Main.projectile[beam].tileCollide = true;
                            }
                        }
                    }

                    // Flounder shoot Sulphuric Mist at the player
                    if (npc.type == ModContent.NPCType<Sulflounder>())
                    {
                        if ((target.Center - npc.Center).Length() < 350f)
                        {
                            npc.localAI[0] += CalamityWorld.death ? 3f : CalamityWorld.revenge ? 2f : 1f;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 180f)
                            {
                                npc.localAI[0] = 0f;
                                if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                                {
                                    var speed = 4f;
                                    var vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height / 2);
                                    var velX = target.Center.X - vector.X + Main.rand.NextFloat(-20f, 20f);
                                    var velY = target.Center.Y - vector.Y + Main.rand.NextFloat(-20f, 20f);
                                    var dist = (float)Math.Sqrt((double)(velX * velX + velY * velY));
                                    dist = speed / dist;
                                    velX *= dist;
                                    velY *= dist;
                                    var damage = Main.masterMode ? 21 : Main.expertMode ? 25 : 35;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (npc.spriteDirection == 1 ? 10f : -10f), npc.Center.Y, velX, velY, ModContent.ProjectileType<SulphuricAcidMist>(), damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    // Sea Minnows face away from the player
                    if (npc.type == ModContent.NPCType<SeaMinnow>())
                        npc.direction *= -1;
                }
                else
                {
                    // No target behavior
                    npc.velocity.X += npc.direction * 0.1f;
                    if (npc.velocity.X < -2.5f || npc.velocity.X > 2.5f)
                        npc.velocity.X *= 0.95f;

                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y -= 0.01f;
                        if (npc.velocity.Y < -0.3f)
                            npc.ai[0] = 1f;
                    }
                    else
                    {
                        npc.velocity.Y += 0.01f;
                        if (npc.velocity.Y > 0.3f)
                            npc.ai[0] = -1f;
                    }
                }

                var npcTileX = (int)(npc.position.X + npc.width / 2) / 16;
                var npcTileY = (int)(npc.position.Y + npc.height / 2) / 16;
                if (Main.tile[npcTileX, npcTileY - 1].LiquidAmount > 128)
                {
                    if (Main.tile[npcTileX, npcTileY + 1].HasTile)
                        npc.ai[0] = -1f;
                    else if (Main.tile[npcTileX, npcTileY + 2].HasTile)
                        npc.ai[0] = -1f;
                }

                if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                    npc.velocity.Y *= 0.95f;
            }
            else
            {
                // Out of water behavior
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.94f;
                    if (npc.velocity.X > -0.2f && npc.velocity.X < 0.2f)
                        npc.velocity.X = 0f;
                }

                npc.velocity.Y += 0.4f;
                if (npc.velocity.Y > 12f)
                    npc.velocity.Y = 12f;

                npc.ai[0] = 1f;
            }

            npc.rotation = npc.velocity.Y * npc.direction * rotation;
            var rotationLimit = 2f * rotation;
            npc.rotation = MathHelper.Clamp(npc.rotation, -rotationLimit, rotationLimit);
        }
        #endregion

        internal static void WalkAndHandleObstacles(NPC npc, int backUpTimer, bool noXMovement, bool unusedFlag)
        {
            bool isOnSolidTile = false;
            if (npc.velocity.Y == 0f)
            {
                int yTile = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int initialXTile = (int)npc.position.X / 16;
                int maxXTile = (int)(npc.position.X + (float)npc.width) / 16;
                for (int xTile = initialXTile; xTile <= maxXTile; xTile++)
                {
                    if (Main.tile[xTile, yTile] == null)
                    {
                        return;
                    }
                    if (Main.tile[xTile, yTile].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[xTile, yTile].TileType])
                    {
                        isOnSolidTile = true;
                        break;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int fallFaceDirection = 0;
                if (npc.velocity.X < 0f)
                {
                    fallFaceDirection = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    fallFaceDirection = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int xTileBelow = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * fallFaceDirection)) / 16f);
                int yTileBelow = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if ((float)(xTileBelow * 16) < position.X + (float)npc.width && (float)(xTileBelow * 16 + 16) > position.X && ((Main.tile[xTileBelow, yTileBelow].HasUnactuatedTile && !Main.tile[xTileBelow, yTileBelow].TopSlope && !Main.tile[xTileBelow, yTileBelow - 1].TopSlope && Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow].TileType] && !Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow].TileType]) || (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock && Main.tile[xTileBelow, yTileBelow - 1].HasUnactuatedTile)) && (!Main.tile[xTileBelow, yTileBelow - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 1].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 1].TileType] || (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock && (!Main.tile[xTileBelow, yTileBelow - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 4].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 4].TileType]))) && (!Main.tile[xTileBelow, yTileBelow - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 2].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 2].TileType]) && (!Main.tile[xTileBelow, yTileBelow - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 3].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 3].TileType]) && (!Main.tile[xTileBelow - fallFaceDirection, yTileBelow - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow - fallFaceDirection, yTileBelow - 3].TileType]))
                {
                    float yPixelDistance = (float)(yTileBelow * 16);
                    if (Main.tile[xTileBelow, yTileBelow].IsHalfBlock)
                    {
                        yPixelDistance += 8f;
                    }
                    if (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock)
                    {
                        yPixelDistance -= 8f;
                    }
                    if (yPixelDistance < position.Y + (float)npc.height)
                    {
                        float percentageTileRisen = position.Y + (float)npc.height - yPixelDistance;
                        float fullTileAmt = 16.1f;
                        if (percentageTileRisen <= fullTileAmt)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - yPixelDistance;
                            npc.position.Y = yPixelDistance - (float)npc.height;
                            if (percentageTileRisen < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (isOnSolidTile)
            {
                int doorCheckX = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                int doorCheckY = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if ((Main.tile[doorCheckX, doorCheckY - 1].HasUnactuatedTile && (Main.tile[doorCheckX, doorCheckY - 1].TileType == TileID.ClosedDoor || Main.tile[doorCheckX, doorCheckY - 1].TileType == TileID.TallGateClosed)) & unusedFlag)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        npc.velocity.X = 0.5f * (float)-(float)npc.direction;
                        int doorOpenInc = 5;
                        if (Main.tile[doorCheckX, doorCheckY - 1].TileType == TileID.TallGateClosed)
                        {
                            doorOpenInc = 2;
                        }
                        npc.ai[1] += (float)doorOpenInc;
                        npc.ai[2] = 0f;
                        bool letMeIn = false;
                        if (npc.ai[1] >= 10f)
                        {
                            letMeIn = true;
                            npc.ai[1] = 10f;
                        }
                        WorldGen.KillTile(doorCheckX, doorCheckY - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !letMeIn) && letMeIn && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.tile[doorCheckX, doorCheckY - 1].TileType == TileID.ClosedDoor)
                            {
                                bool canOpenDoor = WorldGen.OpenDoor(doorCheckX, doorCheckY - 1, npc.direction);
                                if (!canOpenDoor)
                                {
                                    npc.ai[3] = (float)backUpTimer;
                                    npc.netUpdate = true;
                                }
                                if (Main.dedServ & canOpenDoor)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, (float)doorCheckX, (float)(doorCheckY - 1), (float)npc.direction, 0, 0, 0);
                                }
                            }
                            if (Main.tile[doorCheckX, doorCheckY - 1].TileType == TileID.TallGateClosed)
                            {
                                bool canOpenTallGate = WorldGen.ShiftTallGate(doorCheckX, doorCheckY - 1, false);
                                if (!canOpenTallGate)
                                {
                                    npc.ai[3] = (float)backUpTimer;
                                    npc.netUpdate = true;
                                }
                                if (Main.dedServ & canOpenTallGate)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, (float)doorCheckX, (float)(doorCheckY - 1), 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int faceDirection = npc.spriteDirection;
                    if ((npc.velocity.X < 0f && faceDirection == -1) || (npc.velocity.X > 0f && faceDirection == 1))
                    {
                        if (npc.height >= 32 && Main.tile[doorCheckX, doorCheckY - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 2].TileType])
                        {
                            if (Main.tile[doorCheckX, doorCheckY - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 3].TileType])
                            {
                                npc.velocity.Y = -8f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -7f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[doorCheckX, doorCheckY - 1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 1].TileType])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(doorCheckY * 16) > 20f && Main.tile[doorCheckX, doorCheckY].HasUnactuatedTile && !Main.tile[doorCheckX, doorCheckY].TopSlope && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY].TileType])
                        {
                            npc.velocity.Y = -5f;
                            npc.netUpdate = true;
                        }
                        else if (npc.directionY < 0 && (!Main.tile[doorCheckX, doorCheckY + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY + 1].TileType]) && (!Main.tile[doorCheckX + npc.direction, doorCheckY + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[doorCheckX + npc.direction, doorCheckY + 1].TileType]))
                        {
                            npc.velocity.Y = -8f;
                            npc.velocity.X = npc.velocity.X * 1.5f;
                            npc.netUpdate = true;
                        }
                        else if (unusedFlag)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                        if ((npc.velocity.Y == 0f & noXMovement) && npc.ai[3] == 1f)
                        {
                            npc.velocity.Y = -5f;
                        }
                    }
                }
            }
            else if (unusedFlag)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }
        }
    }
}
