using System;
using CalamityMod.Events;
using CalamityMod.Graphics;
using CalamityMod.Systems.Graphic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class DeerclopsAI : VanillaAIOverride
    {
        public static bool hasTargetBeenInRange = true;
        public const float IncreaseDRTriggerDistance = 750f;
        public const float MaxDRIncreaseDistance = 1200f;
        public static float borderScale = 5f;
        public static string ArenaTexPath = "CalamityMod/Particles/LargeBloom";

        // Vanilla values
        public static int DebrisDamage = 18; // 72

        // Rev+ exclusive
        public static int IceSpikeDamage = 16; // 64 (buffed from 52)
        public static int HandDamage = 13; // 52 (buffed from 40)

        public static Asset<Texture2D> ArenaTex;

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            NPC.deerclopsBoss = NPC.whoAmI;

            // Percent life remaining
            float lifeRatio = (float)NPC.life / (float)NPC.lifeMax;

            // Difficulty bools
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Projectile types and damage
            int rubble = ProjectileID.DeerclopsRangedProjectile;
            int iceSpike = ProjectileID.DeerclopsIceSpike;
            int shadowHand = ProjectileID.InsanityShadowHostile;

            if (NPC.target.WithinBounds(Main.player.Length) && Main.player[NPC.target].dead)
            {
                hasTargetBeenInRange = false;
            }

            // Target data
            NPCAimedTarget targetData = NPC.GetTargetData();

            // Movement variables
            bool haltMovement = false;
            bool goHome = false;

            // Damage resistance based on distance from target
            float distanceFromTarget = NPC.Distance(targetData.Center);
            bool triggerDRIncrease = distanceFromTarget >= IncreaseDRTriggerDistance;
            float resistDamageAmount = MathHelper.Clamp((distanceFromTarget - IncreaseDRTriggerDistance) / (MaxDRIncreaseDistance - IncreaseDRTriggerDistance), 0f, 1f);
            NPC.localAI[3] = MathHelper.Lerp(0f, 30f, resistDamageAmount);
            float dustAndDRScalar = Utils.Remap(NPC.localAI[3], 0f, 30f, 0f, 1f);
            calamityGlobalNPC.DR = MathHelper.Lerp(0f, 0.9f, dustAndDRScalar);


            if (dustAndDRScalar > 0f)
            {
                float invincibleDustAmount = Main.rand.NextFloat() * dustAndDRScalar * 3f;
                while (invincibleDustAmount > 0f)
                {
                    invincibleDustAmount -= 1f;
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -3f, 0, default(Color), 1.4f).noGravity = true;
                }
            }
            else if (!hasTargetBeenInRange && NPC.target.WithinBounds(Main.player.Length) && !Main.player[NPC.target].dead)
            {
                // Target entered the border for the first time
                hasTargetBeenInRange = true;
            }

            // Set the last deerclops position (used only for post-death border shenanigans)
            Main.LocalPlayer.Calamity().lastDeerclopsPosition = NPC.Center;

            // Spawn settings
            if (NPC.homeTileX == -1 && NPC.homeTileY == -1)
            {
                Point point = NPC.Bottom.ToTileCoordinates();
                NPC.homeTileX = point.X;
                NPC.homeTileY = point.Y;
                NPC.ai[2] = NPC.homeTileX;
                NPC.ai[3] = NPC.homeTileY;
                NPC.netUpdate = true;
                NPC.timeLeft = 86400;
            }

            // Decrease time left based on actual world updates
            NPC.timeLeft -= Main.worldEventUpdates;
            if (NPC.timeLeft < 0)
                NPC.timeLeft = 0;

            // Set home tile so Deerclops knows where to return to
            NPC.homeTileX = (int)NPC.ai[2];
            NPC.homeTileY = (int)NPC.ai[3];

            // Spawn Shadow Hands if the player enters the shadows
            if (Main.netMode != NetmodeID.MultiplayerClient && hasTargetBeenInRange)
                SpawnBorderShadowHands(NPC, lifeRatio, shadowHand, HandDamage, death);

            // AI states
            switch ((int)NPC.ai[0])
            {
                // This case is never used
                case -1:

                    NPC.localAI[3] = -10f;

                    break;

                // Choose an attack to use
                case 0:

                    NPC.TargetClosest();
                    targetData = NPC.GetTargetData();
                    if (ShouldRunAway(NPC, ref targetData, isChasing: true))
                    {
                        NPC.ai[0] = 6f;
                        NPC.ai[1] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.netUpdate = true;
                        break;
                    }
                    else
                    {
                        if (NPC.timeLeft < 86400)
                            NPC.timeLeft = 86400;
                    }

                    float attackRate = 1f;
                    NPC.ai[1] += attackRate;
                    Vector2 relativeCenter = NPC.Bottom + new Vector2(0f, -32f);
                    Vector2 closestTargetPoint = targetData.Hitbox.ClosestPointInRect(relativeCenter);
                    Vector2 distanceFromTarget2 = closestTargetPoint - relativeCenter;
                    (closestTargetPoint - NPC.Center).Length();
                    float distanceCheckMultiplier = 0.6f;

                    bool useFrontIceSpikeAttack = Math.Abs(distanceFromTarget2.X) >= Math.Abs(distanceFromTarget2.Y) * distanceCheckMultiplier || distanceFromTarget2.Length() < 48f;
                    bool useEitherIceSpikeAttack = distanceFromTarget2.Y <= (float)(100 + targetData.Height) && distanceFromTarget2.Y >= -200f;

                    // Can only use ice spikes a maximum of three times in a row before doing something else
                    float iceSpikeAttackLimit = 3f;
                    bool doNotUseIceSpikes = calamityGlobalNPC.newAI[1] >= iceSpikeAttackLimit;
                    if (!doNotUseIceSpikes)
                    {
                        // Deerclops must be this close to its target on the X axis to do the ice spike attack
                        // This distance increases at lower HP because the ice spikes get bigger
                        float iceSpikesDistanceGateValue = 120f + MathHelper.Lerp(0f, 60f, 1f - lifeRatio);
                        if (Math.Abs(distanceFromTarget2.X) < iceSpikesDistanceGateValue && useEitherIceSpikeAttack && NPC.velocity.Y == 0f && NPC.localAI[1] >= 2f)
                        {
                            NPC.velocity.X = 0f;
                            NPC.ai[0] = 4f;
                            NPC.ai[1] = 0f;
                            NPC.localAI[1] = 0f;
                            calamityGlobalNPC.newAI[0] -= 1f;
                            calamityGlobalNPC.newAI[1] += 1f;
                            NPC.SyncExtraAI();
                            NPC.netUpdate = true;
                            break;
                        }

                        if (Math.Abs(distanceFromTarget2.X) < iceSpikesDistanceGateValue && useEitherIceSpikeAttack && NPC.velocity.Y == 0f && useFrontIceSpikeAttack)
                        {
                            NPC.velocity.X = 0f;
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.localAI[1] += 1f;
                            calamityGlobalNPC.newAI[0] -= 1f;
                            calamityGlobalNPC.newAI[1] += 1f;
                            NPC.SyncExtraAI();
                            NPC.netUpdate = true;
                            break;
                        }
                    }

                    // Can only use rubble and ice spikes a maximum of four times in a row before doing something else
                    float rubbleAttackLimit = 4f;
                    bool doNotUseRubble = calamityGlobalNPC.newAI[1] >= rubbleAttackLimit;
                    float rubbleGateValue = death ? 160f : 200f;
                    if (!doNotUseRubble)
                    {
                        bool useRubbleAttack = NPC.ai[1] >= rubbleGateValue;
                        if (NPC.velocity.Y == 0f && NPC.velocity.X != 0f && useRubbleAttack)
                        {
                            NPC.velocity.X = 0f;
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 0f;
                            NPC.localAI[1] = 0f;
                            calamityGlobalNPC.newAI[0] -= 1f;
                            calamityGlobalNPC.newAI[1] += 1f;
                            NPC.SyncExtraAI();
                            NPC.netUpdate = true;
                            break;
                        }
                    }

                    float shadowHandGateValue = death ? 60f : 75f;
                    bool useShadowHandAttack = NPC.ai[1] >= shadowHandGateValue;
                    if (NPC.velocity.Y == 0f && NPC.velocity.X == 0f && useShadowHandAttack)
                    {
                        NPC.velocity.X = 0f;
                        NPC.ai[0] = targetData.Center.Y < NPC.Center.Y - 50f ? 5f : 3f;
                        NPC.ai[1] = 0f;
                        NPC.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] -= 1f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        NPC.SyncExtraAI();
                        NPC.netUpdate = true;
                        break;
                    }

                    // This replaced the slow debuff infliction
                    // Must use 4 different attacks before being able to use this attack again
                    float secondShadowHandAttackCooldown = 4f;
                    float secondShadowHandGateValue = death ? 80f : 100f;
                    bool useSecondShadowHandAttack = NPC.ai[1] >= secondShadowHandGateValue;
                    if (NPC.velocity.Y == 0f && useSecondShadowHandAttack && Math.Abs(distanceFromTarget2.X) > 100f && calamityGlobalNPC.newAI[0] <= 0f)
                    {
                        NPC.velocity.X = 0f;
                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] = secondShadowHandAttackCooldown;
                        calamityGlobalNPC.newAI[1] = 0f;
                        NPC.SyncExtraAI();
                        NPC.netUpdate = true;
                    }

                    // If Deerclops has been taking a while to attack, stop moving on the X axis to force a shadow hand attack
                    float haltMovementGateValue = doNotUseRubble ? (secondShadowHandGateValue + 20f) : doNotUseIceSpikes ? (rubbleGateValue + 20f) : 240f;
                    if (distanceFromTarget2.Length() < IncreaseDRTriggerDistance)
                        haltMovement = NPC.ai[1] >= haltMovementGateValue;

                    break;

                // Create spikes in front of Deerclops
                case 1:

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    MakeIceSpikes(NPC, 1, iceSpike, IceSpikeDamage, lifeRatio, death, 36, 20, 4, bothSides: false);

                    float iceSpikePhaseGateValue = 80f;
                    if (NPC.ai[1] >= iceSpikePhaseGateValue)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Scoop up rubble
                case 2:

                    int scoopRubbleGateValue = 32;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] == (float)(scoopRubbleGateValue - 20))
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, NPC.Center);

                    if (NPC.ai[1] == (float)scoopRubbleGateValue)
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);

                    haltMovement = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] >= (float)scoopRubbleGateValue)
                    {
                        Point sourceTileCoords = NPC.Top.ToTileCoordinates();
                        int numRubble = death ? 60 : 20;
                        int distancedByThisManyTiles = death ? 3 : 5;
                        sourceTileCoords.X += NPC.direction * 3;
                        sourceTileCoords.Y -= 10;
                        int screenShakeGateValue = (int)NPC.ai[1] - scoopRubbleGateValue;
                        if (screenShakeGateValue == 0)
                        {
                            PunchCameraModifier modifier4 = new PunchCameraModifier(NPC.Center, new Vector2(0f, -1f), 20f, 6f, 30, 1000f, "Deerclops");
                            Main.instance.CameraModifiers.Add(modifier4);
                        }

                        int rubbleStart = screenShakeGateValue;
                        int rubbleLimit = rubbleStart + 1;
                        if (screenShakeGateValue % 1 != 0)
                            rubbleLimit = rubbleStart;

                        for (int rubbleIndex = rubbleStart; rubbleIndex < rubbleLimit && rubbleIndex < numRubble; rubbleIndex++)
                            ShootRubbleUp(NPC, ref sourceTileCoords, numRubble, distancedByThisManyTiles, rubbleIndex, rubble, DebrisDamage, lifeRatio, death);
                    }

                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn shadow hands around the target with differing velocities
                case 3:

                    if (NPC.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, NPC.Center);

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)NPC.ai[1] % 4 == 0 && NPC.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier5 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier5);
                    }

                    if (NPC.ai[1] == 30f)
                    {
                        NPC.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = (death ? 11 : 9) + (int)MathHelper.Lerp(0f, 7f, 1f - lifeRatio);
                            float velocityMultIncrement = ((totalProjectiles + 1) / (float)totalProjectiles) - 1f;
                            float randomRadialOffset = MathHelper.ToRadians(MathHelper.Lerp(0f, death ? 270f : 180f, 1f - lifeRatio));
                            float radians = MathHelper.TwoPi / totalProjectiles + randomRadialOffset;
                            float velocity = (death ? 9f : 7f) + MathHelper.Lerp(0f, 3.5f, 1f - lifeRatio);
                            SpawnShadowHandRing(NPC, shadowHand, HandDamage, totalProjectiles, velocityMultIncrement, radians, velocity, halveVelocityMultiplier: false);
                        }
                    }

                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn ice spikes on both sides
                case 4:

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    NPC.TargetClosest();
                    MakeIceSpikes(NPC, 1, iceSpike, IceSpikeDamage, lifeRatio, death, 56, 15, 2, bothSides: true);

                    float doubleIceSpikePhaseGateValue = 90f;
                    if (NPC.ai[1] >= doubleIceSpikePhaseGateValue)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn shadow hands around the target (literally just a different variant of the other shadow hand attack with more spread and more velocity)
                case 5:

                    if (NPC.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, NPC.Center);

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)NPC.ai[1] % 4 == 0 && NPC.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier);
                    }

                    if (NPC.ai[1] == 30f)
                    {
                        NPC.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = (death ? 20 : 16) + (int)MathHelper.Lerp(0f, 7f, 1f - lifeRatio);
                            float velocityMultIncrement = ((totalProjectiles + 1) / (float)totalProjectiles) - 1f;
                            float randomRadialOffset = Main.rand.NextFloat(MathHelper.ToRadians(MathHelper.Lerp(0f, death ? 360f : 270f, 1f - lifeRatio)));
                            float radians = MathHelper.TwoPi / totalProjectiles + randomRadialOffset;
                            float velocity = 12f + MathHelper.Lerp(0f, 4f, 1f - lifeRatio);
                            SpawnShadowHandRing(NPC, shadowHand, HandDamage, totalProjectiles, velocityMultIncrement, radians, velocity, halveVelocityMultiplier: true);
                        }
                    }

                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Try to go home
                case 6:

                    NPC.TargetClosest(faceTarget: false);
                    targetData = NPC.GetTargetData();

                    if (NPC.timeLeft > 300)
                        NPC.timeLeft = 300;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!ShouldRunAway(NPC, ref targetData, isChasing: false))
                        {
                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.localAI[1] = 0f;
                            NPC.netUpdate = true;
                            break;
                        }

                        if (NPC.timeLeft <= 0)
                        {
                            NPC.ai[0] = 8f;
                            NPC.ai[1] = 0f;
                            NPC.localAI[1] = 0f;
                            NPC.netUpdate = true;
                            break;
                        }
                    }

                    if (NPC.direction != NPC.oldDirection)
                        NPC.netUpdate = true;

                    goHome = true;
                    NPC.ai[1] += 1f;
                    Vector2 homeVector = new Vector2(NPC.homeTileX * 16, NPC.homeTileY * 16);
                    bool farBelowHome = NPC.Top.Y > homeVector.Y + 1600f;
                    bool closeToHome = NPC.Distance(homeVector) < 1020f;
                    NPC.Distance(targetData.Center);
                    float stopMovingGateValue = NPC.ai[1] % 600f;
                    if (closeToHome && stopMovingGateValue < 420f)
                        haltMovement = true;

                    bool returnHome = false;
                    int returnHomeDueToBelowHomeGateValue = 300;
                    if (farBelowHome && NPC.ai[1] >= (float)returnHomeDueToBelowHomeGateValue)
                        returnHome = true;

                    int returnHomeDueToFarFromHomeGateValue = 1500;
                    if (!closeToHome && NPC.ai[1] >= (float)returnHomeDueToFarFromHomeGateValue)
                        returnHome = true;

                    if (returnHome)
                    {
                        NPC.ai[0] = 7f;
                        NPC.ai[1] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Return home
                case 7:

                    if (NPC.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, NPC.Center);

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)NPC.ai[1] % 4 == 0 && NPC.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier3 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier3);
                    }

                    if (NPC.ai[1] == 40f)
                    {
                        NPC.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.netUpdate = true;
                            NPC.Bottom = new Vector2(NPC.homeTileX * 16, NPC.homeTileY * 16);
                        }
                    }

                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Despawn
                case 8:

                    if (NPC.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, NPC.Center);

                    NPC.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)NPC.ai[1] % 4 == 0 && NPC.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier2 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier2);
                    }

                    if (NPC.ai[1] >= 40f)
                    {
                        NPC.life = -1;
                        NPC.HitEffect();
                        NPC.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);

                        return false;
                    }

                    break;
            }

            // Movement
            Movement(NPC, lifeRatio, haltMovement, goHome, death);

            return false;
        }

        private static bool ShouldRunAway(NPC npc, ref NPCAimedTarget targetData, bool isChasing)
        {
            // Run away if the target is far enough away from Deerclops' spawn point and not in the snow biome, or if the target is dead, or if the target is 2400 or more units away
            if (targetData.Type == NPCTargetType.Player)
            {
                Player player = Main.player[npc.target];
                bool zoneSnow = player.ZoneSnow;
                Vector2 other = new Vector2(npc.homeTileX * 16, npc.homeTileY * 16);
                float distanceToTriggerRunAway = 480f;
                zoneSnow |= player.Distance(other) <= distanceToTriggerRunAway;
                return (player.dead || (!isChasing && !zoneSnow)) | (npc.Distance(player.Center) >= 2400f);
            }

            if (targetData.Type == NPCTargetType.None)
                return true;

            return false;
        }

        private static void SpawnBorderShadowHands(NPC npc, float lifeRatio, int shadowHand, int HandDamage, bool death)
        {
            int shadowHandSpawnRate = death ? 15 : 20;
            npc.localAI[2] += 1f;
            int shadowHandTimer = (int)npc.localAI[2];
            if (shadowHandTimer % shadowHandSpawnRate != 0)
                return;

            int rotation = shadowHandTimer / shadowHandSpawnRate;
            if (shadowHandTimer / shadowHandSpawnRate >= 3)
                npc.localAI[2] = 0f;

            foreach (Player player in Main.ActivePlayers)
            {
                // Spawn hands to cut the player off and force them back towards Deerclops
                // This only happens if the player is triggering Deerclops' increased DR
                float minShadowHandSpawnDistanceFromPlayer = 360f;
                float playerDistanceFromDeerclops = Vector2.Distance(npc.Center, player.Center);
                if (playerDistanceFromDeerclops >= IncreaseDRTriggerDistance && playerDistanceFromDeerclops <= MaxDRIncreaseDistance)
                {
                    Vector2 spawnPosition = npc.Center + (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * (playerDistanceFromDeerclops + minShadowHandSpawnDistanceFromPlayer);
                    float shadowHandVelocity = death ? 6f : 5f;
                    Vector2 spawnVelocity = (player.Center - spawnPosition).SafeNormalize(Vector2.UnitY) * shadowHandVelocity;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition, spawnVelocity, shadowHand, HandDamage, 0f, Main.myPlayer);
                }
            }
        }

        private static void SpawnShadowHandRing(NPC npc, int shadowHand, int handDamage, int totalProjectiles, float velocityMultIncrement, float radians, float velocity, bool halveVelocityMultiplier)
        {
            Vector2 spinningPoint = new Vector2(0f, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 actualVelocity = spinningPoint.RotatedBy(radians * k);
                float velocityMultiplier = halveVelocityMultiplier ? 1f - (k * velocityMultIncrement * 0.5f) : 1f - k * velocityMultIncrement;
                Projectile.NewProjectile(npc.GetSource_FromAI(), Main.player[npc.target].Center + Vector2.Normalize(actualVelocity) * 550f, actualVelocity * velocityMultiplier * -1f, shadowHand, handDamage, 0f, Main.myPlayer);
            }
        }

        private static void ShootRubbleUp(NPC npc, ref Point sourceTileCoords, int howMany, int distancedByThisManyTiles, int whichOne, int rubble, int DebrisDamage, float lifeRatio, bool death)
        {
            // Loop to spawn rubble
            // The rubble attempts are used to offset the Y coordinates of the rubble spawns to make sure they can spawn in non-solid tiles
            int rubbleSpawnLocation = whichOne * distancedByThisManyTiles;
            int maxRubbleSpawnAttempts = 35;
            for (int rubbleSpawnAttempts = 0; rubbleSpawnAttempts < maxRubbleSpawnAttempts; rubbleSpawnAttempts++)
            {
                int posX = sourceTileCoords.X + rubbleSpawnLocation * npc.direction;
                int posY = sourceTileCoords.Y + rubbleSpawnAttempts;
                if (WorldGen.ActiveAndWalkableTile(posX, posY))
                {
                    SpawnRubble(npc, posX, posY, howMany, whichOne, rubble, DebrisDamage, lifeRatio, death);
                    break;
                }
            }
        }

        private static void SpawnRubble(NPC npc, int posX, int posY, int howMany, int whichOne, int rubble, int DebrisDamage, float lifeRatio, bool death)
        {
            Vector2 rubbleVelocity = new Vector2(0f, -1f).RotatedBy((float)(whichOne * npc.direction) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
            int ai1_FrameToUse = Main.rand.Next(Main.projFrames[rubble] * 4);
            ai1_FrameToUse = 6 + Main.rand.Next(6);
            float delay = death ? 24f : 30f;
            float ai2_DelayBeforeGoingUp = (whichOne + 1) * delay;
            float velocityMultiplier = MathHelper.Lerp(0.01f, 0.015f, 1f - lifeRatio);
            Projectile.NewProjectile(npc.GetSource_FromAI(), new Vector2(posX * 16 + 8, posY * 16 - 8), rubbleVelocity * velocityMultiplier, rubble, DebrisDamage, 0f, Main.myPlayer, 0f, ai1_FrameToUse, ai2_DelayBeforeGoingUp);
        }

        private static void MakeIceSpikes(NPC npc, int phaseCounterSlot, int iceSpike, int iceSpikeDamage, float lifeRatio, bool death, int iceSpikeGateValue, int numIceSpikes, int spikeInterval, bool bothSides)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (!(npc.ai[phaseCounterSlot] < (float)iceSpikeGateValue))
            {
                Point sourceTileCoords = npc.Bottom.ToTileCoordinates();
                int xOffsetMult = 1;
                if (!bothSides)
                    sourceTileCoords.X += npc.direction * 3;

                int screenShakeGateValue = (int)npc.ai[phaseCounterSlot] - iceSpikeGateValue;
                if (screenShakeGateValue == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "Deerclops");
                    Main.instance.CameraModifiers.Add(modifier);
                }

                int iceSpikeStart = screenShakeGateValue / spikeInterval * spikeInterval;
                int iceSpikeLimit = iceSpikeStart + spikeInterval;
                if (screenShakeGateValue % spikeInterval != 0)
                    iceSpikeLimit = iceSpikeStart;

                // Ice spikes get fucking gigantic later on in the fight
                float iceSpikeScaleIncrease = MathHelper.Lerp(1f, 2f, 1f - lifeRatio);
                if (bothSides)
                {
                    for (int iceSpikeIndex = iceSpikeStart; iceSpikeIndex >= 0 && iceSpikeIndex < iceSpikeLimit && iceSpikeIndex < numIceSpikes; iceSpikeIndex++)
                    {
                        int xOffset = (int)Math.Round(iceSpikeIndex * xOffsetMult * iceSpikeScaleIncrease);
                        TryMakingSpike(npc, ref sourceTileCoords, npc.direction, numIceSpikes, -iceSpikeIndex, xOffset, iceSpike, iceSpikeDamage, lifeRatio, death);
                        TryMakingSpike(npc, ref sourceTileCoords, -npc.direction, numIceSpikes, -iceSpikeIndex, xOffset, iceSpike, iceSpikeDamage, lifeRatio, death);
                    }
                }
                else
                {
                    for (int i = iceSpikeStart; i < iceSpikeLimit && i < numIceSpikes; i++)
                    {
                        int xOffset = (int)Math.Round(i * xOffsetMult * iceSpikeScaleIncrease);
                        TryMakingSpike(npc, ref sourceTileCoords, npc.direction, numIceSpikes, i, xOffset, iceSpike, iceSpikeDamage, lifeRatio, death);
                    }
                }
            }
        }

        private static void TryMakingSpike(NPC npc, ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset, int iceSpike, int IceSpikeDamage, float lifeRatio, bool death)
        {
            int posX = sourceTileCoords.X + xOffset * dir;
            int posY = FindBestY(npc, ref sourceTileCoords, posX);
            if (WorldGen.ActiveAndWalkableTile(posX, posY))
            {
                Vector2 iceSpikeSpawnPos = new Vector2(posX * 16 + 8, posY * 16 - 8);
                Vector2 iceSpikeVelocity = new Vector2(0f, -1f).RotatedBy((float)(whichOne * dir) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
                float iceSpikeScale = 0.1f + Main.rand.NextFloat() * 0.1f + (float)xOffset * 1.1f / (float)howMany;
                Projectile.NewProjectile(npc.GetSource_FromAI(), iceSpikeSpawnPos, iceSpikeVelocity, iceSpike, IceSpikeDamage, 0f, Main.myPlayer, 0f, iceSpikeScale);
            }
        }

        private static int FindBestY(NPC npc, ref Point sourceTileCoords, int x)
        {
            int bestY = sourceTileCoords.Y;
            NPCAimedTarget targetData = npc.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new Vector2(hitbox.Center.X, hitbox.Bottom);
                int y = (int)(vector.Y / 16f);
                int sign = Math.Sign(y - bestY);
                int y2 = y + sign * 15;
                int? potentialBestY = null;
                float yLimit = float.PositiveInfinity;
                for (int i = bestY; i != y2; i += sign)
                {
                    if (WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        float newYLimit = new Point(x, i).ToWorldCoordinates().Distance(vector);
                        if (!potentialBestY.HasValue || !(newYLimit >= yLimit))
                        {
                            potentialBestY = i;
                            yLimit = newYLimit;
                        }
                    }
                }

                if (potentialBestY.HasValue)
                    bestY = potentialBestY.Value;
            }

            for (int j = 0; j < 20; j++)
            {
                if (bestY < 10)
                    break;

                if (!WorldGen.SolidTile(x, bestY))
                    break;

                bestY--;
            }

            for (int k = 0; k < 20; k++)
            {
                if (bestY > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, bestY))
                    break;

                bestY++;
            }

            return bestY;
        }

        private static void Movement(NPC npc, float lifeRatio, bool haltMovement, bool goHome, bool death)
        {
            float moveSpeed = MathHelper.Lerp(death ? 4f : 3.5f, death ? 6f : 5f, 1f - lifeRatio);
            float moveSpeedDivisor = 4f;
            float yVelocityIncrease = death ? -0.5f : -0.4f;
            float yVelocityMin = death ? -12f : -8f;
            float yVelocityIncrease2 = death ? 0.5f : 0.4f;
            Rectangle targetHitbox = npc.GetTargetData().Hitbox;

            if (goHome)
            {
                targetHitbox = new Rectangle(npc.homeTileX * 16, npc.homeTileY * 16, 16, 16);
                if (npc.Distance(targetHitbox.Center.ToVector2()) < 240f)
                    targetHitbox.X = (int)(npc.Center.X + (float)(160 * npc.direction));
            }

            float distanceFromTargetX = (float)targetHitbox.Center.X - npc.Center.X;
            float absoluteDistanceFromTargetX = Math.Abs(distanceFromTargetX);
            if (goHome && distanceFromTargetX != 0f)
                npc.direction = (npc.spriteDirection = Math.Sign(distanceFromTargetX));

            bool closeToTarget = absoluteDistanceFromTargetX < 80f;
            bool stopMoving = closeToTarget || haltMovement;
            if (npc.ai[0] == -1f)
            {
                distanceFromTargetX = 5f;
                moveSpeed = 5.35f;
                stopMoving = false;
            }

            if (stopMoving)
            {
                npc.velocity.X *= 0.8f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
            }
            else
            {
                int moveDirection = Math.Sign(distanceFromTargetX);
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (float)moveDirection * moveSpeed, 1f / moveSpeedDivisor);
            }

            int npcCenterXOffset = 40;
            int npcCenterYOffset = 20;
            int gfxOffsetY = 0;
            Vector2 npcCenter = new Vector2(npc.Center.X - (float)(npcCenterXOffset / 2), npc.position.Y + (float)npc.height - (float)npcCenterYOffset + (float)gfxOffsetY);
            bool moveDown = npcCenter.X < (float)targetHitbox.X && npcCenter.X + (float)npc.width > (float)(targetHitbox.X + targetHitbox.Width);
            bool aboveTarget = npcCenter.Y + (float)npcCenterYOffset < (float)(targetHitbox.Y + targetHitbox.Height - 16);
            bool acceptTopSurfaces = npc.Bottom.Y >= (float)targetHitbox.Top;
            bool insideTiles = Collision.SolidCollision(npcCenter, npcCenterXOffset, npcCenterYOffset, acceptTopSurfaces);
            bool insideTiles2 = Collision.SolidCollision(npcCenter, npcCenterXOffset, npcCenterYOffset - 4, acceptTopSurfaces);
            bool moveUp = !Collision.SolidCollision(npcCenter + new Vector2(npcCenterXOffset * npc.direction, 0f), 16, 80, acceptTopSurfaces);
            float yVelocity = death ? -12f : -8f;

            if (insideTiles || insideTiles2)
                npc.localAI[0] = 0f;

            if ((moveDown || closeToTarget) && aboveTarget)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease2 * 2f, 0.001f, 16f);
            }
            else if (insideTiles && !insideTiles2)
            {
                npc.velocity.Y = 0f;
            }
            else if (insideTiles)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease, yVelocityMin, 0f);
            }
            else if (npc.velocity.Y == 0f && moveUp && npc.localAI[0] == 0f)
            {
                npc.velocity.Y = yVelocity;
                npc.localAI[0] = 1f;
            }
            else
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease2, yVelocity, 16f);
        }
    }
}
