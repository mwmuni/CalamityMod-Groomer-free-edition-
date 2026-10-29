using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    // This is the most horrible abomination of code I have ever seen in my life.
    // CIT 20OCT2025: Despite this, I still got bored enough that I decided to refactor the entire thing.
    // Hopefully it should be far more readable.
    public class MoonLordAI : VanillaAIOverride
    {
        public static readonly SoundStyle DeathrayChargeSound = new("CalamityMod/Sounds/Custom/MoonLordLaserCharge");

        // Vanilla values
        public static int BoltDamage = 30; // 120
        public static int EyeDamage = 30; // 120
        public static int SphereDamage = 40; // 160
        public static int DeathrayDamage = 75; // 300

        public static int TrueEyeBoltDamage = 35; // 140
        public static int TrueEyeEyeDamage = 35; // 140
        public static int TrueEyeDeathrayDamage = 50; // 200
        public static int TrueEyeSphereDamage = 55; // 220

        // Vanilla values (GFB)
        public static int MoonBoulderDamage = 70; // 280

        private static float AdjustForAggressionLevel(float value, int aggressionLevel, float level6Adjustment, float level5Adjustment, float level3Adjustment, float level2Adjustment, float level1Adjustment)
        {
            switch (aggressionLevel)
            {
                case 6:
                    return value + level6Adjustment;
                case 5:
                    return value + level5Adjustment;
                case 4:
                    return value;
                case 3:
                    return value + level3Adjustment;
                case 2:
                    return value + level2Adjustment;
                case 1:
                    return value + level1Adjustment;
                default:
                    return value;
            }
        }

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            int aggressionLevel = 4;
            if (NPC.type == NPCID.MoonLordCore || NPC.type == NPCID.MoonLordHand || NPC.type == NPCID.MoonLordHead)
            {
                switch (NPC.CountNPCS(NPCID.MoonLordFreeEye))
                {
                    case 0:
                        break;
                    case 1:
                        aggressionLevel = 3;
                        break;
                    case 2:
                        aggressionLevel = 2;
                        break;
                    case 3:
                        aggressionLevel = 1;
                        break;
                    default:
                        break;
                }
            }

            if (death)
                aggressionLevel = 5;
            if (Main.getGoodWorld)
                aggressionLevel = 6;

            if (NPC.type == NPCID.MoonLordCore)
                BuffedMoonLordCoreAI(aggressionLevel);
            else if (NPC.type == NPCID.MoonLordHead)
                BuffedMoonLordHeadAI(aggressionLevel);
            else if (NPC.type == NPCID.MoonLordHand)
                BuffedMoonLordHandAI(aggressionLevel);
            else if (NPC.type == NPCID.MoonLordFreeEye)
                BuffedTrueEyeAI();
            else if (NPC.type == NPCID.MoonLordLeechBlob)
                BuffedMoonLeechBlobAI();

            return false;
        }

        public void BuffedMoonLordCoreAI(int aggressionLevel)
        {
            // Play a random Moon Lord sound
            if (NPC.ai[0] != -1f && NPC.ai[0] != 2f && Main.rand.NextBool(200))
            {
                SoundStyle voiceSound = Utils.SelectRandom(Main.rand,
                [
                        SoundID.Zombie93,
                        SoundID.Zombie94,
                        SoundID.Zombie95,
                        SoundID.Zombie96,
                        SoundID.Zombie97,
                        SoundID.Zombie98,
                        SoundID.Zombie99
                ]);
                SoundEngine.PlaySound(voiceSound, NPC.Center);
            }

            // Start the AI
            if (NPC.localAI[3] == 0f)
            {
                NPC.netUpdate = true;
                NPC.localAI[3] = 1f;
                NPC.ai[0] = -1f;
            }

            // Teleport when target gets too far
            if (NPC.ai[0] == -2f)
            {
                NPC.dontTakeDamage = true;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] == 30f)
                    SoundEngine.PlaySound(SoundID.Zombie92, NPC.Center);

                if (NPC.ai[1] < 60f)
                    MoonlordDeathDrama.RequestLight(NPC.ai[1] / 30f, NPC.Center);

                if (NPC.ai[1] == 60f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                }
            }

            // Spawn head and hands
            if (NPC.ai[0] == -1f)
            {
                NPC.dontTakeDamage = true;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] == 30f)
                    SoundEngine.PlaySound(SoundID.Zombie92, NPC.Center);

                if (NPC.ai[1] < 60f)
                    MoonlordDeathDrama.RequestLight(NPC.ai[1] / 30f, NPC.Center);

                if (NPC.ai[1] == 60f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.type == NPCID.MoonLordCore)
                    {
                        NPC.netUpdate = true;

                        for (int i = 0; i < 2; i++)
                        {
                            int handSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + i * 800 - 400, (int)NPC.Center.Y - 100, NPCID.MoonLordHand, NPC.whoAmI);
                            Main.npc[handSpawn].ai[2] = i; // Used to differentiate between left and right hands
                            Main.npc[handSpawn].ai[3] = NPC.whoAmI;
                            Main.npc[handSpawn].netUpdate = true;
                            NPC.localAI[i] = handSpawn;
                        }

                        int headSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 400, NPCID.MoonLordHead, NPC.whoAmI);
                        Main.npc[headSpawn].ai[3] = NPC.whoAmI;
                        Main.npc[headSpawn].netUpdate = true;
                        NPC.localAI[2] = headSpawn;
                    }
                }
                // In summary, localAI[0] holds the index of the left hand, localAI[1] holds the index of the right hand, and localAI[2] holds the index of the head.
            }

            // If for whatever reason there are less True Eyes of Cthulhu than there should be, spawn more.
            int trueEyesThatShouldBeActive = 0;
            if (Main.npc[(int)NPC.localAI[0]].Calamity().newAI[0] == 1f)
                trueEyesThatShouldBeActive++;
            if (Main.npc[(int)NPC.localAI[1]].Calamity().newAI[0] == 1f)
                trueEyesThatShouldBeActive++;
            if (Main.npc[(int)NPC.localAI[2]].Calamity().newAI[0] == 1f)
                trueEyesThatShouldBeActive++;

            if (NPC.CountNPCS(NPCID.MoonLordFreeEye) < trueEyesThatShouldBeActive)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int totalSpawns = NPC.NewNPC(NPC.GetSource_FromAI(), (int)Main.npc[(int)NPC.localAI[2]].Center.X, (int)Main.npc[(int)NPC.localAI[2]].Center.Y, NPCID.MoonLordFreeEye);
                    Main.npc[totalSpawns].ai[3] = NPC.whoAmI;
                    Main.npc[totalSpawns].netUpdate = true;
                }
            }

            // Fly near target, don't take damage
            if (NPC.ai[0] == 0f)
            {
                NPC.dontTakeDamage = true;
                NPC.TargetClosest(false);

                Vector2 targetDistance = Main.player[NPC.target].Center - NPC.Center;
                if (targetDistance.Length() > 20f)
                {
                    float velocity = 9.25f;
                    velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.25f, -0.5f, -0.75f);
                    // Move slower if the head is doing the phantasmal deathray
                    if (Main.npc[(int)NPC.localAI[2]].ai[0] == 1f)
                        velocity -= 2.25f;

                    Vector2 desiredVelocity = Vector2.Normalize(targetDistance - NPC.velocity) * velocity;
                    Vector2 currentVelocity = NPC.velocity;
                    NPC.SimpleFlyMovement(desiredVelocity, 0.5f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, currentVelocity, 0.5f);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Despawn if other parts aren't there
                    bool shouldDespawn = false;
                    if (NPC.localAI[0] < 0f || NPC.localAI[1] < 0f || NPC.localAI[2] < 0f)
                        shouldDespawn = true;
                    else if (!Main.npc[(int)NPC.localAI[0]].active || Main.npc[(int)NPC.localAI[0]].type != NPCID.MoonLordHand)
                        shouldDespawn = true;
                    else if (!Main.npc[(int)NPC.localAI[1]].active || Main.npc[(int)NPC.localAI[1]].type != NPCID.MoonLordHand)
                        shouldDespawn = true;
                    else if (!Main.npc[(int)NPC.localAI[2]].active || Main.npc[(int)NPC.localAI[2]].type != NPCID.MoonLordHead)
                        shouldDespawn = true;

                    if (shouldDespawn)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }

                    // Take damage if other parts are down
                    bool coreIsOpen = true;
                    if (Main.npc[(int)NPC.localAI[0]].Calamity().newAI[0] != 1f)
                        coreIsOpen = false;
                    if (Main.npc[(int)NPC.localAI[1]].Calamity().newAI[0] != 1f)
                        coreIsOpen = false;
                    if (Main.npc[(int)NPC.localAI[2]].Calamity().newAI[0] != 1f)
                        coreIsOpen = false;

                    if (coreIsOpen)
                    {
                        NPC.ai[0] = 1f;
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                    }
                }
            }

            // Fly near target, take damage
            else if (NPC.ai[0] == 1f)
            {
                NPC.dontTakeDamage = false;
                NPC.TargetClosest(false);

                Vector2 targetDistanceVulnerable = Main.player[NPC.target].Center - NPC.Center;
                if (targetDistanceVulnerable.Length() > 20f)
                {
                    float velocity = 9.25f;
                    velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.25f, -0.5f, -0.75f);
                    // Move slower if the head is doing the phantasmal deathray
                    if (Main.npc[(int)NPC.localAI[2]].ai[0] == 1f)
                        velocity -= 2f;

                    Vector2 desiredVelocity = Vector2.Normalize(targetDistanceVulnerable - NPC.velocity) * velocity;
                    Vector2 currentVelocity = NPC.velocity;
                    NPC.SimpleFlyMovement(desiredVelocity, 0.5f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, currentVelocity, 0.5f);
                }
            }

            // Death effects
            else if (NPC.ai[0] == 2f)
            {
                NPC.dontTakeDamage = true;
                NPC.Calamity().ShouldCloseHPBar = true;
                NPC.velocity = new Vector2(NPC.direction, -0.5f);

                NPC.ai[1] += 1f;
                if (NPC.ai[1] < 60f)
                    MoonlordDeathDrama.RequestLight(NPC.ai[1] / 60f, NPC.Center);

                // Kill all projectiles and the True Eyes once the screen fades to white
                if (NPC.ai[1] == 60f)
                {
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if ((projectile.type == ProjectileID.MoonLeech || projectile.type == ProjectileID.PhantasmalBolt ||
                            projectile.type == ProjectileID.PhantasmalDeathray || projectile.type == ProjectileID.PhantasmalEye ||
                            projectile.type == ProjectileID.PhantasmalSphere))
                            projectile.Kill();
                    }

                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.type == NPCID.MoonLordFreeEye)
                        {
                            n.HitEffect(0, 9999.0);
                            n.active = false;
                        }
                    }
                }

                // Dust and smoke effects
                if (NPC.ai[1] % 3f == 0f && NPC.ai[1] < 580f && NPC.ai[1] > 60f)
                {
                    Vector2 randPositionOffset = Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(20f, 420f);
                    Vector2 dustPos = NPC.Center + randPositionOffset;
                    Point dustPosTileCoords = dustPos.ToTileCoordinates();
                    bool inOpenSpace = WorldGen.InWorld(dustPosTileCoords.X, dustPosTileCoords.Y, 0) && !WorldGen.SolidTile(dustPosTileCoords.X, dustPosTileCoords.Y);

                    float dustScale = Main.rand.NextFloat(1f, 2f);
                    float fadeIn = Main.rand.NextFloat(0.4f, 1.4f);

                    if (inOpenSpace)
                    {
                        float randDustAmt = Main.rand.Next(6, 19);
                        //MoonlordDeathDrama.AddExplosion(npcPosition);
                        for (int j = 0; j < randDustAmt * 2; j++)
                        {
                            float dustRotation = Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.TwoPi / randDustAmt * j;
                            Dust vortex = Dust.NewDustPerfect(dustPos, DustID.Vortex, Vector2.UnitY.RotatedBy(dustRotation) * Main.rand.NextFloat(1.6f, 9.6f), Scale: dustScale);
                            vortex.noGravity = true;
                            vortex.fadeIn = fadeIn;
                        }
                    }

                    for (float k = 0f; k < NPC.ai[1] / 60f; k++)
                    {
                        Vector2 randPosOffset = Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(20f, 820f);
                        Vector2 smokePos = NPC.Center + randPosOffset;
                        Point smokePosTileCoords = smokePos.ToTileCoordinates();
                        bool smokeOpenSpace = WorldGen.InWorld(smokePosTileCoords.X, smokePosTileCoords.Y, 0) && !WorldGen.SolidTile(smokePosTileCoords.X, smokePosTileCoords.Y);

                        if (smokeOpenSpace)
                        {
                            Dust openDust = Dust.NewDustPerfect(smokePos, Main.rand.NextBool() ? DustID.Smoke : DustID.Vortex, -Vector2.UnitY * Main.rand.NextFloat(0.9f, 7.5f), Scale: dustScale);
                            openDust.noGravity = true;
                            openDust.fadeIn = fadeIn;
                        }
                    }

                }

                // Explosion effects
                if (NPC.ai[1] % 15f == 0f && NPC.ai[1] < 480f && NPC.ai[1] >= 90f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 randomOffset = Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(20f, 420f);
                    Vector2 npcOffset = NPC.Center + randomOffset;
                    Point npcOffsetTileCoords = npcOffset.ToTileCoordinates();
                    bool inOpenSpace = WorldGen.InWorld(npcOffsetTileCoords.X, npcOffsetTileCoords.Y, 0) && !WorldGen.SolidTile(npcOffsetTileCoords.X, npcOffsetTileCoords.Y);

                    if (inOpenSpace)
                    {
                        float smokeRotation = Main.rand.NextBool().ToDirectionInt() * (MathHelper.Pi / 8f + Main.rand.NextFloat(MathHelper.PiOver4));
                        Vector2 smokeVelocity = -Vector2.UnitY.RotatedBy(smokeRotation) * Main.rand.NextFloat(3f, 6f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), npcOffset, smokeVelocity, ProjectileID.BlowupSmokeMoonlord, 0, 0f, Main.myPlayer);
                    }
                }

                if (NPC.ai[1] == 1f)
                    SoundEngine.PlaySound(SoundID.NPCDeath61, NPC.Center);

                if (NPC.ai[1] >= 480f)
                    MoonlordDeathDrama.RequestLight((NPC.ai[1] - 480f) / 120f, NPC.Center);

                // Actually kill the boss at the end of the animation.
                if (NPC.ai[1] >= 600f)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 1337.0); // A HitEffect that deals exactly 1337 damage is what tells the game to spawn Moon Lord's skeleton gores.
                    NPC.checkDead();

                    // Despawn all the other parts
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.type == NPCID.MoonLordHand || n.type == NPCID.MoonLordHead)
                        {
                            n.active = false;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }

                    NPC.active = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);

                    return;
                }
            }

            // Despawn effects
            else if (NPC.ai[0] == 3f)
            {
                NPC.dontTakeDamage = true;
                Vector2 despawnVelocityLerp = new Vector2(NPC.direction, -0.5f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, despawnVelocityLerp, 0.98f);

                NPC.ai[1] += 1f;
                if (NPC.ai[1] < 60f)
                    MoonlordDeathDrama.RequestLight(NPC.ai[1] / 40f, NPC.Center);

                // Kill all projectiles, True Eyes, and gores once the screen fades to white
                if (NPC.ai[1] == 40f)
                {
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if ((projectile.type == ProjectileID.MoonLeech || projectile.type == ProjectileID.PhantasmalBolt ||
                            projectile.type == ProjectileID.PhantasmalDeathray || projectile.type == ProjectileID.PhantasmalEye ||
                            projectile.type == ProjectileID.PhantasmalSphere))
                            projectile.Kill();
                    }

                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.type == NPCID.MoonLordFreeEye)
                        {
                            n.HitEffect(0, 9999.0);
                            n.active = false;
                        }
                    }

                    for (int l = 0; l < Main.maxGore; l++)
                    {
                        Gore gore2 = Main.gore[l];
                        if (gore2.active && gore2.type >= GoreID.MoonLordHeart1 && gore2.type <= GoreID.MoonLordHeart4)
                            gore2.active = false;
                    }
                }

                if (NPC.ai[1] >= 60f)
                {
                    // Despawn all the other parts first
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.type == NPCID.MoonLordHand || n.type == NPCID.MoonLordHead)
                        {
                            n.active = false;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }

                    NPC.active = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);

                    NPC.LunarApocalypseIsUp = false;
                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.WorldData, -1, -1, null);

                    return;
                }
            }

            // Despawn
            bool preventDespawn = NPC.ai[0] == -2f || NPC.ai[0] == -1f || NPC.ai[0] == 2f || NPC.ai[0] == 3f || (Main.player[NPC.target].active && !Main.player[NPC.target].dead);
            if (!preventDespawn)
            {
                foreach (Player p in Main.ActivePlayers)
                {
                    if (!p.dead)
                    {
                        preventDespawn = true;
                        break;
                    }
                }
            }
            if (!preventDespawn)
            {
                NPC.ai[0] = 3f;
                NPC.ai[1] = 0f;
                NPC.netUpdate = true;
            }

            // Teleport
            if (NPC.ai[0] >= 0f && NPC.ai[0] < 2f && Main.netMode != NetmodeID.MultiplayerClient && NPC.Distance(Main.player[NPC.target].Center) > 1800f)
            {
                NPC.ai[0] = -2f;
                NPC.netUpdate = true;
                // Teleports the core
                Vector2 teleportOffset = Main.player[NPC.target].Center - Vector2.UnitY * 150f - NPC.Center;
                NPC.position += teleportOffset;

                // Teleports the left hand
                if (Main.npc[(int)NPC.localAI[0]].active)
                {
                    Main.npc[(int)NPC.localAI[0]].position += teleportOffset;
                    Main.npc[(int)NPC.localAI[0]].netUpdate = true;
                }
                // Teleports the right hand
                if (Main.npc[(int)NPC.localAI[1]].active)
                {
                    Main.npc[(int)NPC.localAI[1]].position += teleportOffset;
                    Main.npc[(int)NPC.localAI[1]].netUpdate = true;
                }
                // Teleports the head
                if (Main.npc[(int)NPC.localAI[2]].active)
                {
                    Main.npc[(int)NPC.localAI[2]].position += teleportOffset;
                    Main.npc[(int)NPC.localAI[2]].netUpdate = true;
                }
                // Teleports the True Eyes
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == NPCID.MoonLordFreeEye)
                    {
                        n.position += teleportOffset;
                        n.netUpdate = true;
                    }
                }
            }
        }

        public void BuffedMoonLordHeadAI(int aggressionLevel)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Despawn
            if (!Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != NPCID.MoonLordCore)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
            }

            // Trigger Daybreak projectiles
            if (NPC.localAI[3] == 13f && !NPC.dontTakeDamage)
                NPC.PopAllAttachedProjectilesAndTakeDamageForThem();

            // Variables
            NPC.dontTakeDamage = NPC.localAI[3] >= 15f;
            if (calamityGlobalNPC.newAI[0] == 1f)
                NPC.dontTakeDamage = true;

            NPC.velocity = Vector2.Zero;
            NPC.Center = Main.npc[(int)NPC.ai[3]].Center - Vector2.UnitY * 400f;
            Vector2 eyeSizeVector = new Vector2(27f, 59f);
            float attackTimer = 0f;
            int phaseAttackTime = 0;
            int mouthAnimationCheck = 0;
            int eyeAnimationCheck = 0;

            // Invulnerable
            if (NPC.ai[0] >= 0f || NPC.ai[0] == -2f)
            {
                // Vanilla sets ai[0] to -2 in checkDead, Calamity makes the head continue to attack after being killed
                if (NPC.ai[0] == -2f)
                {
                    if (calamityGlobalNPC.newAI[0] != 1f)
                        calamityGlobalNPC.newAI[0] = 1f;

                    NPC.life = NPC.lifeMax;
                    NPC.netUpdate = true;
                    NPC.dontTakeDamage = true;
                }

                // Go to death animation
                if (Main.npc[(int)NPC.ai[3]].ai[0] == 2f)
                {
                    NPC.ai[0] = -3f;
                    return;
                }

                // Set up attacks
                float ai0CrossCheck = NPC.ai[0];
                NPC.ai[1] += 1f;
                int attackIncrement = 0;
                int totalAttackTimer = 0;

                // Yes, it is completely true: Moon Lord uses a 4D array to organize its attack pattern.
                // I will use this opportunity to explain to the best of my abilities how this 4D array is organized.
                // The first value is unused; it is always 0. There is vanilla code suggesting this was originally going to be set to a random value from 0-2, but alas.
                // The second value determines what body part to get attacks for. 0 is left hand, 1 is right hand, 2 is head.
                // The third value holds what is actually passed to the AI. 0 contains the value for ai[0], and 1 contains the attack duration in frames, used for ai[1] comparison.
                // The fourth value is what number attack it is in the pattern, from 0-4.
                while (attackIncrement < 5)
                {
                    phaseAttackTime = NPC.MoonLordAttacksArray[0, 2, 1, attackIncrement];
                    if (phaseAttackTime + totalAttackTimer > NPC.ai[1])
                        break;

                    totalAttackTimer += phaseAttackTime;
                    attackIncrement++;
                }

                if (attackIncrement == 5)
                {
                    attackIncrement = 0;
                    NPC.ai[1] = 0f;
                    phaseAttackTime = NPC.MoonLordAttacksArray[0, 2, 1, attackIncrement];
                    totalAttackTimer = 0;
                }

                NPC.ai[0] = NPC.MoonLordAttacksArray[0, 2, 0, attackIncrement];
                attackTimer = (int)NPC.ai[1] - totalAttackTimer;

                if (NPC.ai[0] != ai0CrossCheck)
                    NPC.netUpdate = true;
            }

            // Death animation behavior
            if (NPC.ai[0] == -3f)
            {
                NPC.dontTakeDamage = true;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.Pi / 12f, 0.07f);

                // ai[1] is used here for animating the head, localAI[2] is used here for ensuring the mouth is open
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 32f)
                    NPC.ai[1] = 0f;
                if (NPC.ai[1] < 0f)
                    NPC.ai[1] = 0f;

                if (NPC.localAI[2] < 14f)
                    NPC.localAI[2] += 1f;
            }

            // Setup phase for deathray and leech attacks
            else if (NPC.ai[0] == 0f)
            {
                eyeAnimationCheck = 3;
                NPC.TargetClosest(false);

                Vector2 targetDist = Main.player[NPC.target].Center - NPC.Center + Vector2.UnitY * 22f;
                float deathrayTravelDist = targetDist.Length() / 500f;
                if (deathrayTravelDist > 1f)
                    deathrayTravelDist = 1f;
                deathrayTravelDist = 1f - deathrayTravelDist;
                deathrayTravelDist *= 2f;
                if (deathrayTravelDist > 1f)
                    deathrayTravelDist = 1f;

                NPC.localAI[0] = targetDist.ToRotation(); // localAI[0] controls the angle of the deathray
                NPC.localAI[1] = deathrayTravelDist; // localAI[1] controls the draw location of the eye's pupil, which also controls the relative location of the deathray
                NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 1f, 0.2f); // localAI[2] controls the animation of the mouth, this makes it close
            }

            // Deathray
            if (NPC.ai[0] == 1f)
            {
                if (attackTimer < 180f)
                {
                    // When localAI[1] is 0, the pupil draws in the center of the eye
                    NPC.localAI[1] -= 0.05f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;

                    if (attackTimer >= 60f)
                    {
                        // Play a chargeup sound for the deathray
                        if (attackTimer == 60f)
                            SoundEngine.PlaySound(DeathrayChargeSound, Main.player[NPC.target].Center);

                        // Dust telegraph
                        int deathrayDustAmt = attackTimer >= 120f ? 2 : 1;
                        for (int i = 0; i < deathrayDustAmt; i++)
                        {
                            float deathrayDustScale = i % 2 == 1 ? 1.65f : 0.8f;
                            Vector2 deathrayDustRotation = NPC.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * eyeSizeVector / 2f;

                            Dust deathrayDust = Dust.NewDustDirect(deathrayDustRotation - Vector2.One * 8f, 16, 16, DustID.Vortex, 0f, 0f, Scale: deathrayDustScale);
                            deathrayDust.velocity = Utils.DirectionTo(deathrayDustRotation, NPC.Center) * 0.35f * (10f - deathrayDustAmt * 2f);
                            deathrayDust.noGravity = true;
                            deathrayDust.customData = NPC;
                        }
                    }
                }
                else if (attackTimer < phaseAttackTime - 15f)
                {
                    // Controls the angular speed of the deathray, lower number means it rotates faster
                    if (calamityGlobalNPC.newAI[1] == 0f)
                    {
                        calamityGlobalNPC.newAI[1] = 420f;
                        calamityGlobalNPC.newAI[1] = AdjustForAggressionLevel(calamityGlobalNPC.newAI[1], aggressionLevel, -120f, -60f, 120f, 240f, 360f);
                    }

                    // Fire the deathray
                    if (attackTimer == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.TargetClosest(false);
                        Vector2 deathrayRotationSpeed = Utils.DirectionTo(NPC.Center, Main.player[NPC.target].Center);
                        int deathrayRotationDirection = (deathrayRotationSpeed.X < 0).ToDirectionInt();
                        deathrayRotationSpeed = deathrayRotationSpeed.RotatedBy(-deathrayRotationDirection * MathHelper.TwoPi / 6f);
                        float angularSpeed = deathrayRotationDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1];

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, deathrayRotationSpeed, ProjectileID.PhantasmalDeathray, DeathrayDamage, 0f, Main.myPlayer, angularSpeed, NPC.whoAmI);
                        NPC.ai[2] = (deathrayRotationSpeed.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * deathrayRotationDirection;
                        NPC.netUpdate = true;
                    }

                    // When localAI[1] is 1, the pupil draws at the edge of the eye, in the direction determined by localAI[0]
                    NPC.localAI[1] += 0.05f;
                    if (NPC.localAI[1] > 1f)
                        NPC.localAI[1] = 1f;

                    float deathrayFaceDirection = (NPC.ai[2] >= 0f).ToDirectionInt();
                    float deathrayTimer = NPC.ai[2];
                    if (deathrayTimer < 0f)
                        deathrayTimer *= -1f;

                    deathrayTimer += deathrayFaceDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1] - MathHelper.Pi;
                    NPC.localAI[0] = deathrayTimer;
                    NPC.ai[2] = (deathrayTimer + MathHelper.Pi) * deathrayFaceDirection; // ai[2] is used as a temporary buffer to facilitate incrementing localAI[0]
                }
                else
                {
                    // Reset deathray angular speed
                    calamityGlobalNPC.newAI[1] = 0f;

                    // Reset pupil draw location
                    NPC.localAI[1] -= 0.07f;
                    if (NPC.localAI[1] < 0f)
                    {
                        NPC.localAI[1] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld) // GFB moon boulder bullshit thanks Red
                        {
                            for (int k = 0; k < 30; k++)
                            {
                                if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.MoonBoulder, 70, 10f);
                            }
                        }
                    }

                    eyeAnimationCheck = 3;
                }
            }

            // Moon Leech thing
            else if (NPC.ai[0] == 2f)
            {
                mouthAnimationCheck = 2;
                eyeAnimationCheck = 3;
                Vector2 mouthOffset = Vector2.UnitY * 216f;

                // Spawn the leech tongue(s)
                if (attackTimer == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 leechSpawnPos = NPC.Center + mouthOffset;
                    foreach (Player p in Main.ActivePlayers)
                    {
                        if (!p.dead && Vector2.Distance(p.Center, leechSpawnPos) <= 3000f)
                        {
                            Vector2 targetLeechDist = Utils.SafeNormalize(Main.player[NPC.target].Center - leechSpawnPos, Vector2.Zero);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), leechSpawnPos, targetLeechDist, ProjectileID.MoonLeech, 0, 0f, Main.myPlayer, NPC.whoAmI + 1, p.whoAmI);
                        }
                    }
                }

                // Spawn Moon Leech Clots from players with a leech tongue on them
                if (attackTimer >= 120f && attackTimer <= 240f && attackTimer % 30f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type == ProjectileID.MoonLeech && Main.player[(int)p.ai[1]].FindBuffIndex(BuffID.MoonLeech) != -1)
                        {
                            Vector2 targetCenter = Main.player[NPC.target].Center;
                            int moonLeech = NPC.NewNPC(NPC.GetSource_FromAI(), (int)targetCenter.X, (int)targetCenter.Y, NPCID.MoonLordLeechBlob, 0, NPC.whoAmI + 1, p.whoAmI);
                            Main.npc[moonLeech].netUpdate = true;
                        }
                    }
                }
            }

            // Phantasmal Bolts
            else if (NPC.ai[0] == 3f)
            {
                if (attackTimer == 1f)
                {
                    NPC.TargetClosest(false);
                    NPC.netUpdate = true;
                }

                Vector2 aimDirection = Main.player[NPC.target].Center - NPC.Center;
                bool shootFirstBolt = attackTimer == phaseAttackTime - 14f;
                bool shootSecondBolt = attackTimer == phaseAttackTime - 7f;
                bool shootThirdBolt = attackTimer == phaseAttackTime;
                switch (aggressionLevel)
                {
                    case 6:
                    case 5:
                    case 4:
                        break;
                    case 3:
                    // Lower aggression fires less phantasmal bolts
                    case 2:
                        shootSecondBolt = false;
                        break;
                    case 1:
                        shootSecondBolt = false;
                        shootThirdBolt = false;
                        break;
                    default:
                        break;
                }

                // localAI[0] and localAI[1] are again used to control where the eye's pupil draws
                NPC.localAI[0] = NPC.localAI[0].AngleLerp(aimDirection.ToRotation(), 0.5f);
                NPC.localAI[1] += 0.05f;
                if (NPC.localAI[1] > 1f)
                    NPC.localAI[1] = 1f;

                if (attackTimer == phaseAttackTime - 35f)
                    SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                // Fire the phantasmal bolts
                if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 boltDirection = Utils.Vector2FromElipse(NPC.localAI[0].ToRotationVector2(), eyeSizeVector * NPC.localAI[1]);

                    float velocity = 6.25f;
                    // Higher aggression increases and lower aggression reduces phantasmal bolt speed.
                    velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 1.5f, 0.75f, -0.25f, -0.5f, -0.75f);

                    Vector2 boltVelocity = Vector2.Normalize(aimDirection) * velocity;
                    int type = ProjectileID.PhantasmalBolt;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + boltDirection, boltVelocity, type, BoltDamage, 0f, Main.myPlayer);
                }
            }

            // localAI[2] is used to control the animation of the mouth opening and closing
            int headEyeVulnerableCheck = mouthAnimationCheck * 7;
            if (headEyeVulnerableCheck > NPC.localAI[2])
                NPC.localAI[2] += 1f;
            if (headEyeVulnerableCheck < NPC.localAI[2])
                NPC.localAI[2] -= 1f;
            if (NPC.localAI[2] < 0f)
                NPC.localAI[2] = 0f;
            if (NPC.localAI[2] > 14f)
                NPC.localAI[2] = 14f;

            // localAI[3] is used to control the animation of the head eye opening and closing
            // It also controls whether or not the head can be damaged
            int headEyeDeathrayCheck = eyeAnimationCheck * 5;
            if (headEyeDeathrayCheck > NPC.localAI[3])
                NPC.localAI[3] += 1f;
            if (headEyeDeathrayCheck < NPC.localAI[3])
                NPC.localAI[3] -= 1f;
            if (NPC.localAI[3] < 0f)
                NPC.localAI[2] = 0f;
            if (NPC.localAI[3] > 15f)
                NPC.localAI[2] = 15f;
        }

        public void BuffedMoonLordHandAI(int aggressionLevel)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Start attack array
            NPC.InitializeMoonLordAttacks();

            // Despawn
            if (!Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != NPCID.MoonLordCore)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
            }

            // Variables
            bool isLeftHand = NPC.ai[2] == 0f;
            float handFaceDirection = -isLeftHand.ToDirectionInt();
            NPC.spriteDirection = (int)handFaceDirection;

            // Trigger Daybreak projectiles
            if (NPC.frameCounter == 19.0 && !NPC.dontTakeDamage)
                NPC.PopAllAttachedProjectilesAndTakeDamageForThem();

            NPC.dontTakeDamage = NPC.frameCounter >= 21.0;
            if (calamityGlobalNPC.newAI[0] == 1f)
                NPC.dontTakeDamage = true;

            Vector2 eyeSizeVector = new Vector2(30f, 66f);
            Vector2 coreCenter = Main.npc[(int)NPC.ai[3]].Center;
            float handAttackTimer = 0f;
            float phaseAttackTime = 0f;
            int handFrameCheck = 0;

            // Go to death animation
            if (Main.npc[(int)NPC.ai[3]].ai[0] == 2f)
                NPC.ai[0] = -2f;

            // Choose attacks
            if (NPC.ai[0] != -2f || (NPC.ai[0] == -2f && Main.npc[(int)NPC.ai[3]].ai[0] != 2f))
            {
                // Vanilla sets ai[0] to -2 in checkDead, Calamity makes the hands continue to attack after being killed
                if (NPC.ai[0] == -2f && Main.npc[(int)NPC.ai[3]].ai[0] != 2f)
                {
                    if (calamityGlobalNPC.newAI[0] != 1f)
                        calamityGlobalNPC.newAI[0] = 1f;

                    NPC.life = NPC.lifeMax;
                    NPC.netUpdate = true;
                    NPC.dontTakeDamage = true;
                }

                // Set up attacks
                float ai0CrossCheck = NPC.ai[0];
                NPC.ai[1] += 1f;
                int handType = isLeftHand ? 0 : 1;
                int attackIncrement = 0;
                int totalAttackTimer = 0;

                // Yes, it is completely true: Moon Lord uses a 4D array to organize its attack pattern.
                // I will use this opportunity to explain to the best of my abilities how this 4D array is organized.
                // The first value is unused; it is always 0. There is vanilla code suggesting this was originally going to be set to a random value from 0-2, but alas.
                // The second value determines what body part to get attacks for. 0 is left hand, 1 is right hand, 2 is head.
                // The third value holds what is actually passed to the AI. 0 contains the value for ai[0], and 1 contains the attack duration in frames, used for ai[1] comparison.
                // The fourth value is what number attack it is in the pattern, from 0-4.
                while (attackIncrement < 5)
                {
                    phaseAttackTime = NPC.MoonLordAttacksArray[0, handType, 1, attackIncrement];
                    if (phaseAttackTime + totalAttackTimer > NPC.ai[1])
                        break;

                    totalAttackTimer += (int)phaseAttackTime;
                    attackIncrement++;
                }

                if (attackIncrement == 5)
                {
                    attackIncrement = 0;
                    NPC.ai[1] = 0f;
                    phaseAttackTime = NPC.MoonLordAttacksArray[0, handType, 1, attackIncrement];
                    totalAttackTimer = 0;
                }

                NPC.ai[0] = NPC.MoonLordAttacksArray[0, handType, 0, attackIncrement];
                handAttackTimer = (int)NPC.ai[1] - totalAttackTimer;
                if (NPC.ai[0] != ai0CrossCheck)
                    NPC.netUpdate = true;
            }

            if (NPC.ai[0] == -2f)
            {
                handFrameCheck = 0;
                NPC.dontTakeDamage = true;
                NPC.velocity = Main.npc[(int)NPC.ai[3]].velocity;
            }

            // Move
            else if (NPC.ai[0] == 0f)
            {
                handFrameCheck = 3;
                // When localAI[1] is 0, the pupil draws in the center of the eye
                NPC.localAI[1] -= 0.05f;
                if (NPC.localAI[1] < 0f)
                    NPC.localAI[1] = 0f;

                Vector2 handMovementVector = coreCenter + new Vector2(350f * handFaceDirection, -100f);
                Vector2 handMovementDirection = handMovementVector - NPC.Center;

                if (handMovementDirection.Length() > 20f)
                {
                    handMovementDirection.Normalize();

                    float velocity = 7.5f;
                    velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.4f, -0.8f, -1.2f);

                    handMovementDirection *= velocity;
                    Vector2 currentVelocity = NPC.velocity;
                    if (handMovementDirection != Vector2.Zero)
                        NPC.SimpleFlyMovement(handMovementDirection, 0.3f);
                    NPC.velocity = Vector2.Lerp(currentVelocity, NPC.velocity, 0.5f);
                }
            }

            // Phantasmal Eyes
            else if (NPC.ai[0] == 1f)
            {
                handFrameCheck = 0;
                float divisor = 6f;
                divisor = AdjustForAggressionLevel(divisor, aggressionLevel, -2f, -1f, 2f, 4f, 6f);

                if (handAttackTimer >= 56)
                {
                    NPC.localAI[1] -= 0.07f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;
                }
                else if (handAttackTimer >= 28)
                {
                    // Higher values of localAI[1] make the pupil draw closer to the edge of the eye
                    NPC.localAI[1] += 0.05f;
                    if (NPC.localAI[1] > 0.75f)
                        NPC.localAI[1] = 0.75f;

                    // localAI[0] controls the angle in which the eye looks
                    float handPauseDirection = MathHelper.TwoPi * (handAttackTimer % 28) / 28 - MathHelper.PiOver2;
                    NPC.localAI[0] = new Vector2(MathF.Cos(handPauseDirection) * eyeSizeVector.X, MathF.Sin(handPauseDirection) * eyeSizeVector.Y).ToRotation();

                    // Spawn phantasmal eyes
                    if (handAttackTimer % divisor == 0f)
                    {
                        float velocity = 3f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 0f, 0f, 0.5f, 1f, 1.5f);

                        Vector2 eyeDirection = Utils.Vector2FromElipse(NPC.localAI[0].ToRotationVector2(), eyeSizeVector * NPC.localAI[1]);
                        Vector2 eyeSpawn = NPC.Center + Vector2.Normalize(eyeDirection) * eyeSizeVector.Length() * 0.4f + new Vector2(-handFaceDirection, 3f);
                        Vector2 eyeVelocity = Vector2.Normalize(eyeDirection) * velocity;
                        float ai = (Main.rand.NextFloat(MathHelper.TwoPi) - MathHelper.Pi) / 30f + MathHelper.Pi / 180f * handFaceDirection;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), eyeSpawn, eyeVelocity, ProjectileID.PhantasmalEye, EyeDamage, 0f, Main.myPlayer, 0f, ai, aggressionLevel);
                        Main.projectile[proj].timeLeft = 1200;
                    }
                }
                else
                {
                    NPC.localAI[1] += 0.02f;
                    if (NPC.localAI[1] > 0.75f)
                        NPC.localAI[1] = 0.75f;

                    float handPauseDirection = MathHelper.TwoPi * (handAttackTimer % 28) / 28 - MathHelper.PiOver2;
                    NPC.localAI[0] = new Vector2(MathF.Cos(handPauseDirection) * eyeSizeVector.X, MathF.Sin(handPauseDirection) * eyeSizeVector.Y).ToRotation();
                }
            }

            // Phantasmal Spheres
            else if (NPC.ai[0] == 2f)
            {
                // localAI[1] is again used to control the pupil draw position
                NPC.localAI[1] -= 0.05f;
                if (NPC.localAI[1] < 0f)
                    NPC.localAI[1] = 0f;

                Vector2 sphereHandDirection = coreCenter + new Vector2(320f * handFaceDirection, -110f);
                Vector2 sphereHandDirectionMaxBound = new Vector2(400f * handFaceDirection, -60f);

                float velocityMultiplier = 0.885f;
                velocityMultiplier = AdjustForAggressionLevel(velocityMultiplier, aggressionLevel, -0.04f, -0.02f, 0.004f, 0.008f, 0.012f);

                if (handAttackTimer < 30f)
                {
                    // Set the hand's velocity for moving away from the body
                    Vector2 sphereHandTravelVelocity = sphereHandDirection - NPC.Center;
                    if (sphereHandTravelVelocity != Vector2.Zero)
                    {
                        Vector2 sphereHandTravelDist = Vector2.Normalize(sphereHandTravelVelocity);

                        float velocity = 10f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.5f, -1f, -1.5f);

                        NPC.velocity = Vector2.SmoothStep(NPC.velocity, sphereHandTravelDist * Math.Min(velocity, sphereHandTravelVelocity.Length()), 0.2f);
                    }
                }
                else if (handAttackTimer < 210f)
                {
                    // Set the frame to slightly closed
                    handFrameCheck = 1;
                    int sphereHandSpeed = (int)handAttackTimer - 30;

                    int divisor = 30;
                    divisor = (int)AdjustForAggressionLevel(divisor, aggressionLevel, -10f, -5f, 15f, 30f, 60f);

                    // Spawn the phantasmal spheres
                    if (sphereHandSpeed % divisor == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int finalSphereHandSpeed = sphereHandSpeed / 30;
                        Vector2 sphereFireDirection = new Vector2(5f * handFaceDirection, finalSphereHandSpeed - 12.5f);
                        sphereFireDirection.X += (finalSphereHandSpeed - 3.5f) * handFaceDirection * 3f;
                        sphereFireDirection *= 1.2f;

                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, sphereFireDirection, ProjectileID.PhantasmalSphere, SphereDamage, 1f, Main.myPlayer, 0f, NPC.whoAmI);
                        Main.projectile[proj].timeLeft = 1200;
                    }

                    // Set the hand's velocity for moving away from the body
                    Vector2 handSmoothMovement = Vector2.SmoothStep(sphereHandDirection, sphereHandDirection + sphereHandDirectionMaxBound, (handAttackTimer - 30f) / 180f) - NPC.Center;
                    if (handSmoothMovement != Vector2.Zero)
                    {
                        Vector2 handSmoothMoveNormalize = handSmoothMovement;
                        handSmoothMoveNormalize.Normalize();

                        float velocity = 24f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -1f, -2f, -3f);

                        NPC.velocity = Vector2.Lerp(NPC.velocity, handSmoothMoveNormalize * Math.Min(velocity, handSmoothMovement.Length()), 0.5f);
                    }
                }
                // Slow the hand down at the end of the attack
                else if (handAttackTimer < 282f)
                {
                    handFrameCheck = 0;
                    NPC.velocity *= velocityMultiplier;
                }
                else if (handAttackTimer < 287f)
                {
                    handFrameCheck = 1;
                    NPC.velocity *= velocityMultiplier;
                }
                else if (handAttackTimer < 292f)
                {
                    handFrameCheck = 2;
                    NPC.velocity *= velocityMultiplier;
                }
                else if (handAttackTimer < 300f)
                {
                    handFrameCheck = 3;

                    // Cause phantasmal spheres to start moving
                    if (handAttackTimer == 292f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Determine the direction to make phantasmal spheres move
                        int closestPlayer = Player.FindClosest(NPC.position, NPC.width, NPC.height);
                        Vector2 sphereVelocity = Utils.SafeNormalize(Main.player[closestPlayer].Center - (NPC.Center + Vector2.UnitY * -350f), Vector2.UnitY);

                        float velocity = 2f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.25f, -0.5f, -0.75f);

                        sphereVelocity *= velocity;
                        foreach (Projectile sp in Main.ActiveProjectiles)
                        {
                            if (sp.type == ProjectileID.PhantasmalSphere && sp.ai[1] == NPC.whoAmI && sp.ai[0] != -1f)
                            {
                                sp.ai[0] = -1f;
                                sp.velocity = sphereVelocity;
                                sp.netUpdate = true;
                            }
                        }
                    }

                    Vector2 handPauseSmoothSpeed = Vector2.SmoothStep(sphereHandDirection, sphereHandDirection + sphereHandDirectionMaxBound, 1f - (handAttackTimer - 270f) / 30f) - NPC.Center;
                    if (handPauseSmoothSpeed != Vector2.Zero)
                    {
                        Vector2 handPauseDirection = handPauseSmoothSpeed;
                        handPauseDirection.Normalize();

                        float velocity = 17.5f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -1f, -2f, -3f);

                        NPC.velocity = Vector2.Lerp(NPC.velocity, handPauseDirection * Math.Min(velocity, handPauseSmoothSpeed.Length()), 0.1f);
                    }
                }
                else
                {
                    handFrameCheck = 3;

                    // Make the hand return back to being close to the body
                    Vector2 handReturnSmoothSpeed = sphereHandDirection - NPC.Center;
                    if (handReturnSmoothSpeed != Vector2.Zero)
                    {
                        Vector2 handReturnDirection = handReturnSmoothSpeed;
                        handReturnDirection.Normalize();

                        float velocity = 10f;
                        velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 3f, 1.5f, -0.5f, -1f, -1.5f);

                        NPC.velocity = Vector2.SmoothStep(NPC.velocity, handReturnDirection * Math.Min(velocity, handReturnSmoothSpeed.Length()), 0.2f);
                    }
                }
            }

            // Phantasmal Bolts
            else if (NPC.ai[0] == 3f)
            {
                if (handAttackTimer == 0f)
                {
                    NPC.TargetClosest(false);
                    NPC.netUpdate = true;
                }

                Vector2 aimDirection = Main.player[NPC.target].Center - NPC.Center;
                bool shootFirstBolt = handAttackTimer == phaseAttackTime - 14f;
                bool shootSecondBolt = handAttackTimer == phaseAttackTime - 7f;
                bool shootThirdBolt = handAttackTimer == phaseAttackTime;
                switch (aggressionLevel)
                {
                    case 6:
                    case 5:
                    case 4:
                        break;
                    // Lower aggression fires less phantasmal bolts
                    case 3:
                    case 2:
                        shootSecondBolt = false;
                        break;
                    case 1:
                        shootSecondBolt = false;
                        shootThirdBolt = false;
                        break;
                    default:
                        break;
                }

                // localAI[0] and localAI[1] are again used to control where the eye's pupil draws
                NPC.localAI[0] = NPC.localAI[0].AngleLerp(aimDirection.ToRotation(), 0.5f);
                NPC.localAI[1] += 0.05f;
                if (NPC.localAI[1] > 1f)
                    NPC.localAI[1] = 1f;

                if (handAttackTimer == phaseAttackTime - 35f)
                    SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                // Fire the phantasmal bolts
                if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 boltShootDirection = Utils.Vector2FromElipse(NPC.localAI[0].ToRotationVector2(), eyeSizeVector * NPC.localAI[1]);

                    float velocity = 6.25f;
                    // Higher aggression increases and lower aggression reduces phantasmal bolt speed.
                    velocity = AdjustForAggressionLevel(velocity, aggressionLevel, 1.5f, 0.75f, -0.25f, -0.5f, -0.75f);

                    Vector2 boltShootSpeed = Vector2.Normalize(aimDirection) * velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + boltShootDirection, boltShootSpeed, ProjectileID.PhantasmalBolt, BoltDamage, 0f, Main.myPlayer);
                }
            }

            // Movement of hands when not attacking
            Vector2 handDirection = coreCenter + new Vector2(220f * handFaceDirection, -60f);
            Vector2 minHandFaceDirection = handDirection + new Vector2(handFaceDirection * 110f, -150f);
            Vector2 maxHandFaceDirection = minHandFaceDirection + new Vector2(handFaceDirection * 370f, 150f);

            if (minHandFaceDirection.X > maxHandFaceDirection.X)
                Utils.Swap(ref minHandFaceDirection.X, ref maxHandFaceDirection.X);
            if (minHandFaceDirection.Y > maxHandFaceDirection.Y)
                Utils.Swap(ref minHandFaceDirection.Y, ref maxHandFaceDirection.Y);

            Vector2 defaultHandVelocity = Vector2.Clamp(NPC.Center + NPC.velocity, minHandFaceDirection, maxHandFaceDirection);
            if (defaultHandVelocity != NPC.Center + NPC.velocity)
                NPC.Center = defaultHandVelocity - NPC.velocity;

            // Frame control, also controls when the hands are immune to damage
            int handFrameTimer = handFrameCheck * 7;
            if (handFrameTimer > NPC.frameCounter)
            {
                double handFrameControl = NPC.frameCounter;
                NPC.frameCounter = handFrameControl + 1.0;
            }
            if (handFrameTimer < NPC.frameCounter)
            {
                double handFrameControl = NPC.frameCounter;
                NPC.frameCounter = handFrameControl - 1.0;
            }

            if (NPC.frameCounter < 0.0)
                NPC.frameCounter = 0.0;
            if (NPC.frameCounter > 21.0)
                NPC.frameCounter = 21.0;
        }

        public void BuffedTrueEyeAI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Despawn if the main boss is dying.
            if (Main.npc[(int)NPC.ai[3]].ai[0] == 2f)
            {
                NPC.HitEffect(0, 9999.0);
                NPC.active = false;
            }

            // Sync up the behavior of True Eyes.
            if (calamityGlobalNPC.newAI[0] == 0f)
            {
                int eyeCount = NPC.CountNPCS(NPC.type);
                if (eyeCount > 1)
                {
                    int eyesSynced = 1;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.whoAmI != NPC.whoAmI && n.type == NPC.type)
                        {
                            n.ai[0] = 0f;
                            n.ai[1] = 0f;
                            n.ai[2] = 0f;
                            n.localAI[0] = 0f;
                            n.localAI[1] = 0f;
                            n.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[0] = 1f;
                            calamityGlobalNPC.newAI[1] = 0f;
                            NPC.netUpdate = true;

                            eyesSynced++;
                            if (eyesSynced >= eyeCount)
                                break;
                        }
                    }
                }
                else
                    calamityGlobalNPC.newAI[0] = 1f;
            }

            if (Main.rand.NextBool(420))
                SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Zombie100 : SoundID.Zombie101, NPC.Center);

            // Despawn if the main boss despawned.
            if (!Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != NPCID.MoonLordCore)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
            }

            // Set up attacks
            Vector2 eyeSizeVector = new Vector2(30f);
            float phaseAttackTime = 0f;
            float ai0CrossCheck = NPC.ai[0];
            NPC.ai[1] += 1f;

            // True Eyes of Cthulhu use a separate attack array from Moon Lord which, thankfully, is only a 2D array. It is organized as follows:
            // The first value holds what is actually passed to the AI. 0 contains the value for ai[0], and 1 contains the attack duration in frames, used for ai[1] comparison.
            // The second value is what number attack it is in the pattern, from 0-9.
            int attackIncrement = 0;
            int totalAttackTimer = 0;
            while (attackIncrement < 10)
            {
                phaseAttackTime = NPC.MoonLordAttacksArray2[1, attackIncrement];
                if (phaseAttackTime + totalAttackTimer > NPC.ai[1])
                    break;

                totalAttackTimer += (int)phaseAttackTime;
                attackIncrement += 1;
            }

            if (attackIncrement == 10)
            {
                attackIncrement = 0;
                NPC.ai[1] = 0f;
                phaseAttackTime = NPC.MoonLordAttacksArray2[1, attackIncrement];
                totalAttackTimer = 0;
            }

            NPC.ai[0] = NPC.MoonLordAttacksArray2[0, attackIncrement];
            float secondAttackTimer = (int)NPC.ai[1] - totalAttackTimer;

            if (NPC.ai[0] != ai0CrossCheck)
                NPC.netUpdate = true;

            // Completely unused state as far as I can tell.
            if (NPC.ai[0] == -1f)
            {
                NPC.ai[1] += 1f;
                if (NPC.ai[1] > 180f)
                    NPC.ai[1] = 0f;

                float localAI2Lerp;
                if (NPC.ai[1] < 60f)
                {
                    localAI2Lerp = 0.75f;

                    NPC.localAI[0] = 0f;

                    NPC.localAI[1] = (float)Math.Sin(NPC.ai[1] * MathHelper.TwoPi / 15f) * 0.35f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[0] = MathHelper.Pi;
                }
                else if (NPC.ai[1] < 120f)
                {
                    localAI2Lerp = 1f;

                    if (NPC.localAI[1] < 0.5f)
                        NPC.localAI[1] += 0.025f;

                    NPC.localAI[0] += 0.209439516f;
                }
                else
                {
                    localAI2Lerp = 1.15f;

                    NPC.localAI[1] -= 0.05f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;
                }

                NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], localAI2Lerp, 0.3f);
            }

            // Move towards the target.
            if (NPC.ai[0] == 0f)
            {
                NPC.TargetClosest(false);

                Vector2 v7 = Main.player[NPC.target].Center - NPC.Center;

                // localAI[0] controls the angle in which the eye looks.
                // localAI[1] controls how close to the edge the pupil draws.
                NPC.localAI[0] = NPC.localAI[0].AngleLerp(v7.ToRotation(), 0.5f);
                NPC.localAI[1] += 0.05f;
                if (NPC.localAI[1] > 0.7f)
                    NPC.localAI[1] = 0.7f;
                // localAI[2] controls the size of the pupil, such as when it gets smaller while firing phantasmal bolts.
                NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 1f, 0.2f);

                // Actual movement.
                float velocity = 36f;
                Vector2 freeEyeTargetCenter = Main.player[NPC.target].Center;
                Vector2 freeEyeTargetDistance = Utils.SafeNormalize(freeEyeTargetCenter - NPC.Center, Vector2.Zero) * velocity;

                if (Vector2.Distance(NPC.Center, freeEyeTargetCenter) > 300f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 29 + freeEyeTargetDistance.X) / 30;
                    NPC.velocity.Y = (NPC.velocity.Y * 29 + freeEyeTargetDistance.Y) / 30;
                }
                else
                {
                    NPC.velocity *= 0.8f;
                    if (NPC.velocity.Length() < 1f)
                        NPC.velocity = Vector2.Zero;
                }

                // Push away from other True Eyes.
                float freeEyeAccel = 0.5f;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.whoAmI != NPC.whoAmI && n.type == NPC.type)
                    {
                        if (Vector2.Distance(NPC.Center, n.Center) < 150f)
                        {
                            if (NPC.position.X < n.position.X)
                                NPC.velocity.X -= freeEyeAccel;
                            else
                                NPC.velocity.X += freeEyeAccel;

                            if (NPC.position.Y < n.position.Y)
                                NPC.velocity.Y -= freeEyeAccel;
                            else
                                NPC.velocity.Y += freeEyeAccel;
                        }
                    }
                }
                return;
            }

            // Phantasmal bolts.
            if (NPC.ai[0] == 1f)
            {
                if (secondAttackTimer == 0f)
                {
                    NPC.TargetClosest(false);
                    NPC.netUpdate = true;
                }

                // Slow down before firing.
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 1f)
                    NPC.velocity = Vector2.Zero;

                Vector2 aimDirection = Main.player[NPC.target].Center - NPC.Center;

                NPC.localAI[0] = NPC.localAI[0].AngleLerp(aimDirection.ToRotation(), 0.5f);
                NPC.localAI[1] += 0.05f;
                if (NPC.localAI[1] > 1f)
                    NPC.localAI[1] = 1f;

                if (secondAttackTimer < 20f)
                    NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 1.1f, 0.2f);
                else
                    NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 0.4f, 0.2f);

                if (secondAttackTimer == phaseAttackTime - 35f)
                    SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                // Fire the phantasmal bolts.
                if ((secondAttackTimer == phaseAttackTime - 14f || secondAttackTimer == phaseAttackTime - 7f || secondAttackTimer == phaseAttackTime) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 freeEyeBoltDirection = Utils.Vector2FromElipse(NPC.localAI[0].ToRotationVector2(), eyeSizeVector * NPC.localAI[1]);
                    float velocity = 8f;
                    Vector2 freeEyeBoltVel = Vector2.Normalize(aimDirection) * velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + freeEyeBoltDirection, freeEyeBoltVel, ProjectileID.PhantasmalBolt, TrueEyeBoltDamage, 0f, Main.myPlayer);
                }
            }
            // Phantasmal spheres. This also covers an ai[0] of 4, which is normally the phantasmal deathray in vanilla.
            else if (NPC.ai[0] == 2f || NPC.ai[0] == 4f)
            {
                int type = ProjectileID.PhantasmalSphere;

                // Slow down before attacking.
                if (secondAttackTimer < 15f)
                {
                    NPC.localAI[1] -= 0.07f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;

                    NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 0.4f, 0.2f);

                    NPC.velocity *= 0.8f;
                    if (NPC.velocity.Length() < 1f)
                        NPC.velocity = Vector2.Zero;
                }
                // Start spawning the phantasmal spheres around the eye in a hexagon.
                else if (secondAttackTimer < 75f)
                {
                    float freeEyeAttackPattern = (secondAttackTimer - 15f) / 10f;
                    int freeEyeRotateValue = 0;
                    int freeEyeRotateTransition = 0;
                    switch ((int)freeEyeAttackPattern)
                    {
                        case 0:
                            freeEyeRotateValue = 0;
                            freeEyeRotateTransition = 2;
                            break;
                        case 1:
                            freeEyeRotateValue = 2;
                            freeEyeRotateTransition = 5;
                            break;
                        case 2:
                            freeEyeRotateValue = 5;
                            freeEyeRotateTransition = 3;
                            break;
                        case 3:
                            freeEyeRotateValue = 3;
                            freeEyeRotateTransition = 1;
                            break;
                        case 4:
                            freeEyeRotateValue = 1;
                            freeEyeRotateTransition = 4;
                            break;
                        case 5:
                            freeEyeRotateValue = 4;
                            freeEyeRotateTransition = 0;
                            break;
                    }

                    Vector2 basePoint = -Vector2.UnitY * 30f;
                    Vector2 freeEyeRotateLerp = basePoint.RotatedBy(freeEyeRotateValue * MathHelper.TwoPi / 6f);
                    Vector2 freeEyeTransitionLerp = basePoint.RotatedBy(freeEyeRotateTransition * MathHelper.TwoPi / 6f);
                    // Your first thought may be to see this and think "Why is there a lerp which has a lerp value that is always 0?"
                    // It is not always 0 though, because freeEyeAttackPattern is a float, so it's a decimal number subtracted from a truncated integer.
                    // This is necessary for the pupil location to smoothly transition between each point.
                    Vector2 freeEyeRotation = Vector2.Lerp(freeEyeRotateLerp, freeEyeTransitionLerp, freeEyeAttackPattern - (int)freeEyeAttackPattern);
                    float freeEyeRotationDist = freeEyeRotation.Length() / 30f;
                    // localAI[0] and localAI[1] are again used to control where the eye's pupil draws.
                    NPC.localAI[0] = freeEyeRotation.ToRotation();
                    NPC.localAI[1] = MathHelper.Lerp(NPC.localAI[1], freeEyeRotationDist, 0.5f);

                    // Dust telegraph
                    for (int k = 0; k < 2; k++)
                    {
                        Dust trueEyeDust = Dust.NewDustDirect(NPC.Center + freeEyeRotation - Vector2.One * 4f, 0, 0, DustID.Vortex, 0f, 0f);
                        trueEyeDust.velocity += freeEyeRotation / 15f;
                        trueEyeDust.noGravity = true;
                    }

                    // Spawn the phantasmal spheres in their initial positions around the True Eye.
                    if ((secondAttackTimer - 15f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 trueEyeSphereDirection = Utils.SafeNormalize(freeEyeRotation, -Vector2.UnitY) * 4f;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + freeEyeRotation, trueEyeSphereDirection, type, 0, 0f, Main.myPlayer, 30f, NPC.whoAmI);
                        Main.projectile[proj].timeLeft = 1200;

                        if (Main.zenithWorld)
                        {
                            for (int k = 0; k < 3; k++) // GFB moon boulder bullshit thanks Red
                            {
                                if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 0f), ProjectileID.MoonBoulder, MoonBoulderDamage, 10f);
                            }
                        }
                    }
                }
                // Launching of the phantasmal spheres.
                else
                {
                    // Brief upwards motion before the launch.
                    if (secondAttackTimer < 105f)
                    {
                        NPC.localAI[0] = NPC.localAI[0].AngleLerp(NPC.ai[2] - MathHelper.PiOver2, 0.2f);
                        NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 0.75f, 0.2f);

                        if (secondAttackTimer == 75f)
                        {
                            NPC.TargetClosest(false);
                            NPC.netUpdate = true;
                            NPC.velocity = -Vector2.UnitY * 7f;

                            foreach (Projectile sp in Main.ActiveProjectiles)
                            {
                                if (sp.type == type && sp.ai[1] == NPC.whoAmI && sp.ai[0] != -1f)
                                {
                                    sp.velocity += NPC.velocity;
                                    sp.netUpdate = true;
                                }
                            }
                        }

                        // Slow down, rotate towards the target.
                        NPC.velocity.Y = NPC.velocity.Y * 0.96f;
                        NPC.ai[2] = (Main.player[NPC.target].Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
                        NPC.rotation = NPC.rotation.AngleTowards(NPC.ai[2], MathHelper.Pi / 30f);

                        return;
                    }

                    // Actually launch the spheres.
                    if (secondAttackTimer < 120f)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie102, NPC.Center);

                        if (secondAttackTimer == 105f)
                            NPC.netUpdate = true;

                        float velocity = 12f;
                        Vector2 trueEyeSphereVelocity = (NPC.ai[2] - MathHelper.PiOver2).ToRotationVector2() * velocity;
                        NPC.velocity = trueEyeSphereVelocity * 2f;

                        // Set damage and velocity.
                        foreach (Projectile sp in Main.ActiveProjectiles)
                        {
                            if (sp.type == type && sp.ai[1] == NPC.whoAmI && sp.ai[0] != -1f)
                            {
                                sp.ai[0] = -1f;
                                sp.damage = TrueEyeSphereDamage;
                                sp.velocity = trueEyeSphereVelocity;
                                sp.netUpdate = true;
                            }
                        }

                        return;
                    }

                    NPC.velocity *= 0.92f;
                    NPC.rotation = NPC.rotation.AngleLerp(0f, 0.2f);
                }
            }
            // Phantasmal eyes.
            else if (NPC.ai[0] == 3f)
            {
                // Slow down, set pupil drawing variables.
                if (secondAttackTimer < 15f)
                {
                    NPC.localAI[1] -= 0.07f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;

                    NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 0.4f, 0.2f);

                    NPC.velocity *= 0.9f;
                    if (NPC.velocity.Length() < 1f)
                        NPC.velocity = Vector2.Zero;
                }
                else if (secondAttackTimer < 45f)
                {
                    NPC.localAI[0] = 0f;

                    NPC.localAI[1] = (float)Math.Sin((secondAttackTimer - 15f) * MathHelper.TwoPi / 15f) * 0.5f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[0] = MathHelper.Pi;
                }
                else
                {
                    // Reset velocity and rotation.
                    if (secondAttackTimer >= 185f)
                    {
                        NPC.velocity *= 0.88f;
                        NPC.rotation = NPC.rotation.AngleLerp(0f, 0.2f);

                        NPC.localAI[1] -= 0.07f;
                        if (NPC.localAI[1] < 0f)
                            NPC.localAI[1] = 0f;

                        NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 1f, 0.2f);
                        return;
                    }

                    // Set the direction rotate in.
                    if (secondAttackTimer == 45f)
                    {
                        NPC.ai[2] = Main.rand.NextBool().ToDirectionInt() * MathHelper.TwoPi / 40f;
                        NPC.netUpdate = true;
                    }

                    if ((secondAttackTimer - 15f - 30f) % 40f == 0f)
                        NPC.ai[2] *= 0.95f;

                    // localAI[0] and localAI[1] are again used to control where the eye's pupil draws.
                    NPC.localAI[0] += NPC.ai[2];
                    NPC.localAI[1] += 0.05f;
                    if (NPC.localAI[1] > 1f)
                        NPC.localAI[1] = 1f;

                    // Spin around in a circle.
                    Vector2 trueEyeDirection = NPC.localAI[0].ToRotationVector2() * eyeSizeVector * NPC.localAI[1];
                    float trueEyeVelScale = MathHelper.Lerp(8f, 20f, (secondAttackTimer - 15f - 30f) / 140f);
                    NPC.velocity = Vector2.Normalize(trueEyeDirection) * trueEyeVelScale;
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);

                    // Spawn the phantasmal eyes.
                    if ((secondAttackTimer - 45f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 trueEyeEyeDirection = NPC.Center + Vector2.Normalize(trueEyeDirection) * eyeSizeVector.Length() * 0.4f;
                        Vector2 trueEyeEyeSpeed = Vector2.Normalize(trueEyeDirection) * 5f;
                        float ai1 = (Main.rand.NextFloat(MathHelper.TwoPi) - MathHelper.Pi) / 30f + MathHelper.Pi / 180f * NPC.ai[2];
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), trueEyeEyeDirection, trueEyeEyeSpeed, ProjectileID.PhantasmalEye, TrueEyeEyeDamage, 0f, Main.myPlayer, 0f, ai1);
                        Main.projectile[proj].timeLeft = 1200;
                    }
                }
            }
            // Phantasmal deathray. This code is never reached in Rev+ as the phantasmal deathray is replaced with another phantasmal sphere attack.
            else if (NPC.ai[0] == 4f)
            {
                if (secondAttackTimer == 0f)
                {
                    NPC.TargetClosest(false);
                    NPC.netUpdate = true;
                }

                // Slow down, set pupil draw variables.
                if (secondAttackTimer < 180f)
                {
                    NPC.localAI[2] = MathHelper.Lerp(NPC.localAI[2], 1f, 0.2f);

                    NPC.localAI[1] -= 0.05f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;

                    NPC.velocity *= 0.95f;
                    if (NPC.velocity.Length() < 1f)
                        NPC.velocity = Vector2.Zero;

                    // Dust telegraph
                    if (secondAttackTimer >= 60f)
                    {
                        int dustAmt = secondAttackTimer >= 120f ? 2 : 1;
                        for (int j = 0; j < dustAmt; j++)
                        {
                            float dustScale = j % 2 == 1 ? 1.65f : 0.8f;

                            Vector2 trueEyeDustDirection = NPC.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * eyeSizeVector / 2f;
                            Dust trueEyeDust = Dust.NewDustDirect(trueEyeDustDirection - Vector2.One * 8f, 16, 16, DustID.Vortex, 0f, 0f, Scale: dustScale);
                            trueEyeDust.velocity = Vector2.Normalize(NPC.Center - trueEyeDustDirection) * 0.35f * (10f - dustAmt * 2f);
                            trueEyeDust.noGravity = true;
                            trueEyeDust.customData = NPC;
                        }
                    }
                }
                else
                {
                    if (secondAttackTimer < phaseAttackTime - 15f)
                    {
                        if (calamityGlobalNPC.newAI[1] == 0f)
                            calamityGlobalNPC.newAI[1] = 600f;

                        // Fire the phantasmal deathray.
                        if (secondAttackTimer == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Deathray sweeps slower if the head is doing the deathray attack.
                            if (Main.npc[(int)Main.npc[(int)NPC.ai[3]].localAI[2]].ai[0] == 1f)
                                calamityGlobalNPC.newAI[1] *= 1.5f;

                            NPC.TargetClosest(false);

                            Vector2 deathrayTargetDist = Utils.DirectionTo(NPC.Center, Main.player[NPC.target].Center);
                            int deathraySweepDirection = (deathrayTargetDist.X < 0f).ToDirectionInt();

                            deathrayTargetDist = deathrayTargetDist.RotatedBy(-(double)deathraySweepDirection * MathHelper.TwoPi / 6f);
                            int type = ProjectileID.PhantasmalDeathray;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, deathrayTargetDist, type, TrueEyeDeathrayDamage, 0f, Main.myPlayer, deathraySweepDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1], NPC.whoAmI);
                            NPC.ai[2] = (deathrayTargetDist.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * deathraySweepDirection;
                            NPC.netUpdate = true;
                        }

                        NPC.localAI[1] += 0.05f;
                        if (NPC.localAI[1] > 1f)
                            NPC.localAI[1] = 1f;

                        float deathrayRotationDirection = (NPC.ai[2] >= 0f).ToDirectionInt();
                        float deathrayRotation = NPC.ai[2];
                        if (deathrayRotation < 0f)
                            deathrayRotation *= -1f;

                        deathrayRotation += deathrayRotationDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1] - MathHelper.Pi;
                        NPC.localAI[0] = deathrayRotation;
                        NPC.ai[2] = (deathrayRotation + MathHelper.Pi) * deathrayRotationDirection;

                        return;
                    }

                    calamityGlobalNPC.newAI[1] = 0f;

                    NPC.localAI[1] -= 0.07f;
                    if (NPC.localAI[1] < 0f)
                        NPC.localAI[1] = 0f;
                }
            }
        }

        public void BuffedMoonLeechBlobAI()
        {
            // Variables
            Vector2 mouthMovement = Vector2.UnitY * 216f;
            int headIndex = (int)Math.Abs(NPC.ai[0]) - 1;
            int leechTongue = (int)NPC.ai[1];

            // Despawn if the head despawned.
            if (!Main.npc[headIndex].active || Main.npc[headIndex].type != NPCID.MoonLordHead)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return;
            }

            // Heal a Moon Lord part after reaching the mouth.
            NPC.ai[2] += 1f;
            if (NPC.ai[2] >= 180f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int coreIndex = (int)Main.npc[headIndex].ai[3];
                    int leftHandHeal = -1;
                    int rightHandHeal = -1;
                    int headHeal = headIndex;

                    // Find the indices of the left and right hands.
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.ai[3] == coreIndex)
                        {
                            if (leftHandHeal == -1 && n.type == NPCID.MoonLordHand && n.ai[2] == 0f)
                                leftHandHeal = n.whoAmI;
                            if (rightHandHeal == -1 && n.type == NPCID.MoonLordHand && n.ai[2] == 1f)
                                rightHandHeal = n.whoAmI;
                            if (leftHandHeal != -1 && rightHandHeal != -1)
                                break;
                        }
                    }

                    // Find heal limits for each part. Death Mode can heal more health.
                    int maxHealAmt = CalamityWorld.death ? 1500 : 1250;
                    int coreMissingHP = Main.npc[coreIndex].lifeMax - Main.npc[coreIndex].life;
                    int leftHandMissingHP = Main.npc[leftHandHeal].lifeMax - Main.npc[leftHandHeal].life;
                    int rightHandMissingHP = Main.npc[rightHandHeal].lifeMax - Main.npc[rightHandHeal].life;
                    int headMissingHP = Main.npc[headHeal].lifeMax - Main.npc[headHeal].life;

                    // Head healing.
                    if (headMissingHP > 0 && maxHealAmt > 0)
                    {
                        // Each failsafe threshold ensures it will either heal the Leech's maximum heal or up to the NPC's max health.
                        int headHealthFailsafe = headMissingHP - maxHealAmt;
                        if (headHealthFailsafe > 0)
                            headHealthFailsafe = 0;

                        int headHealingAmt = maxHealAmt + headHealthFailsafe;
                        // maxHealAmt gets subtracted from, if this is still greater than 0 then it can heal additional parts.
                        maxHealAmt -= headHealingAmt;
                        Main.npc[headHeal].life += headHealingAmt;
                        NPC.HealEffect(Utils.CenteredRectangle(Main.npc[headHeal].Center, new Vector2(50f)), headHealingAmt);
                    }
                    if (coreMissingHP > 0 && maxHealAmt > 0)
                    {
                        int coreHealthFailsafe = coreMissingHP - maxHealAmt;
                        if (coreHealthFailsafe > 0)
                            coreHealthFailsafe = 0;

                        int coreHealingAmt = maxHealAmt + coreHealthFailsafe;
                        maxHealAmt -= coreHealingAmt;
                        Main.npc[coreIndex].life += coreHealingAmt;
                        NPC.HealEffect(Utils.CenteredRectangle(Main.npc[coreIndex].Center, new Vector2(50f)), coreHealingAmt);
                    }
                    if (leftHandMissingHP > 0 && maxHealAmt > 0)
                    {
                        int leftHandHealthFailsafe = leftHandMissingHP - maxHealAmt;
                        if (leftHandHealthFailsafe > 0)
                            leftHandHealthFailsafe = 0;

                        int leftHandHealingAmt = maxHealAmt + leftHandHealthFailsafe;
                        maxHealAmt -= leftHandHealingAmt;
                        Main.npc[leftHandHeal].life += leftHandHealingAmt;
                        NPC.HealEffect(Utils.CenteredRectangle(Main.npc[leftHandHeal].Center, new Vector2(50f)), leftHandHealingAmt);
                    }
                    if (rightHandMissingHP > 0 && maxHealAmt > 0)
                    {
                        int rightHandHealthFailsafe = rightHandMissingHP - maxHealAmt;
                        if (rightHandHealthFailsafe > 0)
                            rightHandHealthFailsafe = 0;

                        int rightHandHealingAmt = maxHealAmt + rightHandHealthFailsafe;
                        Main.npc[rightHandHeal].life += rightHandHealingAmt;
                        NPC.HealEffect(Utils.CenteredRectangle(Main.npc[rightHandHeal].Center, new Vector2(50f)), rightHandHealingAmt);
                    }
                }

                // Die after healing.
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return;
            }

            // Move towards Moon Lord's mouth.
            NPC.velocity = Vector2.Zero;
            NPC.Center = Vector2.Lerp(Main.projectile[leechTongue].Center, Main.npc[(int)Math.Abs(NPC.ai[0]) - 1].Center + mouthMovement, NPC.ai[2] / 180f);

            // Dust effects
            Vector2 basePos = -Vector2.UnitY * NPC.height / 2f;
            for (int i = 0; i < 4; i++)
            {
                Dust leechDust = Dust.NewDustPerfect(NPC.Center - Vector2.One * 4f + basePos.RotatedBy(i * MathHelper.TwoPi / 6f), DustID.Vortex, -Vector2.UnitY, Scale: 0.7f);
                leechDust.noGravity = true;
                leechDust.customData = NPC;
            }

            basePos = -Vector2.UnitY * NPC.height / 6f;
            for (int j = 0; j < 2; j++)
            {
                int leechDust2 = Dust.NewDust(NPC.Center - Vector2.One * 4f + basePos.RotatedBy(j * MathHelper.TwoPi / 6f), 0, 0, DustID.Vortex, 0f, -2f, Scale: 1.5f);
                Main.dust[leechDust2].noGravity = true;
                Main.dust[leechDust2].customData = NPC;
            }
        }
    }
}
