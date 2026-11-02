using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class CrimulanPaladin : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[Type] = 6;
        }
        public static int GlobDamage = 15; // 60

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 42; // 84
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 1.1f;
            NPC.defense = 12;
            NPC.LifeMaxNERB(7500, 9000, 160000);
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.KingSlime;
            NPC.value = 0f;
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.slimeGodRed = NPC.whoAmI;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || NPC.localAI[1] == 1f || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            NPC.defense = NPC.defDefense;
            int setDamage = NPC.defDamage;
            if (NPC.localAI[1] == 1f)
            {
                NPC.defense = NPC.defDefense + 24;
                setDamage += 25;
            }

            float scale = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 0.6f : Main.getGoodWorld ? 0.8f : 1f;
            NPC.aiAction = 0;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            Player player = SlimeGodPaladinBehavior.TargetAndHandleDespawn(NPC, 3f, true);

            if (lifeRatio <= 0.5f && Main.netMode != NetmodeID.MultiplayerClient && expertMode)
            {
                if (Main.zenithWorld)
                {
                    int type = ModContent.ProjectileType<UnstableCrimulanGlob>();
                    for (int i = 0; i < 30; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1499, 1500) * 0.01f, (float)Main.rand.Next(-1499, 1500) * 0.01f, type, 35, 0f);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
                Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X - 30, (int)spawnAt.Y, ModContent.NPCType<SplitCrimulanPaladin>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X + 30, (int)spawnAt.Y, ModContent.NPCType<SplitCrimulanPaladin>());
                if (Main.zenithWorld && NPC.CountNPCS(ModContent.NPCType<SplitEbonianPaladin>()) < 3) // Split into 3 slimes if the other large slime hasn't split yet
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X, (int)spawnAt.Y - 30, ModContent.NPCType<SplitCrimulanPaladin>());

                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool enraged = true;
            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                    enraged = false;
            }

            if (NPC.localAI[1] != 1f)
            {
                if (enraged)
                    NPC.defense = NPC.defDefense * 2;
            }

            // Teleport
            float teleportGateValue = 720f;
            if (!player.dead && NPC.timeLeft > 10 && calamityGlobalNPC.newAI[0] >= teleportGateValue && NPC.ai[0] == 0f && NPC.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down dramatically
                NPC.velocity.X *= 0.5f;

                NPC.ai[0] = 5f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.netUpdate = true;
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];

                    Vector2 teleportDestination = SlimeGodPaladinTeleport.FindDestination(NPC, player, 800f);
                    NPC.localAI[0] = teleportDestination.X;
                    NPC.localAI[3] = teleportDestination.Y;
                    calamityGlobalNPC.newAI[0] = 0f;
                }
            }

            // Get ready to teleport by increasing ai[3]
            if (calamityGlobalNPC.newAI[0] < teleportGateValue)
            {
                if (!Collision.CanHitLine(NPC.Center, 0, 0, player.Center, 0, 0) || Math.Abs(NPC.Top.Y - player.Bottom.Y) > 320f)
                    calamityGlobalNPC.newAI[0] += death ? 3f : 2f;
                else
                    calamityGlobalNPC.newAI[0] += 1f;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, NPC.Center) * 0.005f;

            if (NPC.ai[0] == 0f)
            {
                bool phaseThroughTilesToReachTarget = Vector2.Distance(player.Center, NPC.Center) > 2400f || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (Main.netMode != NetmodeID.MultiplayerClient && phaseThroughTilesToReachTarget)
                {
                    // Set damage
                    NPC.damage = setDamage;

                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
                else if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += 1f;

                    float jumpGateValue = 50f;
                    float velocityX = death ? 10f : revenge ? 9f : expertMode ? 8f : 6f;
                    if (revenge)
                    {
                        float moveBoost = death ? 45f * (1f - lifeRatio) : 30f * (1f - lifeRatio);
                        float speedBoost = death ? 4.5f * (1f - lifeRatio) : 3f * (1f - lifeRatio);
                        jumpGateValue -= moveBoost;
                        velocityX += speedBoost;
                    }

                    float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                    float speedMult = 1f;
                    if (distanceBelowTarget > 0f)
                        speedMult += distanceBelowTarget * 0.002f;

                    if (speedMult > 2f)
                        speedMult = 2f;

                    float velocityY = 4f;
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                        velocityY += 2f;

                    if (NPC.ai[1] > jumpGateValue)
                    {
                        // Set damage
                        NPC.damage = setDamage;

                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] >= 2f)
                        {
                            NPC.ai[3] = 0f;
                            velocityX *= 1.25f;
                        }

                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= velocityY * speedMult;
                        NPC.velocity.X = (velocityX + distanceSpeedBoost) * NPC.direction;
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[1] >= jumpGateValue - 30f)
                        NPC.aiAction = 1;
                }
                else
                {
                    SlimeGodPaladinBehavior.UpdateAirborneJump(NPC, player, setDamage);
                }

                NPC.ai[2] += 1f;
                if (revenge)
                    NPC.ai[2] += death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);

                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float phaseSwitchGateValue = 360f;
                    bool switchPhase = NPC.ai[2] >= phaseSwitchGateValue;
                    if (switchPhase)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            switch ((int)NPC.localAI[2])
                            {
                                default:
                                case 1:
                                    NPC.ai[0] = 2f;
                                    break;

                                case 2:
                                    NPC.ai[0] = 1f;
                                    break;

                            }

                            if (NPC.ai[0] == 1f)
                            {
                                // Set damage
                                NPC.damage = setDamage;

                                NPC.noTileCollide = true;
                                NPC.velocity.Y = death ? -10f : revenge ? -9f : expertMode ? -8f : -7f;
                            }

                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] >= phaseSwitchGateValue - 30f)
                        NPC.aiAction = 1;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 targetCenter = player.Center;
                targetCenter.Y -= 350f;
                Vector2 targetDist = targetCenter - NPC.Center;

                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    targetDist = player.Center - NPC.Center;
                    targetDist.Normalize();
                    targetDist *= death ? 10f : revenge ? 9f : expertMode ? 8f : 7f;
                    NPC.velocity = (NPC.velocity * 4f + targetDist) / 5f;

                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 1.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = targetDist;
                    }
                }
                else
                {
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 40f && NPC.Center.Y < player.Center.Y - 300f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 1f;
                        return;
                    }

                    targetDist.Normalize();
                    targetDist *= (death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + targetDist) / 6f;
                }
            }
            else if (NPC.ai[0] == 1.1f)
            {
                // Set damage
                NPC.damage = setDamage;

                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && (atTargetPosition || NPC.localAI[1] == 0f) && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }

                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        SoundEngine.PlaySound(SlimeGodCore.BigShotSound, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Eruption of slime balls that fall down
                            float projectileVelocity = 8f;
                            int type = ModContent.ProjectileType<UnstableCrimulanGlob>();
                            int damage = GlobDamage;
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = 5;
                            float rotation = MathHelper.ToRadians(45);
                            SlimeGodPaladinBehavior.FireGlobEruption(NPC, destination, numProj, rotation, type, damage, 1.5f, false, false, true, 1f, 0f);

                            // Fire slime balls directly at players with a max of 2
                            if (enraged && expertMode)
                            {
                                SlimeGodPaladinBehavior.FireAtPlayers(NPC, projectileVelocity, type, damage);
                            }
                        }

                        NPC.localAI[2] = NPC.ai[0] - 0.1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 0f)
                    NPC.noTileCollide = true;

                NPC.noGravity = true;

                NPC.velocity.Y += 0.5f;
                float velocityLimit = death ? 15f : revenge ? 14f : expertMode ? 13f : 12f;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
            }
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 15f)
                    {
                        // Set damage
                        NPC.damage = setDamage;

                        NPC.ai[1] = 0f;

                        SlimeGodPaladinBehavior.StartHop(NPC, player, death, revenge, expertMode, distanceSpeedBoost, false);
                    }
                    else
                        NPC.aiAction = 1;
                }
                else
                {
                    // Set damage
                    NPC.damage = setDamage;

                    NPC.velocity.X *= 0.98f;
                    float velocityLimit = (death ? 5.5f : revenge ? 5f : expertMode ? 4.5f : 4f);
                    if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        NPC.velocity.X = -velocityLimit;
                    if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        NPC.velocity.X = velocityLimit;
                }

                if (NPC.ai[2] >= 3f && NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Despawn
            else if (NPC.ai[0] == 3f)
            {
                SlimeGodPaladinBehavior.Despawn(NPC);
            }

            else if (NPC.ai[0] == 4f)
            {
                SlimeGodPaladinBehavior.CatchUpToTarget(NPC, player, setDamage, true);
            }
            else if (NPC.ai[0] == 5f)
            {
                SlimeGodPaladinBehavior.FadeOutForTeleport(NPC, death, ref scale, 6f);

                // Emit teleport dust
                SlimeGodPaladinBehavior.EmitTeleportDust(NPC, Color.Crimson);
            }
            else if (NPC.ai[0] == 6f)
            {
                SlimeGodPaladinBehavior.FadeInAfterTeleport(NPC, death, ref scale);

                // Emit teleport dust
                SlimeGodPaladinBehavior.EmitTeleportDust(NPC, Color.Crimson);
            }

            if (bossLife == 0f && NPC.life > 0)
                bossLife = NPC.lifeMax;

            if (NPC.life > 0)
            {
                SlimeGodPaladinBehavior.UpdateScale(NPC, lifeRatio, scale, false);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int slimeSpawnThreshold = (int)((double)NPC.lifeMax * 0.15);
                    if ((float)(NPC.life + slimeSpawnThreshold) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                        int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                        int slimeType = ModContent.NPCType<CrimsonSlimeSpawn>();
                        if (Main.rand.NextBool(3))
                            slimeType = ModContent.NPCType<CrimsonSlimeSpawn2>();

                        int slimeSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, slimeType);
                        Main.npc[slimeSpawn].SetDefaults(slimeType);
                        Main.npc[slimeSpawn].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[slimeSpawn].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[slimeSpawn].ai[0] = (float)(-1000 * Main.rand.Next(3));
                        Main.npc[slimeSpawn].ai[1] = 0f;
                        if (Main.dedServ && slimeSpawn < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slimeSpawn, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override bool CheckActive() => false;

        public override Color? GetAlpha(Color drawColor)
        {
            Color lightColor = new Color(Main.DiscoR, 100, 150, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : drawColor;
            return newColor * NPC.Opacity;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Color dustColor = Color.Crimson;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }

    // Shared by the four concrete paladins; their attack selection and mod identities stay separate.
    internal static class SlimeGodPaladinBehavior
    {
        internal static Player TargetAndHandleDespawn(NPC npc, float despawnState, bool clearNewAi)
        {
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player player = Main.player[npc.target];
            if (npc.ai[0] != despawnState)
            {
                if (player.dead || !player.active)
                {
                    npc.TargetClosest();
                    player = Main.player[npc.target];
                    if (player.dead || !player.active)
                    {
                        npc.ai[0] = despawnState;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        if (clearNewAi)
                        {
                            npc.Calamity().newAI[0] = 0f;
                        }
                        npc.netUpdate = true;
                    }
                }
                else if (npc.timeLeft < 1800)
                    npc.timeLeft = 1800;
            }

            return player;
        }

        internal static void CatchUpToTarget(NPC npc, Player player, int setDamage, bool resetJumpCounter)
        {
            npc.damage = setDamage;

            if (npc.velocity.X > 0f)
                npc.direction = 1;
            else
                npc.direction = -1;

            npc.spriteDirection = npc.direction;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            Vector2 distanceFromTarget = player.Center - npc.Center;
            distanceFromTarget.Y -= 16f;
            if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromTarget.Length() < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height) &&
                Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
            {
                npc.damage = 0;

                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                // Split paladins retain this counter when resuming their attacks.
                if (resetJumpCounter)
                {
                    npc.ai[3] = 0f;
                }
                npc.netUpdate = true;
            }

            if (distanceFromTarget.Length() > 12f)
            {
                distanceFromTarget.Normalize();
                distanceFromTarget *= 12f;
            }

            npc.velocity = (npc.velocity * 4f + distanceFromTarget) / 5f;
        }

        internal static void UpdateScale(NPC npc, float lifeRatio, float scale, bool split)
        {
            float scaleRatio = lifeRatio;
            scaleRatio = scaleRatio * 0.5f + 0.75f;
            scaleRatio *= scale;

            if (scaleRatio != npc.scale)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)npc.height;
                if (split)
                {
                    npc.scale = scaleRatio * 0.75f;
                }
                else
                {
                    npc.scale = scaleRatio;
                }
                npc.width = (int)(150f * npc.scale);
                npc.height = (int)(92f * npc.scale);
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)npc.height;
            }
        }

        internal static void UpdateAirborneJump(NPC npc, Player player, int setDamage)
        {
            npc.damage = setDamage;

            npc.velocity.X *= 0.99f;
            if (npc.direction < 0 && npc.velocity.X > -1f)
                npc.velocity.X = -1f;
            if (npc.direction > 0 && npc.velocity.X < 1f)
                npc.velocity.X = 1f;

            if (!player.dead)
            {
                if (npc.velocity.Y > 0f && npc.Bottom.Y > player.Top.Y)
                    npc.noTileCollide = false;
                else if (Collision.CanHit(npc.position, npc.width, npc.height, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.noTileCollide = false;
                else
                    npc.noTileCollide = true;
            }
        }

        internal static void FireAtPlayers(NPC npc, float projectileVelocity, int type, int damage)
        {
            List<int> targets = new List<int>();
            foreach (Player plr in Main.ActivePlayers)
            {
                if (!plr.dead)
                    targets.Add(plr.whoAmI);

                if (targets.Count > 1)
                    break;
            }
            foreach (int t in targets)
            {
                Vector2 projFireDirection = Vector2.Normalize(Main.player[t].Center - npc.Center) * projectileVelocity;
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(projFireDirection) * 30f * npc.scale, projFireDirection, type, damage, 0f, Main.myPlayer);
            }
        }

        internal static void FireGlobEruption(NPC npc, Vector2 destination, int numProj, float rotation, int type, int damage, float velocityMultiplier, bool spawnAboveCenter, bool omitCenterShots, bool includeProjectileAI, float ai0, float ai1)
        {
            for (int i = 0; i < numProj; i++)
            {
                if (!omitCenterShots || i < 4 || i > 6)
                {
                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                    if (includeProjectileAI)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnAboveCenter ? npc.Center + Vector2.UnitY * 30f * npc.scale + Vector2.Normalize(perturbedSpeed) * 30f * npc.scale : npc.Center + Vector2.Normalize(perturbedSpeed) * 30f * npc.scale, perturbedSpeed * velocityMultiplier, type, damage, 0f, Main.myPlayer, ai0, ai1);
                    }
                    else
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnAboveCenter ? npc.Center + Vector2.UnitY * 30f * npc.scale + Vector2.Normalize(perturbedSpeed) * 30f * npc.scale : npc.Center + Vector2.Normalize(perturbedSpeed) * 30f * npc.scale, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                    }
                }
            }
        }

        internal static void StartHop(NPC npc, Player player, bool death, bool revenge, bool expertMode, float distanceSpeedBoost, bool ebonian)
        {
            npc.velocity.Y -= ebonian ? 6f : 3f;
            if (player.position.Y + player.height < npc.Center.Y)
                npc.velocity.Y -= ebonian ? 1.2f : 1.25f;
            if (player.position.Y + player.height < npc.Center.Y - 40f)
                npc.velocity.Y -= ebonian ? 1.4f : 1.5f;
            if (player.position.Y + player.height < npc.Center.Y - 80f)
                npc.velocity.Y -= ebonian ? 1.7f : 1.75f;
            if (player.position.Y + player.height < npc.Center.Y - 120f)
                npc.velocity.Y -= 2f;
            if (player.position.Y + player.height < npc.Center.Y - 160f)
                npc.velocity.Y -= ebonian ? 2.2f : 2.25f;
            if (player.position.Y + player.height < npc.Center.Y - 200f)
                npc.velocity.Y -= ebonian ? 2.4f : 2.5f;
            if (!Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                npc.velocity.Y -= 2f;

            npc.velocity.X = ((death ? (ebonian ? 11f : 14f) : revenge ? (ebonian ? 10f : 13f) : expertMode ? (ebonian ? 9f : 12f) : (ebonian ? 8f : 11f)) + distanceSpeedBoost) * npc.direction;
            npc.ai[2] += 1f;
        }

        internal static void Despawn(NPC npc)
        {
            npc.damage = 0;

            npc.noTileCollide = true;
            npc.Opacity -= 0.03f;

            if (npc.timeLeft > 10)
                npc.timeLeft = 10;

            if (npc.Opacity < 0f)
                npc.Opacity = 0f;

            npc.velocity.X *= 0.98f;
        }

        internal static void FadeOutForTeleport(NPC npc, bool death, ref float scale, float nextState)
        {
            npc.damage = 0;

            npc.aiAction = 1;
            npc.ai[1] += 1f;
            float teleportTime = death ? 30f : 40f;
            scale = MathHelper.Clamp((teleportTime - npc.ai[1]) / teleportTime, 0f, 1f);
            scale = 0.5f + scale * 0.5f;
            if (npc.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.Bottom = new Vector2(npc.localAI[0], npc.localAI[3]);
                npc.ai[0] = nextState;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[1] >= teleportTime * 2f)
            {
                npc.ai[0] = nextState;
                npc.ai[1] = 0f;
            }
        }

        internal static void FadeInAfterTeleport(NPC npc, bool death, ref float scale)
        {
            npc.damage = 0;

            npc.ai[1] += 1f;
            float teleportEndTime = death ? 15f : 20f;
            scale = MathHelper.Clamp(npc.ai[1] / teleportEndTime, 0f, 1f);
            scale = 0.5f + scale * 0.5f;
            if (npc.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
                npc.TargetClosest();
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[1] >= teleportEndTime * 2f)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.TargetClosest();
            }
        }

        internal static void EmitTeleportDust(NPC npc, Color color)
        {
            Color dustColor = color;
            dustColor.A = 150;
            for (int i = 0; i < 10; i++)
            {
                int crimsonDust = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, DustID.TintableDust, npc.velocity.X, npc.velocity.Y, npc.alpha, dustColor, 2f);
                Main.dust[crimsonDust].noGravity = true;
                Main.dust[crimsonDust].velocity *= 0.5f;
            }
        }
    }
}
