using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;

public static partial class RevengeanceAndDeathAI
{
    public class TortoiseAI : VanillaAIOverride
    {
        public override bool AI(Mod mod)
        {
            if (NPC.target < 0 || Main.player[NPC.target].dead || NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }

            int turtleFaceDirection = 0;
            if (NPC.velocity.X < 0f)
            {
                turtleFaceDirection = -1;
            }
            if (NPC.velocity.X > 0f)
            {
                turtleFaceDirection = 1;
            }

            Vector2 position = NPC.position;
            position.X += NPC.velocity.X;
            int turtleTileX = (int)((position.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 1) * turtleFaceDirection)) / 16f);
            int turtleTileY = (int)((position.Y + (float)NPC.height - 1f) / 16f);

            if ((float)(turtleTileX * 16) < position.X + (float)NPC.width && (float)(turtleTileX * 16 + 16) > position.X && ((Main.tile[turtleTileX, turtleTileY].HasUnactuatedTile && !Main.tile[turtleTileX, turtleTileY].TopSlope && !Main.tile[turtleTileX, turtleTileY - 1].TopSlope && ((Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY].TileType] && !Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY].TileType]) || (Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY].TileType] && (!Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY - 1].TileType] || !Main.tile[turtleTileX, turtleTileY - 1].HasUnactuatedTile) && Main.tile[turtleTileX, turtleTileY].TileType != TileID.Anvils && Main.tile[turtleTileX, turtleTileY].TileType != TileID.WorkBenches && Main.tile[turtleTileX, turtleTileY].TileType != TileID.MythrilAnvil))) || (Main.tile[turtleTileX, turtleTileY - 1].IsHalfBlock && Main.tile[turtleTileX, turtleTileY - 1].HasUnactuatedTile)) && (!Main.tile[turtleTileX, turtleTileY - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY - 1].TileType] || Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY - 1].TileType] || (Main.tile[turtleTileX, turtleTileY - 1].IsHalfBlock && (!Main.tile[turtleTileX, turtleTileY - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY - 4].TileType] || Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY - 4].TileType]))) && (!Main.tile[turtleTileX, turtleTileY - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY - 2].TileType] || Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY - 2].TileType]) && (!Main.tile[turtleTileX, turtleTileY - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[turtleTileX, turtleTileY - 3].TileType] || Main.tileSolidTop[(int)Main.tile[turtleTileX, turtleTileY - 3].TileType]) && (!Main.tile[turtleTileX - turtleFaceDirection, turtleTileY - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[turtleTileX - turtleFaceDirection, turtleTileY - 3].TileType] || Main.tileSolidTop[(int)Main.tile[turtleTileX - turtleFaceDirection, turtleTileY - 3].TileType]))
            {
                float tilePixelPosition = (float)(turtleTileY * 16);
                if (Main.tile[turtleTileX, turtleTileY].IsHalfBlock)
                {
                    tilePixelPosition += 8f;
                }
                if (Main.tile[turtleTileX, turtleTileY - 1].IsHalfBlock)
                {
                    tilePixelPosition -= 8f;
                }

                if (tilePixelPosition < position.Y + (float)NPC.height)
                {
                    float percentageTileRisen = position.Y + (float)NPC.height - tilePixelPosition;
                    if ((double)percentageTileRisen <= 16.1)
                    {
                        NPC.gfxOffY += NPC.position.Y + (float)NPC.height - tilePixelPosition;
                        NPC.position.Y = tilePixelPosition - (float)NPC.height;
                        if (percentageTileRisen < 9f)
                        {
                            NPC.stepSpeed = 0.75f;
                        }
                        else
                        {
                            NPC.stepSpeed = 1.5f;
                        }
                    }
                }
            }

            if (NPC.type == NPCID.IceTortoise && Main.rand.NextBool(10))
                SpawnIceTortoiseDust(NPC);

            if (NPC.ai[0] == 0f)
            {
                if (NPC.velocity.X < 0f)
                {
                    NPC.direction = -1;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.direction = 1;
                }

                NPC.spriteDirection = NPC.direction;

                Vector2 tortoisePosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float tortoiseTargetX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - tortoisePosition.X;
                float tortoiseTargetY = Main.player[NPC.target].position.Y - tortoisePosition.Y;
                float tortoiseTargetDist = (float)Math.Sqrt((double)(tortoiseTargetX * tortoiseTargetX + tortoiseTargetY * tortoiseTargetY));

                bool canHitPlayer = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                {
                    if (tortoiseTargetDist > 200f & canHitPlayer)
                    {
                        NPC.ai[1] += 3f;
                    }
                    if (tortoiseTargetDist > 600f && (canHitPlayer || NPC.position.Y + (float)NPC.height > Main.player[NPC.target].position.Y - 200f))
                    {
                        NPC.ai[1] += 6f;
                    }
                }
                else
                {
                    if (tortoiseTargetDist > 200f & canHitPlayer)
                    {
                        NPC.ai[1] += 6f;
                    }
                    if (tortoiseTargetDist > 600f && (canHitPlayer || NPC.position.Y + (float)NPC.height > Main.player[NPC.target].position.Y - 200f))
                    {
                        NPC.ai[1] += 15f;
                    }
                }

                if (NPC.wet)
                {
                    NPC.ai[1] = 1000f;
                }

                NPC.defense = NPC.defDefense;

                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                {
                    NPC.knockBackResist = 0.5f;
                }
                else
                {
                    NPC.knockBackResist = 0.15f;
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (CalamityWorld.death ? 400f : 500f))
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 1f;
                }

                if (!NPC.justHit && NPC.velocity.X != NPC.oldVelocity.X)
                {
                    NPC.direction *= -1;
                }

                if (NPC.velocity.Y == 0f && Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height)
                {
                    int tortoiseLeftTileX;
                    int tortoiseRightTileX;
                    if (NPC.direction > 0)
                    {
                        tortoiseLeftTileX = (int)(((double)NPC.position.X + (double)NPC.width * 0.5) / 16.0);
                        tortoiseRightTileX = tortoiseLeftTileX + 3;
                    }
                    else
                    {
                        tortoiseRightTileX = (int)(((double)NPC.position.X + (double)NPC.width * 0.5) / 16.0);
                        tortoiseLeftTileX = tortoiseRightTileX - 3;
                    }

                    int tortoiseBotTileY = (int)((NPC.position.Y + (float)NPC.height + 2f) / 16f) - 1;
                    int tortoiseTopTileY = tortoiseBotTileY + 4;
                    bool onSolidTile = false;
                    for (int x = tortoiseLeftTileX; x <= tortoiseRightTileX; x++)
                    {
                        for (int y = tortoiseBotTileY; y <= tortoiseTopTileY; y++)
                        {
                            if (Main.tile[x, y] != null && Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y].TileType])
                            {
                                onSolidTile = true;
                            }
                        }
                    }

                    if (!onSolidTile)
                    {
                        NPC.direction *= -1;
                        NPC.velocity.X = 0.1f * (float)NPC.direction;
                    }
                }

                if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                {
                    float giantShellyMaxVel = 1f;
                    if (NPC.velocity.X < -giantShellyMaxVel || NPC.velocity.X > giantShellyMaxVel)
                    {
                        if (NPC.velocity.Y == 0f)
                        {
                            NPC.velocity *= 0.8f;
                        }
                    }
                    else if (NPC.velocity.X < giantShellyMaxVel && NPC.direction == 1)
                    {
                        NPC.velocity.X = NPC.velocity.X + 0.1f;
                        if (NPC.velocity.X > giantShellyMaxVel)
                        {
                            NPC.velocity.X = giantShellyMaxVel;
                        }
                    }
                    else if (NPC.velocity.X > -giantShellyMaxVel && NPC.direction == -1)
                    {
                        NPC.velocity.X = NPC.velocity.X - 0.1f;
                        if (NPC.velocity.X < -giantShellyMaxVel)
                        {
                            NPC.velocity.X = -giantShellyMaxVel;
                        }
                    }
                }
                else
                {
                    float tortoiseMaxVel = 2f;
                    if (tortoiseTargetDist < 400f)
                    {
                        if (NPC.velocity.X < -tortoiseMaxVel || NPC.velocity.X > tortoiseMaxVel)
                        {
                            if (NPC.velocity.Y == 0f)
                            {
                                NPC.velocity *= 0.8f;
                            }
                        }
                        else if (NPC.velocity.X < tortoiseMaxVel && NPC.direction == 1)
                        {
                            NPC.velocity.X = NPC.velocity.X + 0.1f;
                            if (NPC.velocity.X > tortoiseMaxVel)
                            {
                                NPC.velocity.X = tortoiseMaxVel;
                            }
                        }
                        else if (NPC.velocity.X > -tortoiseMaxVel && NPC.direction == -1)
                        {
                            NPC.velocity.X = NPC.velocity.X - 0.1f;
                            if (NPC.velocity.X < -tortoiseMaxVel)
                            {
                                NPC.velocity.X = -tortoiseMaxVel;
                            }
                        }
                    }
                    else if (NPC.velocity.X < -3f || NPC.velocity.X > 3f)
                    {
                        if (NPC.velocity.Y == 0f)
                        {
                            NPC.velocity *= 0.8f;
                        }
                    }
                    else if (NPC.velocity.X < 3f && NPC.direction == 1)
                    {
                        NPC.velocity.X = NPC.velocity.X + 0.1f;
                        if (NPC.velocity.X > 3f)
                        {
                            NPC.velocity.X = 3f;
                        }
                    }
                    else if (NPC.velocity.X > -3f && NPC.direction == -1)
                    {
                        NPC.velocity.X = NPC.velocity.X - 0.1f;
                        if (NPC.velocity.X < -3f)
                        {
                            NPC.velocity.X = -3f;
                        }
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.velocity.X = NPC.velocity.X * 0.5f;

                if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                {
                    NPC.ai[1] += 1f;
                }
                else
                {
                    NPC.ai[1] += 2f;
                }

                if (NPC.ai[1] >= 30f)
                {
                    NPC.netUpdate = true;
                    NPC.TargetClosest(true);
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[0] = 3f;

                    if (NPC.type == NPCID.SolarSroller)
                    {
                        NPC.ai[0] = 6f;
                        NPC.ai[2] = (float)Main.rand.Next(2, 5);
                    }
                }
            }
            else
            {
                if (NPC.ai[0] == 3f)
                {
                    if (NPC.type == NPCID.IceTortoise && Main.rand.Next(3) < 2)
                        SpawnIceTortoiseDust(NPC);

                    if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                    {
                        NPC.damage = (int)Math.Round(NPC.defDamage * 1.2);
                    }
                    else
                    {
                        NPC.damage = (int)Math.Round(NPC.defDamage * 1.4);
                    }

                    NPC.defense = NPC.defDefense * 2;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] == 1f)
                    {
                        NPC.netUpdate = true;
                        NPC.TargetClosest(true);

                        NPC.ai[2] += 0.3f;
                        NPC.rotation += NPC.ai[2] * (float)NPC.direction;
                        NPC.ai[1] += 1f;

                        bool spinAttackCanHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                        float spinAttackSpeed = 15f;
                        if (!spinAttackCanHit)
                        {
                            spinAttackSpeed = 6f;
                        }
                        if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                        {
                            spinAttackSpeed *= 0.75f;
                        }

                        Vector2 spinAttackPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                        float spinAttackTargetX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - spinAttackPosition.X;
                        float absoluteSpinTargetX = Math.Abs(spinAttackTargetX) * 0.2f;
                        if (NPC.directionY > 0)
                        {
                            absoluteSpinTargetX = 0f;
                        }
                        float spinAttackTargetY = Main.player[NPC.target].position.Y - spinAttackPosition.Y - absoluteSpinTargetX;
                        float spinAttackTargetDist = (float)Math.Sqrt((double)(spinAttackTargetX * spinAttackTargetX + spinAttackTargetY * spinAttackTargetY));
                        NPC.netUpdate = true;

                        spinAttackTargetDist = spinAttackSpeed / spinAttackTargetDist;
                        spinAttackTargetX *= spinAttackTargetDist;
                        spinAttackTargetY *= spinAttackTargetDist;

                        if (!spinAttackCanHit)
                        {
                            spinAttackTargetY = -10f;
                        }

                        NPC.velocity.X = spinAttackTargetX;
                        NPC.velocity.Y = spinAttackTargetY;
                        NPC.ai[3] = NPC.velocity.X;
                    }
                    else
                    {
                        if (NPC.position.X + (float)NPC.width > Main.player[NPC.target].position.X && NPC.position.X < Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width && NPC.position.Y < Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height)
                        {
                            NPC.velocity.X = NPC.velocity.X * 0.8f;
                            NPC.ai[3] = 0f;
                            if (NPC.velocity.Y < 0f)
                            {
                                NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                            }
                        }

                        if (NPC.ai[3] != 0f)
                        {
                            NPC.velocity.X = NPC.ai[3];
                            NPC.velocity.Y = NPC.velocity.Y - 0.33f;
                        }

                        if (NPC.ai[1] >= 90f)
                        {
                            NPC.noGravity = false;
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 4f;
                        }
                    }

                    if (NPC.wet && NPC.directionY < 0)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.45f;
                    }

                    NPC.rotation += NPC.ai[2] * (float)NPC.direction;

                    return false;
                }

                if (NPC.ai[0] == 4f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (NPC.wet && NPC.directionY < 0)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.45f;
                    }

                    NPC.velocity.X = NPC.velocity.X * 0.95f;

                    if (NPC.ai[2] > 0f)
                    {
                        NPC.ai[2] -= 0.01f;
                        NPC.rotation += NPC.ai[2] * (float)NPC.direction;
                    }
                    else if (NPC.velocity.Y >= 0f)
                    {
                        NPC.rotation = 0f;
                    }

                    if (NPC.ai[2] <= 0f && (NPC.velocity.Y == 0f || NPC.wet))
                    {
                        NPC.netUpdate = true;
                        NPC.rotation = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 5f;
                    }
                }
                else
                {
                    if (NPC.ai[0] == 6f)
                    {
                        NPC.damage = (int)Math.Round(NPC.defDamage * 1.2);
                        NPC.defense = NPC.defDefense * 2;
                        NPC.knockBackResist = 0f;

                        if (Main.rand.Next(3) < 2)
                        {
                            int spinAttackDust = Dust.NewDust(NPC.Center - new Vector2(30f), 60, 60, DustID.Torch, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                            Dust dust = Main.dust[spinAttackDust];
                            dust.noGravity = true;
                            dust.velocity *= 0.2f;
                            dust.fadeIn = 1f;
                        }

                        NPC.ai[1] += 1f;
                        if (NPC.ai[3] > 0f)
                        {
                            if (NPC.ai[3] == 1f)
                            {
                                Vector2 vector68 = NPC.Center - new Vector2(50f);
                                for (int i = 0; i < 32; i++)
                                {
                                    int spinEndDust = Dust.NewDust(vector68, 100, 100, DustID.Torch, 0f, 0f, 100, default(Color), 2.5f);
                                    Dust dust = Main.dust[spinEndDust];
                                    dust.noGravity = true;
                                    dust.velocity *= 3f;
                                    spinEndDust = Dust.NewDust(vector68, 100, 100, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                                    dust.velocity *= 2f;
                                    dust.noGravity = true;
                                }

                                if (!Main.dedServ)
                                {
                                    for (int j = 0; j < 4; j++)
                                    {
                                        int spinEndGore = Gore.NewGore(NPC.GetSource_FromAI(), vector68 + new Vector2((float)(50 * Main.rand.Next(100)) / 100f, (float)(50 * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), 1f);
                                        Gore gore = Main.gore[spinEndGore];
                                        gore.velocity *= 0.3f;
                                        gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                                        gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                                    }
                                }
                            }

                            for (int k = 0; k < 5; k++)
                            {
                                int moreSpinEndDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[moreSpinEndDust].velocity *= Main.rand.NextFloat();
                            }

                            NPC.ai[3] += 1f;
                            if (NPC.ai[3] >= 10f)
                            {
                                NPC.ai[3] = 0f;
                            }
                        }

                        if (NPC.ai[1] == 1f)
                        {
                            NPC.netUpdate = true;
                            NPC.TargetClosest(true);

                            bool spinAboveCanHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                            float spinAboveSpeed = 24f;
                            if (!spinAboveCanHit)
                            {
                                spinAboveSpeed = 10f;
                            }

                            Vector2 vector69 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                            float spinAboveTargetX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector69.X;
                            float absoluteSpinAboveTargetX = Math.Abs(spinAboveTargetX) * 0.2f;
                            if (NPC.directionY > 0)
                            {
                                absoluteSpinAboveTargetX = 0f;
                            }
                            float spinAboveTargetY = Main.player[NPC.target].position.Y - vector69.Y - absoluteSpinAboveTargetX;
                            float spinAboveTargetDist = (float)Math.Sqrt((double)(spinAboveTargetX * spinAboveTargetX + spinAboveTargetY * spinAboveTargetY));
                            NPC.netUpdate = true;

                            spinAboveTargetDist = spinAboveSpeed / spinAboveTargetDist;
                            spinAboveTargetX *= spinAboveTargetDist;
                            spinAboveTargetY *= spinAboveTargetDist;

                            if (!spinAboveCanHit)
                            {
                                spinAboveTargetY = -12f;
                            }

                            NPC.velocity.X = spinAboveTargetX;
                            NPC.velocity.Y = spinAboveTargetY;
                        }
                        else
                        {
                            if (NPC.position.X + (float)NPC.width > Main.player[NPC.target].position.X && NPC.position.X < Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width && NPC.position.Y < Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height)
                            {
                                NPC.velocity.X = NPC.velocity.X * 0.9f;
                                if (NPC.velocity.Y < 0f)
                                {
                                    NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                                }
                            }

                            if (NPC.ai[2] == 0f || NPC.ai[1] >= 1200f)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[0] = 5f;
                            }
                        }

                        if (NPC.wet && NPC.directionY < 0)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - 0.45f;
                        }

                        NPC.rotation += MathHelper.Clamp(NPC.velocity.X / 10f * (float)NPC.direction, -0.314159274f, 0.314159274f);

                        return false;
                    }

                    if (NPC.ai[0] == 5f)
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        NPC.rotation = 0f;
                        NPC.velocity.X = 0f;

                        if (NPC.type == NPCID.GiantShelly || NPC.type == NPCID.GiantShelly2)
                        {
                            NPC.ai[1] += 1f;
                        }
                        else
                        {
                            NPC.ai[1] += 2f;
                        }

                        if (NPC.ai[1] >= 30f)
                        {
                            NPC.TargetClosest(true);
                            NPC.netUpdate = true;
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 0f;
                        }

                        if (NPC.wet)
                        {
                            NPC.ai[0] = 3f;
                            NPC.ai[1] = 0f;
                        }
                    }
                }
            }

            return false;
        }

        private static void SpawnIceTortoiseDust(NPC npc)
        {
            int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceRod, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
            Main.dust[dustIndex].noGravity = true;
            Dust dust = Main.dust[dustIndex];
            dust.velocity *= 0.2f;
        }
    }
}
