using System;
using CalamityMod.Events;
using CalamityMod.Particles;
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
    public class GolemAI : VanillaAIOverride
    {
        // Rev+ exclusive
        public static int FireballDamage = 24; // 96 (modified to be always at maximum Expert damage and does not scale)
        public static int LaserDamage = 29; // 116 (modified to be always at maximum Expert damage and does not scale)
        public static int InfernoBoltDamage = 35; // 140

        private static void CheckTempleWall(NPC npc, ref bool enrage, ref bool turboEnrage)
        {
            int targetTilePosX = (int)Main.player[npc.target].Center.X / 16;
            int targetTilePosY = (int)Main.player[npc.target].Center.Y / 16;

            Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
            if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                enrage = false;
            else
                turboEnrage = Main.getGoodWorld;
        }

        private static void ConfigureLaser(NPC npc, int projectileIndex, bool enrage, bool turboEnrage)
        {
            Main.projectile[projectileIndex].timeLeft = enrage ? 600 : 300;
            if (turboEnrage)
                Main.projectile[projectileIndex].extraUpdates += 1;

            npc.netUpdate = true;
        }

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // whoAmI variable
            NPC.golemBoss = NPC.whoAmI;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Spawn parts
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X - 84, (int)NPC.Center.Y - 9, NPCID.GolemFistLeft, 0);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 78, (int)NPC.Center.Y - 9, NPCID.GolemFistRight, 0);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X - 3, (int)NPC.Center.Y - 57, NPCID.GolemHead);
            }

            // Despawn
            if (NPC.target >= 0 && Main.player[NPC.target].dead)
            {
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                if (Main.player[NPC.target].dead)
                    NPC.noTileCollide = true;
            }

            // Enrage if the target isn't inside the temple
            // Turbo enrage if target isn't inside the temple and it's For the Worthy
            bool enrage = !BossRushEvent.BossRushActive;
            bool turboEnrage = false;
            if (Main.player[NPC.target].Center.Y > Main.worldSurface * 16.0)
            {
                CheckTempleWall(NPC, ref enrage, ref turboEnrage);
            }
            else
                turboEnrage = Main.getGoodWorld;

            if (Main.getGoodWorld)
                enrage = true;

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && (enrage || turboEnrage);

            bool reduceFallSpeed = NPC.velocity.Y > 0f && Collision.SolidCollision(NPC.position + Vector2.UnitY * 1.1f * NPC.velocity.Y, NPC.width, NPC.height);

            // Alpha
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                NPC.ai[1] = 0f;
            }

            // Check for body parts
            bool headAlive = NPC.AnyNPCs(NPCID.GolemHead);
            bool leftFistAlive = NPC.AnyNPCs(NPCID.GolemFistLeft);
            bool rightFistAlive = NPC.AnyNPCs(NPCID.GolemFistRight);
            NPC.dontTakeDamage = headAlive || leftFistAlive || rightFistAlive;

            // Distance required for despawning
            int despawnDistance = turboEnrage ? 7500 : enrage ? 6000 : 4500;

            // Deactivate torches
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && NPC.velocity.Y > 0f)
            {
                for (int j = (int)(NPC.position.X / 16f); (float)j < (NPC.position.X + (float)NPC.width) / 16f; j++)
                {
                    for (int k = (int)(NPC.position.Y / 16f); (float)k < (NPC.position.Y + (float)NPC.width) / 16f; k++)
                    {
                        if (Main.tile[j, k].TileType == TileID.Torches)
                        {
                            Main.tile[j, k].Get<TileWallWireStateData>().HasTile = false;
                            if (Main.dedServ)
                                NetMessage.SendTileSquare(-1, j, k);
                        }
                    }
                }
            }

            // Spawn arm dust
            if (!Main.getGoodWorld)
            {
                if (!leftFistAlive)
                {
                    int lostLeftFistDust = Dust.NewDust(new Vector2(NPC.Center.X - 80f * NPC.scale, NPC.Center.Y - 9f), 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1f);
                    Dust dust = Main.dust[lostLeftFistDust];
                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                    if (Main.rand.NextBool(10))
                    {
                        lostLeftFistDust = Dust.NewDust(new Vector2(NPC.Center.X - 80f * NPC.scale, NPC.Center.Y - 9f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 1f);
                        if (!Main.rand.NextBool(20))
                        {
                            Main.dust[lostLeftFistDust].noGravity = true;
                            dust = Main.dust[lostLeftFistDust];
                            dust.scale *= 1f + Main.rand.Next(10) * 0.1f;
                            dust.velocity.Y -= 1f;
                        }
                    }
                }
                if (!rightFistAlive)
                {
                    int lostRightFistDust = Dust.NewDust(new Vector2(NPC.Center.X + 62f * NPC.scale, NPC.Center.Y - 9f), 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1f);
                    Dust dust = Main.dust[lostRightFistDust];
                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                    if (Main.rand.NextBool(10))
                    {
                        lostRightFistDust = Dust.NewDust(new Vector2(NPC.Center.X + 62f * NPC.scale, NPC.Center.Y - 9f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 1f);
                        if (!Main.rand.NextBool(20))
                        {
                            Main.dust[lostRightFistDust].noGravity = true;
                            dust = Main.dust[lostRightFistDust];
                            dust.scale *= 1f + Main.rand.Next(10) * 0.1f;
                            dust.velocity.Y -= 1f;
                        }
                    }
                }
            }

            if (NPC.noTileCollide && !Main.player[NPC.target].dead)
            {
                if (NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y)
                    NPC.noTileCollide = false;
                else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.noTileCollide = false;
            }

            // Jump
            if (NPC.ai[0] == 0f)
            {
                if (NPC.velocity.Y == 0f || NPC.ai[2] > 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Delay before jumping
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.velocity.X *= 0.8f;
                        NPC.ai[1] += 1f;
                    }

                    if (NPC.ai[1] > 0f)
                    {
                        NPC.ai[1] += death ? 1.5f : 1f;
                        if (Main.getGoodWorld)
                            NPC.ai[1] += 100f;

                        if (enrage)
                        {
                            NPC.ai[1] += 18f;
                        }
                        else
                        {
                            if (!leftFistAlive)
                                NPC.ai[1] += 6f;
                            if (!rightFistAlive)
                                NPC.ai[1] += 6f;
                        }
                    }
                    bool canJump = (!headAlive || Main.npc[NPC.FindFirstNPC(NPCID.GolemHead)].ai[0] <= 1f) && (!NPC.AnyNPCs(NPCID.GolemHeadFree) || Main.npc[NPC.FindFirstNPC(NPCID.GolemHeadFree)].ai[0] != 3);
                    if (NPC.ai[1] >= 300f && canJump)
                    {
                        NPC.ai[1] = -20f;
                        NPC.frameCounter = 0D;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        // Set jump velocity
                        if (!headAlive)
                            CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                        // Set damage
                        NPC.damage = NPC.defDamage;

                        if (NPC.ai[3] == 0f)
                            NPC.ai[3] = (death ? !leftFistAlive && !rightFistAlive : !headAlive) ? Main.rand.Next(1, 2 + 1) : 1f;

                        switch ((int)NPC.ai[3])
                        {
                            default:
                            case 0:
                            case 1:
                                NormalJump(canJump);
                                break;

                            // Jump directly above the target's head and slam down
                            case 2:
                                SlamJump(canJump);
                                break;
                        }

                        void NormalJump(bool jump)
                        {
                            if (!jump)
                                return;

                            float velocityBoost = (death ? 3f : 2f) * (1f - (lifeRatio / 2));
                            float velocityX = 4f + velocityBoost;
                            if (enrage)
                                velocityX *= 1.5f;

                            float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                            NPC.direction = playerLocation < 0 ? 1 : -1;
                            calamityGlobalNPC.newAI[1] = NPC.direction;

                            NPC.velocity.X = velocityX * NPC.direction;

                            float distanceBelowTarget = NPC.position.Y - (Main.player[NPC.target].position.Y + 80f);
                            float speedMult = 1f;

                            float multiplier = turboEnrage ? 0.00275f : enrage ? 0.0025f : 0.00175f;
                            if (distanceBelowTarget > 0f && ((!leftFistAlive && !rightFistAlive) || turboEnrage))
                                speedMult += distanceBelowTarget * multiplier;

                            float speedMultLimit = turboEnrage ? 3.25f : enrage ? 3f : 2.5f;
                            if (speedMult > speedMultLimit)
                                speedMult = speedMultLimit;

                            if (Main.player[NPC.target].position.Y < NPC.Bottom.Y)
                                NPC.velocity.Y = ((turboEnrage ? -15.5f : -11.75f) + (enrage ? -4f : 0f)) * speedMult;
                            else
                                NPC.velocity.Y = 1f;

                            NPC.noTileCollide = true;

                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;

                            NPC.netUpdate = true;
                            NPC.SyncExtraAI();
                        }

                        void SlamJump(bool jump)
                        {
                            NPC.noTileCollide = true;

                            NPC.ai[2] += 1f;
                            float jumpVelocity = death ? 21f : 18f;
                            if (enrage)
                                jumpVelocity *= 1.25f;
                            if (turboEnrage)
                                jumpVelocity *= 1.25f;

                            float minJumpTime = 15f;
                            float maxJumpTime = 45f;
                            if ((NPC.ai[2] >= minJumpTime && Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) <= jumpVelocity) || NPC.ai[2] >= maxJumpTime || !jump)
                            {
                                NPC.ai[0] = 1f;
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 1f;
                                NPC.velocity.Y = -3f;
                                NPC.netUpdate = true;
                            }

                            if (!jump)
                                return;

                            Vector2 center = NPC.Center;
                            if (!Main.player[NPC.target].dead && Main.player[NPC.target].active && Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) <= despawnDistance)
                                center = Main.player[NPC.target].Center;

                            center.Y -= 480f;
                            if (NPC.velocity.Y == 0f)
                            {
                                NPC.velocity = center - NPC.Center;
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero);
                                NPC.velocity *= jumpVelocity;

                                float distanceBelowTarget = NPC.position.Y - (Main.player[NPC.target].position.Y + 80f);
                                float speedMult = 1f;

                                float multiplier = turboEnrage ? 0.0025f : enrage ? 0.002f : 0.0015f;
                                if (distanceBelowTarget > 0f && ((!leftFistAlive && !rightFistAlive) || turboEnrage))
                                    speedMult += distanceBelowTarget * multiplier;

                                float speedMultLimit = turboEnrage ? 3.25f : enrage ? 3f : 2.5f;
                                if (speedMult > speedMultLimit)
                                    speedMult = speedMultLimit;

                                if (Main.player[NPC.target].position.Y < NPC.Bottom.Y)
                                    NPC.velocity.Y *= speedMult;
                            }
                            else
                                NPC.velocity.Y *= 0.95f;
                        }
                    }
                }

                // Don't run custom gravity when starting a jump
                if (NPC.ai[0] != 1f && NPC.ai[2] == 0f)
                    CustomGravity(false);
            }

            // Fall down
            else if (NPC.ai[0] == 1f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Play sound
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                    NPC.ai[0] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.SyncExtraAI();

                    // Dust and gore
                    for (int i = (int)NPC.position.X - 20; i < (int)NPC.position.X + NPC.width + 40; i += 20)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int fallDust = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                            Dust dust = Main.dust[fallDust];
                            dust.velocity *= 0.2f;
                        }
                        if (!Main.dedServ)
                        {
                            int fallGore = Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(i - 20, NPC.position.Y + NPC.height - 8f), default, Main.rand.Next(61, 64), 1f);
                            Gore gore = Main.gore[fallGore];
                            gore.velocity *= 0.4f;
                        }
                    }

                    // Fireball explosion when head is detached
                    if (Main.netMode != NetmodeID.MultiplayerClient && (!headAlive || turboEnrage))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int fiery = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                            Main.dust[fiery].velocity.Y *= 6f;
                            Main.dust[fiery].velocity.X *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[fiery].scale = 0.5f;
                                Main.dust[fiery].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 20; j++)
                        {
                            int fiery2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                            Main.dust[fiery2].noGravity = true;
                            Main.dust[fiery2].velocity.Y *= 10f;
                            fiery2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                            Main.dust[fiery2].velocity.X *= 2f;
                        }

                        float projectileVelocity = 4.75f;
                        if (enrage)
                            projectileVelocity *= 1.5f;
                        if (turboEnrage)
                            projectileVelocity *= 1.25f;

                        int type = ProjectileID.Fireball;
                        int damage = FireballDamage;
                        Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                        destination.Normalize();
                        destination *= projectileVelocity;
                        int totalFireballsPerSide = 3;
                        int totalIterations = turboEnrage ? 11 : death ? 40 : 60;
                        float rotation = MathHelper.ToRadians(90);
                        for (int i = 0; i < totalIterations; i++)
                        {
                            // Spawn projectiles 0, 1, 2, 22, 23, and 24
                            if (i < totalFireballsPerSide || i >= totalIterations - totalFireballsPerSide)
                            {
                                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(totalIterations - 1)));
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * (NPC.height / 2 * 0.8f) * NPC.scale + Vector2.Normalize(perturbedSpeed) * (NPC.width / 3) * NPC.scale, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                Main.projectile[proj].timeLeft = enrage ? 480 : 150; // The difference is meant to be this stark.
                                if (turboEnrage)
                                    Main.projectile[proj].extraUpdates += 1;
                            }
                        }

                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    // Velocity when falling
                    if ((NPC.position.X < Main.player[NPC.target].position.X && NPC.position.X + NPC.width > Main.player[NPC.target].position.X + Main.player[NPC.target].width) || NPC.ai[2] == 1f)
                    {
                        NPC.velocity.X *= NPC.ai[2] == 1f ? 0.5f : 0.8f;

                        if (NPC.Bottom.Y < Main.player[NPC.target].position.Y || NPC.ai[2] == 1f)
                        {
                            float fallSpeedBoost = death ? 0.9f * (1f - (lifeRatio / 2)) : 0.75f * (1f - (lifeRatio / 2));
                            float fallSpeed = (death ? 0.3f : 0.2f) + fallSpeedBoost;
                            if (enrage)
                                fallSpeed *= 2f;

                            NPC.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        float velocityChangeBoost = death ? 0.16f * (1f - (lifeRatio / 2)) : 0.12f * (1f - (lifeRatio / 2));
                        float velocityXChange = (death ? 0.285f : 0.2f) + velocityChangeBoost;
                        if (NPC.direction < 0)
                            NPC.velocity.X -= velocityXChange;
                        else if (NPC.direction > 0)
                            NPC.velocity.X += velocityXChange;

                        float velocityBoost = death ? 5.75f * (1f - (lifeRatio / 2)) : 4f * (1f - (lifeRatio / 2));
                        float velocityXCap = (death ? 6f : 4f) + velocityBoost;
                        if (enrage)
                            velocityXCap *= 3f;

                        float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                        int directionRelativeToTarget = playerLocation < 0 ? 1 : -1;
                        bool slowDown = directionRelativeToTarget != calamityGlobalNPC.newAI[1];

                        if (slowDown)
                            velocityXCap *= (enrage ? 0.2f : 0.5f);

                        if (NPC.velocity.X < -velocityXCap)
                            NPC.velocity.X = -velocityXCap;
                        if (NPC.velocity.X > velocityXCap)
                            NPC.velocity.X = velocityXCap;
                    }

                    CustomGravity(NPC.ai[2] == 1f);
                }
            }

            void CustomGravity(bool isSlamming)
            {
                float gravity = turboEnrage ? 0.85f : enrage ? 0.75f : (!leftFistAlive && !rightFistAlive) ? 0.45f : 0.3f;
                float maxFallSpeed = reduceFallSpeed ? 12f : turboEnrage ? 30f : enrage ? 25f : (!leftFistAlive && !rightFistAlive) ? 15f : 10f;
                if (isSlamming && !reduceFallSpeed)
                {
                    gravity *= 4f;
                    maxFallSpeed *= 2f;
                }

                NPC.velocity.Y += gravity;
                if (NPC.velocity.Y > maxFallSpeed)
                    NPC.velocity.Y = maxFallSpeed;
            }

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            // Despawn
            if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) + Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > despawnDistance)
            {
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) + Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > despawnDistance)
                {
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }

            return false;
        }

        public class FistAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                if (NPC.golemBoss < 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                if (NPC.alpha > 0)
                {
                    NPC.alpha -= 10;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                NPC golem = Main.npc[NPC.golemBoss];
                Player player = Main.player[NPC.target];

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Enrage if the target isn't inside the temple
                // Turbo enrage if target isn't inside the temple and it's For the Worthy
                bool enrage = !BossRushEvent.BossRushActive;
                bool turboEnrage = false;
                if (player.Center.Y > Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
                {
                    int targetTilePosX = (int)player.Center.X / 16;
                    int targetTilePosY = (int)player.Center.Y / 16;

                    Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
                    if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                        enrage = false;
                    else
                        turboEnrage = Main.getGoodWorld;
                }
                else
                    turboEnrage = Main.getGoodWorld;

                if (Main.getGoodWorld)
                    enrage = true;

                float aggression = turboEnrage ? 3f : enrage ? 2f : death ? 1.7f : 1f;

                Vector2 fistCenter = golem.Center + golem.velocity + new Vector2(0f, -9f * NPC.scale);
                fistCenter.X += (float)((NPC.type == NPCID.GolemFistLeft) ? -84 : 78) * NPC.scale;
                Vector2 distanceFromFistCenter = fistCenter - NPC.Center;
                float distanceFromRestPosition = distanceFromFistCenter.Length();
                if (NPC.ai[0] == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.noTileCollide = true;

                    float fistSpeed = 28f;
                    fistSpeed *= (aggression + 3f) / 4f;
                    if (fistSpeed > 48f)
                        fistSpeed = 48f;

                    float fistRestDistance = distanceFromRestPosition;
                    if (fistRestDistance < 12f + fistSpeed)
                    {
                        NPC.rotation = 0f;
                        NPC.velocity.X = distanceFromFistCenter.X;
                        NPC.velocity.Y = distanceFromFistCenter.Y;

                        bool canPunch = NPC.alpha == 0 && (NPC.type == NPCID.GolemFistLeft && NPC.Center.X + 100f > player.Center.X) || (NPC.type == NPCID.GolemFistRight && NPC.Center.X - 100f < player.Center.X);
                        if (canPunch)
                        {
                            float fistShootSpeed = death ? Main.rand.NextFloat(aggression * 0.5f, aggression * 2f) : aggression;
                            NPC.ai[1] += fistShootSpeed;
                            if (NPC.life < NPC.lifeMax / 2)
                                NPC.ai[1] += fistShootSpeed;
                            if (NPC.life < NPC.lifeMax / 4)
                                NPC.ai[1] += fistShootSpeed;
                        }

                        float fistPunchGateValue = death ? 120f : 40f;
                        if (NPC.ai[1] >= fistPunchGateValue)
                        {
                            if (canPunch)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[0] = 1f;
                            }
                            else
                                NPC.ai[1] = 0f;

                            // Net update in Master due to rng
                            if (death)
                                NPC.ForceNetUpdate();
                        }
                    }
                    else
                    {
                        fistRestDistance = fistSpeed / fistRestDistance;
                        NPC.velocity.X = distanceFromFistCenter.X * fistRestDistance;
                        NPC.velocity.Y = distanceFromFistCenter.Y * fistRestDistance;

                        NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
                        if (NPC.type == NPCID.GolemFistLeft)
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.ai[1] += 1f;
                    NPC.Center = fistCenter;
                    NPC.rotation = 0f;
                    NPC.velocity = Vector2.Zero;
                    if (NPC.ai[1] <= 15f)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            Vector2 largeRandDustRadius = Main.rand.NextVector2Circular(80f, 80f);
                            Vector2 largeRandDustRecoil = largeRandDustRadius * -1f * 0.05f;
                            Vector2 smallRandDustRadius = Main.rand.NextVector2Circular(20f, 20f);
                            Dust dust = Dust.NewDustPerfect(NPC.Center + largeRandDustRecoil + largeRandDustRadius + smallRandDustRadius, DustID.GoldFlame, largeRandDustRecoil);
                            dust.fadeIn = 1.5f;
                            dust.scale = 0.5f;
                            if (Main.getGoodWorld)
                                dust.noLight = true;

                            dust.noGravity = true;
                        }
                    }

                    if (NPC.ai[1] >= 30f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.noTileCollide = true;
                        NPC.collideX = false;
                        NPC.collideY = false;

                        float fistReturnSpeed = 24f;
                        fistReturnSpeed *= (aggression + 3f) / 4f;
                        if (fistReturnSpeed > 48f)
                            fistReturnSpeed = 48f;

                        Vector2 fistCent = NPC.Center;
                        float fistTargetXDist = player.Center.X - fistCent.X;
                        float fistTargetYDist = player.Center.Y - fistCent.Y;
                        float fistTargetDistance = (float)Math.Sqrt(fistTargetXDist * fistTargetXDist + fistTargetYDist * fistTargetYDist);
                        fistTargetDistance = fistReturnSpeed / fistTargetDistance;
                        NPC.velocity.X = fistTargetXDist * fistTargetDistance;
                        NPC.velocity.Y = fistTargetYDist * fistTargetDistance;
                        NPC.ai[0] = 2f;
                        NPC.ai[1] = 0f;

                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
                        if (NPC.type == NPCID.GolemFistLeft)
                            NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
                    }
                }
                else if (NPC.ai[0] == 2f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                    {
                        for (int j = (int)(NPC.position.X / 16f) - 1; (float)j < (NPC.position.X + (float)NPC.width) / 16f + 1f; j++)
                        {
                            for (int k = (int)(NPC.position.Y / 16f) - 1; (float)k < (NPC.position.Y + (float)NPC.width) / 16f + 1f; k++)
                            {
                                if (Main.tile[j, k].TileType == TileID.Torches)
                                {
                                    Main.tile[j, k].Get<TileWallWireStateData>().HasTile = false;
                                    if (Main.dedServ)
                                        NetMessage.SendTileSquare(-1, j, k);
                                }
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] == 1f)
                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                    if (Main.rand.NextBool())
                    {
                        Vector2 halfVelocityDust = NPC.velocity * 0.5f;
                        Vector2 randDustRadius = Main.rand.NextVector2Circular(20f, 20f);
                        Dust.NewDustPerfect(NPC.Center + halfVelocityDust + randDustRadius, DustID.SparkForLightDisc, halfVelocityDust, 0, Main.OurFavoriteColor).scale = 2f;
                    }

                    if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                    {
                        if (NPC.velocity.X > 0f && NPC.Center.X > player.Center.X)
                            NPC.noTileCollide = false;

                        if (NPC.velocity.X < 0f && NPC.Center.X < player.Center.X)
                            NPC.noTileCollide = false;
                    }
                    else
                    {
                        if (NPC.velocity.Y > 0f && NPC.Center.Y > player.Center.Y)
                            NPC.noTileCollide = false;

                        if (NPC.velocity.Y < 0f && NPC.Center.Y < player.Center.Y)
                            NPC.noTileCollide = false;
                    }

                    float maxPunchDistance = 700f;
                    if (death)
                    {
                        if (NPC.life < NPC.lifeMax / 2)
                            maxPunchDistance += MathHelper.Lerp(-175f, 75f, Main.rand.NextFloat());
                        if (NPC.life < NPC.lifeMax / 4)
                            maxPunchDistance += MathHelper.Lerp(-175f, 75f, Main.rand.NextFloat());
                    }

                    if (distanceFromRestPosition > maxPunchDistance || NPC.collideX || NPC.collideY)
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;
                        NPC.noTileCollide = true;
                        NPC.ai[0] = 0f;
                    }
                }
                else
                {
                    if (NPC.ai[0] != 3f)
                        return false;

                    // Set damage
                    NPC.damage = NPC.defDamage;

                    NPC.noTileCollide = true;
                    float fistAcceleration = 0.4f;
                    Vector2 returningFistCenter = NPC.Center;
                    float returningTargetX = player.Center.X - returningFistCenter.X;
                    float returningTargetY = player.Center.Y - returningFistCenter.Y;
                    float returningTargetDist = (float)Math.Sqrt(returningTargetX * returningTargetX + returningTargetY * returningTargetY);
                    returningTargetDist = 12f / returningTargetDist;
                    returningTargetX *= returningTargetDist;
                    returningTargetY *= returningTargetDist;

                    if (NPC.velocity.X < returningTargetX)
                    {
                        NPC.velocity.X += fistAcceleration;
                        if (NPC.velocity.X < 0f && returningTargetX > 0f)
                            NPC.velocity.X += fistAcceleration * 2f;
                    }
                    else if (NPC.velocity.X > returningTargetX)
                    {
                        NPC.velocity.X -= fistAcceleration;
                        if (NPC.velocity.X > 0f && returningTargetX < 0f)
                            NPC.velocity.X -= fistAcceleration * 2f;
                    }

                    if (NPC.velocity.Y < returningTargetY)
                    {
                        NPC.velocity.Y += fistAcceleration;
                        if (NPC.velocity.Y < 0f && returningTargetY > 0f)
                            NPC.velocity.Y += fistAcceleration * 2f;
                    }
                    else if (NPC.velocity.Y > returningTargetY)
                    {
                        NPC.velocity.Y -= fistAcceleration;
                        if (NPC.velocity.Y > 0f && returningTargetY < 0f)
                            NPC.velocity.Y -= fistAcceleration * 2f;
                    }

                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
                    if (NPC.type == NPCID.GolemFistLeft)
                        NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
                }

                return false;
            }
        }

        public class HeadAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                // Don't collide
                NPC.noTileCollide = true;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                    options.aggroRatio = -1f;
                    options.finishThemOff = true;
                    CalamityUtils.CalamityTargeting(NPC, options);
                }

                // Die if body is gone
                if (NPC.golemBoss < 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                // Percent life remaining
                float lifeRatio = NPC.life / (float)NPC.lifeMax;

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Count body parts
                bool leftFistAlive = NPC.AnyNPCs(NPCID.GolemFistLeft);
                bool rightFistAlive = NPC.AnyNPCs(NPCID.GolemFistRight);
                NPC.dontTakeDamage = leftFistAlive || rightFistAlive;

                // Stay in position on top of body
                NPC.Center = Main.npc[NPC.golemBoss].Center - new Vector2(3f, 57f) * NPC.scale;
                NPC.velocity = Main.npc[NPC.golemBoss].velocity;

                // Enrage if the target isn't inside the temple
                bool enrage = !BossRushEvent.BossRushActive;
                bool turboEnrage = false;
                if (Main.player[NPC.target].Center.Y > Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
                {
                    CheckTempleWall(NPC, ref enrage, ref turboEnrage);
                }
                else
                    turboEnrage = Main.getGoodWorld;

                if (Main.getGoodWorld)
                    enrage = true;

                // Alpha
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= 10;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;

                    NPC.ai[1] = 30f;
                }

                // Spit fireballs if arms are alive
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[1] += 1f;
                    float openMouthGateValue = (!rightFistAlive || !leftFistAlive) ? 10f : 20f;
                    float shootFireballGateValue = (!rightFistAlive || !leftFistAlive) ? 60f : 120f;
                    if (NPC.ai[1] < openMouthGateValue || NPC.ai[1] > shootFireballGateValue - openMouthGateValue)
                        NPC.localAI[0] = 1f;
                    else
                        NPC.localAI[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] >= shootFireballGateValue)
                    {
                        NPC.ai[1] = 0f;

                        Vector2 headCent = new Vector2(NPC.Center.X, NPC.Center.Y + 10f * NPC.scale);
                        float headFireballSpeed = turboEnrage ? 24f : enrage ? 18f : 9f;
                        float headFireballTargetX = Main.player[NPC.target].Center.X - headCent.X;
                        float headFireballTargetY = Main.player[NPC.target].Center.Y - headCent.Y;
                        float headFireballTargetDist = (float)Math.Sqrt(headFireballTargetX * headFireballTargetX + headFireballTargetY * headFireballTargetY);

                        headFireballTargetDist = headFireballSpeed / headFireballTargetDist;
                        headFireballTargetX *= headFireballTargetDist;
                        headFireballTargetY *= headFireballTargetDist;

                        int type = ProjectileID.Fireball;
                        int damage = FireballDamage;

                        int fireballAmount = death ? 2 : 1;
                        Vector2 fireballVelocity = new Vector2(headFireballTargetX, headFireballTargetY);
                        for (int i = 0; i < fireballAmount; i++)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), headCent, fireballVelocity * (1f / (i + 1)), type, damage, 0f, Main.myPlayer);

                        NPC.netUpdate = true;
                    }
                }

                // Shoot lasers and fireballs if arms are dead
                else if (NPC.ai[0] == 1f)
                {
                    // Fire projectiles from eye positions
                    Vector2 projectileFirePos = new Vector2(NPC.Center.X, NPC.Center.Y + 10f * NPC.scale);
                    if (Main.player[NPC.target].Center.X < NPC.Center.X - NPC.width)
                    {
                        NPC.localAI[1] = -1f;
                        projectileFirePos.X -= 40f * NPC.scale;
                    }
                    else if (Main.player[NPC.target].Center.X > NPC.Center.X + NPC.width)
                    {
                        NPC.localAI[1] = 1f;
                        projectileFirePos.X += 40f * NPC.scale;
                    }
                    else
                        NPC.localAI[1] = 0f;

                    // Timer for special laser attack
                    NPC.ai[3]++;
                    if (NPC.ai[3] >= 600f && Main.npc[NPC.golemBoss].velocity.Y == 0f && MathF.Abs(Main.npc[NPC.golemBoss].velocity.X) < 0.5f)
                    {
                        NPC.ai[0] = 2f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.localAI[1] = (Main.player[NPC.target].Center.X > NPC.Center.X).ToDirectionInt();
                        NPC.netUpdate = true;
                    }

                    // Fireballs
                    NPC.ai[1] += 1f;
                    float openMouthGateValue = 20f - (death ? 15f * (1f - (lifeRatio / 2)) : 10f * (1f - (lifeRatio / 2)));
                    float shootFireballGateValue = 120f - (death ? 75f * (1f - (lifeRatio / 2)) : 50f * (1f - (lifeRatio / 2)));
                    if (NPC.ai[1] < openMouthGateValue || NPC.ai[1] > shootFireballGateValue - openMouthGateValue)
                        NPC.localAI[0] = 1f;
                    else
                        NPC.localAI[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] >= shootFireballGateValue)
                    {
                        NPC.ai[1] = 0f;

                        float fireballSpeedFistsDed = turboEnrage ? 28f : enrage ? 21f : 10.5f;
                        float fireballFistsDedTargetX = Main.player[NPC.target].Center.X - projectileFirePos.X;
                        float fireballFistsDedTargetY = Main.player[NPC.target].Center.Y - projectileFirePos.Y;
                        float fireballFistsDedTargetDist = (float)Math.Sqrt(fireballFistsDedTargetX * fireballFistsDedTargetX + fireballFistsDedTargetY * fireballFistsDedTargetY);

                        fireballFistsDedTargetDist = fireballSpeedFistsDed / fireballFistsDedTargetDist;
                        fireballFistsDedTargetX *= fireballFistsDedTargetDist;
                        fireballFistsDedTargetY *= fireballFistsDedTargetDist;

                        int type = ProjectileID.Fireball;
                        int damage = FireballDamage;

                        int fireballAmount = death ? 2 : 1;
                        Vector2 fireballVelocity = new Vector2(fireballFistsDedTargetX, fireballFistsDedTargetY);
                        for (int i = 0; i < fireballAmount; i++)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileFirePos, fireballVelocity * (1f / (i + 1)), type, damage, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 225;
                        }

                        NPC.netUpdate = true;
                    }

                    // Lasers
                    float shootBoost2 = death ? 4.5f * (1f - (lifeRatio / 2)) : 2.75f * (1f - (lifeRatio / 2));
                    NPC.ai[2] += 1f + shootBoost2;
                    if (enrage)
                        NPC.ai[2] += 4f;

                    if (NPC.ai[2] >= 300f)
                    {
                        NPC.ai[2] = 0f;

                        int projType = ProjectileID.EyeBeam;
                        int dmg = LaserDamage;

                        if (NPC.localAI[1] == 0f)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                projectileFirePos = new Vector2(NPC.Center.X, NPC.Center.Y - 22f * NPC.scale);
                                if (i == 0)
                                    projectileFirePos.X -= 18f * NPC.scale;
                                else
                                    projectileFirePos.X += 18f * NPC.scale;

                                float laserSpeed = death ? 15f : 12f;
                                float laserTargetXDist = Main.player[NPC.target].Center.X - projectileFirePos.X;
                                float laserTargetYDist = Main.player[NPC.target].Center.Y - projectileFirePos.Y;
                                float laserTargetDistance = (float)Math.Sqrt(laserTargetXDist * laserTargetXDist + laserTargetYDist * laserTargetYDist);

                                laserTargetDistance = laserSpeed / laserTargetDistance;
                                laserTargetXDist *= laserTargetDistance;
                                laserTargetYDist *= laserTargetDistance;

                                Vector2 laserVelocity = new Vector2(laserTargetXDist, laserTargetYDist);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int bodyLaser = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileFirePos + laserVelocity.SafeNormalize(Vector2.UnitY) * 40f, laserVelocity, projType, dmg, 0f, Main.myPlayer);
                                    ConfigureLaser(NPC, bodyLaser, enrage, turboEnrage);
                                }
                            }
                        }
                        else if (NPC.localAI[1] != 0f)
                        {
                            projectileFirePos = new Vector2(NPC.Center.X, NPC.Center.Y - 22f * NPC.scale);
                            if (NPC.localAI[1] == -1f)
                                projectileFirePos.X -= 30f * NPC.scale;
                            else if (NPC.localAI[1] == 1f)
                                projectileFirePos.X += 30f * NPC.scale;

                            float extraLaserSpeed = death ? 15f : 12f;
                            float extraLaserTargetX = Main.player[NPC.target].Center.X - projectileFirePos.X;
                            float extraLaserTargetY = Main.player[NPC.target].Center.Y - projectileFirePos.Y;
                            float extraLaserTargetDist = (float)Math.Sqrt(extraLaserTargetX * extraLaserTargetX + extraLaserTargetY * extraLaserTargetY);

                            extraLaserTargetDist = extraLaserSpeed / extraLaserTargetDist;
                            extraLaserTargetX *= extraLaserTargetDist;
                            extraLaserTargetY *= extraLaserTargetDist;

                            Vector2 laserVelocity = new Vector2(extraLaserTargetX, extraLaserTargetY);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int extraLasers = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileFirePos + laserVelocity.SafeNormalize(Vector2.UnitY) * 40f, laserVelocity, projType, dmg, 0f, Main.myPlayer);
                                ConfigureLaser(NPC, extraLasers, enrage, turboEnrage);
                            }
                        }
                    }
                }

                // Special laser spread attack
                else if (NPC.ai[0] == 2f || NPC.ai[0] == 3f)
                {
                    int telegraphTime = 60;
                    int endTime = 120;
                    Vector2 spawnLocation = new Vector2(NPC.Center.X + (30f * NPC.scale * NPC.localAI[1]), NPC.Center.Y - 22f * NPC.scale);
                    if (NPC.ai[1] == 1f)
                    {
                        SparkleParticle eyeTele = new(spawnLocation, Vector2.Zero, Color.Yellow, Color.White, 1.25f * NPC.scale, telegraphTime, MathHelper.Pi * 0.02f, needed: true);
                        GeneralParticleHandler.SpawnParticle(eyeTele);
                    }

                    NPC.ai[1]++;
                    if (NPC.ai[1] >= telegraphTime && NPC.ai[1] < endTime && NPC.ai[1] % 2f == 0f)
                    {
                        // Manually plays the sound at a slower rate (the sound from the lasers is disabled by setting ai[1])
                        if (NPC.ai[1] % 10f == 0f)
                            SoundEngine.PlaySound(SoundID.Item33, spawnLocation);

                        float laserFireAngle = MathHelper.ToRadians((NPC.ai[1] - telegraphTime + 20) * (death ? 2.35f : 2f));
                        Vector2 laserVelocity = Vector2.UnitY.RotatedBy(laserFireAngle * -NPC.localAI[1]) * (death ? 15f : 12f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int extraLasers = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation + laserVelocity.SafeNormalize(Vector2.UnitY) * 40f, laserVelocity, ProjectileID.EyeBeam, LaserDamage, 0f, Main.myPlayer, 0f, 1f);
                            ConfigureLaser(NPC, extraLasers, enrage, turboEnrage);
                        }
                    }

                    // Do another attack in the opposite direction in Death Mode
                    if (death && NPC.ai[0] == 2f && NPC.ai[1] >= endTime)
                    {
                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.localAI[1] = -NPC.localAI[1];
                        NPC.netUpdate = true;
                    }

                    if (NPC.ai[1] >= endTime + 30)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                // Laser fire if arms are dead
                if ((!leftFistAlive && !rightFistAlive) || death || Main.getGoodWorld)
                {
                    if (NPC.ai[0] <= 1f)
                        NPC.ai[0] = 1f;
                }
                else
                    NPC.ai[0] = 0f;

                return false;
            }

            public override void FindFrame(Mod mod, int frameHeight)
            {
                // Used to force the head to look to the sides for the laser spread attack
                if (NPC.ai[0] == 2f || NPC.ai[0] == 3f)
                {
                    if (NPC.localAI[1] == 1f)
                        NPC.frame.Y = frameHeight * 2;
                    else
                        NPC.frame.Y = frameHeight * 4;
                }
            }
        }

        public class HeadFreeAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                    options.aggroRatio = -1f;
                    options.finishThemOff = true;
                    CalamityUtils.CalamityTargeting(NPC, options);
                }

                // Die if body is gone
                if (NPC.golemBoss < 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return false;
                }

                // Percent life remaining
                float golemLifeRatio = Main.npc[NPC.golemBoss].life / (float)Main.npc[NPC.golemBoss].lifeMax;
                float PosDelay = 270f;

                // Phases
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
                bool phase2 = golemLifeRatio < 0.66f;
                bool phase3 = golemLifeRatio < 0.33f;

                // Enrage if the target isn't inside the temple
                bool enrage = !BossRushEvent.BossRushActive;
                bool turboEnrage = false;
                if (Main.player[NPC.target].Center.Y > Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
                {
                    CheckTempleWall(NPC, ref enrage, ref turboEnrage);
                }
                else
                    turboEnrage = Main.getGoodWorld;

                if (Main.getGoodWorld)
                    enrage = true;

                // Float through tiles or not
                NPC.noTileCollide = !Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1) || phase2 || turboEnrage;

                // Immediately trigger the laser spread attack at phase transition thresholds
                if (phase2 && NPC.ai[0] == 0f || phase3 && NPC.ai[0] == 1f)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = PosDelay;
                    calamityGlobalNPC.newAI[3] = 0;
                    NPC.netUpdate = true;
                    NPC.SyncExtraAI();
                }

                // Move to new location
                float maxDistanceDiagonal = 360f;
                float maxDistanceStraight = 480f;
                if (NPC.ai[3] <= 0f)
                {
                    NPC.ai[3] = PosDelay;
                    calamityGlobalNPC.newAI[3] += phase2 ? 1 : 0;
                    if (calamityGlobalNPC.newAI[3] >= 7)
                    {
                        calamityGlobalNPC.newAI[3] = 0;
                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = PosDelay;
                    }

                    // Four positions around target
                    if (!(phase3 || turboEnrage))
                    {
                        if (phase2)
                        {
                            // 0 is top left, then goes counter-clockwise
                            switch ((int)(calamityGlobalNPC.newAI[3] % 4))
                            {
                                case 0:
                                    calamityGlobalNPC.newAI[0] = -maxDistanceDiagonal;
                                    calamityGlobalNPC.newAI[1] = -maxDistanceDiagonal;
                                    break;
                                case 1:
                                    calamityGlobalNPC.newAI[0] = -maxDistanceDiagonal;
                                    calamityGlobalNPC.newAI[1] = maxDistanceDiagonal;
                                    break;
                                case 2:
                                    calamityGlobalNPC.newAI[0] = maxDistanceDiagonal;
                                    calamityGlobalNPC.newAI[1] = maxDistanceDiagonal;
                                    break;
                                case 3:
                                    calamityGlobalNPC.newAI[0] = maxDistanceDiagonal;
                                    calamityGlobalNPC.newAI[1] = -maxDistanceDiagonal;
                                    break;
                            }
                        }

                        // Above target
                        else
                        {
                            calamityGlobalNPC.newAI[0] = Main.rand.NextFloat(-150f, 150f);
                            calamityGlobalNPC.newAI[1] = -maxDistanceStraight * 0.85f;
                        }
                    }

                    NPC.netSpam = 5;
                    NPC.SyncExtraAI();
                    NPC.ForceNetUpdate();
                }

                float positioningInc = enrage ? 6f : phase3 ? 2.5f : phase2 ? 1.8f : 1f;
                NPC.ai[3] -= positioningInc;

                // Move in a circle around the player in final phase
                if (phase3 || turboEnrage)
                {
                    float spinSpeedMult = enrage ? 2.5f : death ? 1.35f : 1.2f;
                    calamityGlobalNPC.newAI[0] = maxDistanceStraight * MathF.Sin(MathHelper.ToRadians(NPC.ai[2] * spinSpeedMult));
                    calamityGlobalNPC.newAI[1] = maxDistanceStraight * MathF.Cos(MathHelper.ToRadians(NPC.ai[2] * spinSpeedMult));
                }
                // Always move above the player during laser spread attack
                if (NPC.ai[0] == 3f)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = -maxDistanceStraight * 0.8f;
                }

                float offsetX = calamityGlobalNPC.newAI[0];
                float offsetY = calamityGlobalNPC.newAI[1];
                Vector2 destination = Main.player[NPC.target].Center + new Vector2(offsetX, offsetY);

                // Velocity and acceleration
                float velocity = (turboEnrage ? 15f : 10f) +
                    (turboEnrage ? 7.5f : phase2 ? 10f : 0f) +
                    (turboEnrage ? 7.5f : phase3 ? 15f : 0f);

                if (enrage)
                    velocity = (phase3 || turboEnrage) ? 35f : 25f;

                float acceleration = NPC.ai[0] == 3f ? 1.5f : phase2 ? 0f : turboEnrage ? 5f : enrage ? 3f : 0.3f;

                // How far Golem's Head is from where it's supposed to be
                Vector2 distanceFromDestination = destination - NPC.Center;
                // Whether Golem can fire projectiles
                bool canFireProjectiles = (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 160f && NPC.ai[0] != 3f) || enrage;

                // Ensure the free head stays still during the attack
                if (NPC.ai[0] == 3f && NPC.ai[1] >= 60f)
                    NPC.velocity = Vector2.Zero;
                else
                    CalamityUtils.SmoothMovement(NPC, 80f, distanceFromDestination, velocity, acceleration, !phase2 || NPC.ai[0] == 3f);

                // Laser spread attack, followed by lingering flame bolts
                if (NPC.ai[0] == 3f)
                {
                    int telegraphTime = 60;
                    int laserEndTime = 120;
                    int fireballStartTime = 160;

                    NPC.ai[1]++;
                    // Grrrrr stop incrementing the damn timer
                    NPC.ai[3] = PosDelay;
                    // Laser spread
                    if (NPC.ai[1] >= telegraphTime && NPC.ai[1] < laserEndTime && NPC.ai[1] % 2f == 0f)
                    {
                        // Manually plays the sound at a slower rate (the sound from the lasers is disabled by setting ai[1])
                        if (NPC.ai[1] % 10f == 0f)
                            SoundEngine.PlaySound(SoundID.Item33, NPC.Center);

                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 spawnLocation = new Vector2(NPC.Center.X + 14f * NPC.scale * i, NPC.Center.Y - 20f * NPC.scale);
                            float laserFireAngle = MathHelper.ToRadians((NPC.ai[1] - telegraphTime + 20) * 2f);
                            Vector2 laserVelocity = -Vector2.UnitY.RotatedBy(laserFireAngle * i) * (death ? 15f : 12f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int spreadLasers = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation + laserVelocity.SafeNormalize(Vector2.UnitY) * 40f, laserVelocity, ProjectileID.EyeBeam, LaserDamage, 0f, Main.myPlayer, 0f, 1f);
                                ConfigureLaser(NPC, spreadLasers, enrage, turboEnrage);
                            }
                        }
                    }
                    // Sets the mouth to be open for the upcoming flame bolts
                    if (NPC.ai[1] > laserEndTime)
                        NPC.localAI[0] = 1f;

                    // Flame bolts
                    if (NPC.ai[1] >= fireballStartTime && NPC.ai[1] % 30 == 10f)
                    {
                        Vector2 spawnLocation = NPC.Center + Vector2.UnitY * 20f * NPC.scale;
                        Vector2 fireBoltVelocity = Utils.DirectionTo(spawnLocation, Main.player[NPC.target].Center) * (enrage ? 30f : death ? 16f : 12f);

                        int type = ModContent.ProjectileType<GolemInfernoBolt>();
                        int damage = InfernoBoltDamage;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation, fireBoltVelocity, type, damage, 0f, Main.myPlayer, Main.player[NPC.target].Center.X, Main.player[NPC.target].Center.Y);
                    }

                    // Death fires 2 bolts, Rev fires just 1
                    if (NPC.ai[1] >= fireballStartTime + (death ? 40 : 10))
                    {
                        NPC.ai[0] = phase3 ? 2f : 1f;
                        NPC.ai[1] = 0f;
                        if (phase3)
                            calamityGlobalNPC.newAI[2] = Main.rand.Next(120); // Used as a random start point for phase 3 circling
                        NPC.ai[2] = calamityGlobalNPC.newAI[2];
                        NPC.localAI[0] = 0f;
                        // Needs to be done to properly put it in the top left corner afterwards in phase 2
                        calamityGlobalNPC.newAI[0] = -maxDistanceDiagonal;
                        NPC.netUpdate = true;
                        NPC.SyncExtraAI();
                    }
                }

                // Lasers
                NPC.ai[2] += 1f;

                int laserGateValue = (int)(PosDelay / positioningInc);
                if (canFireProjectiles && Main.netMode != NetmodeID.MultiplayerClient && ((NPC.ai[2] - calamityGlobalNPC.newAI[2]) % (laserGateValue / 2) == 0f))
                {
                    int numLasers = 2;
                    for (int i = 0; i < numLasers; i++)
                    {
                        Vector2 freeHeadProjSpawn = new Vector2(NPC.Center.X, NPC.Center.Y - 20f * NPC.scale);
                        freeHeadProjSpawn.X += 14f * NPC.scale * (i == 1).ToDirectionInt();

                        float freeHeadProjSpeed = 7f + (5f * (1f - golemLifeRatio));
                        Vector2 laserVelocity = Main.player[NPC.target].Center - freeHeadProjSpawn;
                        laserVelocity = laserVelocity.SafeNormalize(Vector2.UnitY) * freeHeadProjSpeed;

                        int type = ProjectileID.EyeBeam;
                        int damage = LaserDamage;
                        int freeHeadLaser = Projectile.NewProjectile(NPC.GetSource_FromAI(), freeHeadProjSpawn + laserVelocity.SafeNormalize(Vector2.UnitY) * 40f, laserVelocity, type, damage, 0f, Main.myPlayer);
                        Main.projectile[freeHeadLaser].timeLeft = enrage ? 600 : 300;
                        if (turboEnrage)
                            Main.projectile[freeHeadLaser].extraUpdates += 1;
                    }
                }

                if (!Main.getGoodWorld)
                {
                    NPC.position += NPC.netOffset;
                    int randDustOffset = Main.rand.Next(2) * 2 - 1;
                    Vector2 randDustPos = NPC.Bottom + new Vector2((float)(randDustOffset * 22) * NPC.scale, -22f * NPC.scale);
                    Dust getGoodDust = Dust.NewDustPerfect(randDustPos, DustID.GoldFlame, (MathHelper.PiOver2 + -MathHelper.PiOver2 * (float)randDustOffset + Main.rand.NextFloatDirection() * MathHelper.PiOver4).ToRotationVector2() * (2f + Main.rand.NextFloat()));
                    Dust dust = getGoodDust;
                    dust.velocity += NPC.velocity;
                    getGoodDust.noGravity = true;
                    getGoodDust = Dust.NewDustPerfect(NPC.Bottom + new Vector2(Main.rand.NextFloatDirection() * 6f * NPC.scale, (Main.rand.NextFloat() * -4f - 8f) * NPC.scale), DustID.GoldFlame, Vector2.UnitY * (2f + Main.rand.NextFloat()));
                    getGoodDust.fadeIn = 0f;
                    getGoodDust.scale = 0.7f + Main.rand.NextFloat() * 0.5f;
                    getGoodDust.noGravity = true;
                    dust = getGoodDust;
                    dust.velocity += NPC.velocity;
                    NPC.position -= NPC.netOffset;
                }

                return false;
            }

            public override bool PreDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                // Draw the head as usual.
                Texture2D golemHeadTexture = TextureAssets.Npc[NPC.type].Value;
                Vector2 headDrawPosition = NPC.Center - screenPos;
                spriteBatch.Draw(golemHeadTexture, headDrawPosition, NPC.frame, NPC.GetAlpha(drawColor), 0f, NPC.frame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);

                // Draw the eyes. The way vanilla handles this is hardcoded bullshit that cannot handle different hitboxes and thus requires rewriting.
                Color eyeColor = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, 0);
                Vector2 eyesDrawPosition = headDrawPosition - NPC.scale * new Vector2(1f, 12f);
                Rectangle eyesFrame = new Rectangle(0, 0, TextureAssets.Golem[1].Value.Width, TextureAssets.Golem[1].Value.Height / 2);
                spriteBatch.Draw(TextureAssets.Golem[1].Value, eyesDrawPosition, eyesFrame, eyeColor, 0f, eyesFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);

                // Draw the glowmasks.
                int frameCounter = (int)NPC.frameCounter / 4;
                Rectangle frame = TextureAssets.Extra[ExtrasID.GolemLights4].Value.Frame(1, 8);
                frame.Y += frame.Height * 2 * frameCounter + NPC.frame.Y;
                Rectangle glowFrame = frame;
                spriteBatch.Draw(TextureAssets.Extra[ExtrasID.GolemLights4].Value, eyesDrawPosition, glowFrame, eyeColor, 0f, glowFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
                frame = NPC.frame;
                Rectangle glowFrame2 = frame;
                spriteBatch.Draw(TextureAssets.Extra[ExtrasID.GolemLights5].Value, eyesDrawPosition, glowFrame2, eyeColor, 0f, glowFrame2.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);

                // Draw the sparkle telegraphs for the laser spread attack if applicable.
                if (NPC.ai[0] == 3f && NPC.ai[1] <= 60f)
                {
                    spriteBatch.SetBlendState(BlendState.Additive);
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Texture2D sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle2").Value;
                        Vector2 sparkleDraw = headDrawPosition + new Vector2(14f * i, -15f) * NPC.scale;
                        var drawFade = Color.Yellow * Utils.GetLerpValue(0, 30, 60f - NPC.ai[1], true);
                        spriteBatch.Draw(sparkle, sparkleDraw, null, drawFade, MathHelper.Pi * 0.02f * NPC.ai[1] * i, sparkle.Size() / 2f, 1.25f * NPC.scale, SpriteEffects.None, 0f);
                    }
                    spriteBatch.SetBlendState(BlendState.AlphaBlend);
                }
                return false;
            }
        }
    }
}
