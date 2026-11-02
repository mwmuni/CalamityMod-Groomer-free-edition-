using System;
using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    /// <summary>Controls the Lunatic Cultist and its ritual clones.</summary>
    /// <remarks>
    /// AI slots store the attack, timer, ritual projectile index, and attack-cycle step.
    /// Clones use the last slot for their owner's NPC index instead of a cycle step.
    /// </remarks>
    public class CultistAI : VanillaAIOverride
    {
        /// <summary>Base damage for clone fireballs, before difficulty scaling.</summary>
        public static int CloneFireballDamage = 18; // 72

        /// <summary>Base damage for the Cultist's fireballs, before difficulty scaling.</summary>
        public static int FireballDamage = 20; // 80

        /// <summary>Placeholder damage for the non-damaging ice mist controller.</summary>
        public static int IceMistDamage = 1; // This attack never actually hits, so damage number is irrelevant

        /// <summary>Base damage for lightning, before difficulty scaling.</summary>
        public static int LightningDamage = 30; // 120

        /// <summary>Base damage for Ancient Doom projectiles, before difficulty scaling.</summary>
        public static int DoomDamage = 45; // 180 (Also fixes Master Mode scaling)

        private enum Attack
        {
            Introduction = -1,
            Idle = 0,
            Move = 1,
            IceMist = 2,
            Fireballs = 3,
            Lightning = 4,
            Ritual = 5,
            Pause = 6,
            AncientLight = 7,
            AncientDoom = 8
        }

        private const float IdleAnimation = 10f;
        private const float ForwardCastAnimation = 11f;
        private const float UpwardCastAnimation = 12f;
        private const float SummoningAnimation = 13f;
        private const int AncientDoomLimit = 10;

        private static readonly Attack[] AttackCycle =
        {
            Attack.Move, Attack.Fireballs,
            Attack.Move, Attack.AncientLight,
            Attack.Move, Attack.Lightning,
            Attack.Move, Attack.IceMist,
            Attack.Move, Attack.AncientDoom,
            Attack.Move, Attack.AncientLight,
            Attack.Move, Attack.Lightning,
            Attack.Move, Attack.Fireballs,
            Attack.Move, Attack.IceMist,
            Attack.Move, Attack.AncientDoom,
            Attack.Move, Attack.Ritual
        };

        private static readonly Attack[] AncientDoomAlternatives =
        {
            Attack.Fireballs, Attack.IceMist, Attack.Lightning, Attack.AncientLight
        };

        // Compare floats directly: casting fractional or invalid synchronized states to an enum would change dispatch.
        private bool IsAttack(Attack attack) => NPC.ai[0] == (float)attack;

        private void FinishAttack(bool retarget = false)
        {
            NPCBehaviorUtils.SetAIState(NPC, (float)Attack.Idle);
            NPC.ai[3] += 1f;
            NPC.velocity = Vector2.Zero;
            if (retarget)
                Retarget();
            NPC.netUpdate = true;
        }

        private void Retarget()
        {
            CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
            options.faceTarget = false;
            CalamityUtils.CalamityTargeting(NPC, options);
        }

        // Keep indices in a snapshot: spawning and network callbacks can change the live NPC collection.
        private List<int> CollectClones(bool includeSelf = false)
        {
            List<int> clones = new List<int>();
            if (includeSelf)
                clones.Add(NPC.whoAmI);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone && Main.npc[i].ai[3] == NPC.whoAmI)
                    clones.Add(i);
            }
            return clones;
        }

        private static void FaceDirection(NPC npc, float directionX)
        {
            int direction = Math.Sign(directionX);
            if (direction != 0)
                npc.direction = npc.spriteDirection = direction;
        }

        private void FireCloneFireball(int cloneIndex, Player player, float fireballSpeed, int damage)
        {
            NPC clone = Main.npc[cloneIndex];
            Vector2 center = clone.Center;
            FaceDirection(clone, player.Center.X - center.X);

            Vector2 playerDirection = Vector2.Normalize(player.Center - center);
            if (playerDirection.HasNaNs())
                playerDirection = new Vector2(NPC.direction, 0f);

            Vector2 shadowFireballDirection = center + new Vector2(NPC.direction * 30, 12f);
            Vector2 shadowFireballVelocity = playerDirection * (fireballSpeed + (float)Main.rand.NextDouble());
            shadowFireballVelocity = shadowFireballVelocity.RotatedByRandom(Math.PI / 6D);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), shadowFireballDirection, shadowFireballVelocity, ProjectileID.CultistBossFireBallClone, damage, 0f, Main.myPlayer);
        }

        private void FaceRitualCenter(bool isCultist)
        {
            Vector2 ritualCenter = Main.projectile[(int)(isCultist ? NPC.ai[2] : Main.npc[(int)NPC.ai[3]].ai[2])].Center;
            Vector2 ritualCenterDirection = ritualCenter - NPC.Center;
            if (ritualCenterDirection == Vector2.Zero)
                ritualCenterDirection = -Vector2.UnitY;

            ritualCenterDirection.Normalize();

            if (Math.Abs(ritualCenterDirection.Y) < 0.77f)
                NPC.localAI[2] = ForwardCastAnimation;
            else if (ritualCenterDirection.Y < 0f)
                NPC.localAI[2] = UpwardCastAnimation;
            else
                NPC.localAI[2] = IdleAnimation;

            FaceDirection(NPC, ritualCenterDirection.X);
        }

        private void CancelRitual()
        {
            FinishAttack(retarget: true);
            Main.projectile[(int)NPC.ai[2]].ai[1] = -1f;
            Main.projectile[(int)NPC.ai[2]].netUpdate = true;

            foreach (NPC clone in Main.ActiveNPCs)
            {
                if (clone.type == NPCID.CultistBossClone && clone.ai[3] == NPC.whoAmI)
                {
                    clone.active = false;
                    clone.ForceNetUpdate();
                }
            }
        }

        private void DespawnEncounter()
        {
            NPC.life = 0;
            NPC.HitEffect(0, 10.0);
            NPC.active = false;

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone && Main.npc[i].ai[3] == NPC.whoAmI)
                {
                    Main.npc[i].life = 0;
                    Main.npc[i].HitEffect(0, 10.0);
                    Main.npc[i].active = false;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);
                }
            }
        }

        private void UpdateIntroduction()
        {
            NPC.alpha -= 5;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            NPC.ai[1] += 1f;
            if (NPC.ai[1] >= 420f)
            {
                NPCBehaviorUtils.SetAIState(NPC, (float)Attack.Idle);
                NPC.netUpdate = true;
            }
            else if (NPC.ai[1] > 360f)
            {
                NPC.velocity *= 0.95f;
                if (NPC.localAI[2] != SummoningAnimation)
                    SoundEngine.PlaySound(SoundID.Zombie105, NPC.Center);

                NPC.localAI[2] = SummoningAnimation;
            }
            else if (NPC.ai[1] > 300f)
            {
                NPC.velocity = -Vector2.UnitY;
                NPC.localAI[2] = IdleAnimation;
            }
            else if (NPC.ai[1] > 120f)
                NPC.localAI[2] = 1f;
            else
                NPC.localAI[2] = 0f;
        }

        private void UpdateIdle(Player player, bool isCultist, int idleTime, float moveSpeed)
        {
            if (NPC.ai[1] == 0f)
                Retarget();

            NPC.localAI[2] = IdleAnimation;
            FaceDirection(NPC, player.Center.X - NPC.Center.X);

            NPC.ai[1] += 1f;
            if (!(NPC.ai[1] >= idleTime && isCultist))
                return;

            int cycleStep = (int)NPC.ai[3];
            Attack attack = BossAttackCycle.Select(AttackCycle, cycleStep, Attack.Move);
            if (cycleStep < 0 || cycleStep >= AttackCycle.Length)
                NPC.ai[3] = -1f;

            if (attack == Attack.AncientDoom && NPC.CountNPCS(NPCID.AncientDoom) >= AncientDoomLimit)
                attack = AncientDoomAlternatives[Main.rand.Next(AncientDoomAlternatives.Length)];
            if (attack == Attack.Ritual)
                NPC.ai[3] = -1f;

            if (attack == Attack.Move)
                BeginMovement(player, cycleStep, moveSpeed);
            else
                NPCBehaviorUtils.SetAIState(NPC, (float)attack);

            NPC.netUpdate = true;
        }

        private void BeginMovement(Player player, int cycleStep, float moveSpeed)
        {
            float predictionDistance = 480f;
            float distanceAboveTarget = -240f;
            if (cycleStep == 2 || cycleStep == 10)
            {
                distanceAboveTarget *= 1.5f;
                predictionDistance *= 1.5f;
            }

            Vector2 prediction = new Vector2(0f + player.velocity.SafeNormalize(Vector2.Zero).X * predictionDistance, distanceAboveTarget);
            float movementTicks = (float)Math.Ceiling((player.Center + prediction - NPC.Center).Length() / moveSpeed);
            if (movementTicks == 0f)
                movementTicks = 1f;

            List<int> formation = CollectClones(includeSelf: true);
            bool evenFormation = formation.Count % 2 == 0;
            int formationIndex = 0;
            foreach (int cloneIndex in formation)
            {
                NPC clone = Main.npc[cloneIndex];
                Vector2 center = clone.Center;
                float angle = (formationIndex + evenFormation.ToInt() + 1) / 2 * MathHelper.TwoPi * 0.4f / formation.Count;
                if (formationIndex % 2 == 1)
                    angle *= -1f;
                if (formation.Count == 1)
                    angle = 0f;

                Vector2 offset = new Vector2(0f, -1f).RotatedBy(angle) * new Vector2(150f, 200f);
                Vector2 displacement = player.Center + (Vector2.UnitX * prediction.X) + offset - center;
                NPCBehaviorUtils.SetAIState(clone, (float)Attack.Move, movementTicks);
                clone.velocity = displacement / movementTicks * 2f;
                if (NPC.whoAmI >= clone.whoAmI)
                    clone.position -= clone.velocity;

                clone.netUpdate = true;
                formationIndex++;
            }
        }

        private void UpdateMovement()
        {
            NPC.localAI[2] = IdleAnimation;
            if (NPC.ai[1] % 2f != 0f && NPC.ai[1] != 1f)
                NPC.position -= NPC.velocity;

            NPC.ai[1] -= 1f;
            if (NPC.ai[1] <= 0f)
                FinishAttack();
        }

        private void BeginRitual(Player player, bool death, float lifeRatio)
        {
            NPC.localAI[1] += 1f;

            List<int> clones = CollectClones();
            int spawnLimit = 2 + (death ? 2 : 0) + (lifeRatio < 0.75f ? 1 : 0) +
                (lifeRatio < 0.5f ? 1 : 0) + (lifeRatio < 0.25f ? 1 : 0) + (lifeRatio < 0.1f ? 1 : 0);
            int newCloneCount = Math.Min(8 - clones.Count, spawnLimit);
            int formationSize = clones.Count + newCloneCount + 1;

            Vector2 RitualOffset(int slot) => new Vector2(180f, 0f).RotatedBy(slot * MathHelper.TwoPi / formationSize - MathHelper.PiOver2);
            int closestSlot = 0;
            float closestDistance = Vector2.Distance(NPC.Center + RitualOffset(0), player.Center);
            for (int slot = 1; slot < formationSize; slot++)
            {
                float distance = Vector2.Distance(NPC.Center + RitualOffset(slot), player.Center);
                if (closestDistance > distance)
                {
                    closestSlot = slot;
                    closestDistance = distance;
                }
            }
            int cultistSlot = closestSlot < formationSize / 2 ? closestSlot + formationSize / 2 : closestSlot - formationSize / 2;

            int cloneIndex = 0;
            for (int slot = 0; slot < formationSize; slot++)
            {
                if (slot == cultistSlot)
                    continue;

                Vector2 position = NPC.Center + RitualOffset(slot);
                if (cloneIndex < newCloneCount)
                {
                    int spawnedClone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y + NPC.height / 2, NPCID.CultistBossClone, NPC.whoAmI);
                    Main.npc[spawnedClone].ai[3] = NPC.whoAmI;
                    Main.npc[spawnedClone].netUpdate = true;
                    Main.npc[spawnedClone].localAI[1] = NPC.localAI[1];
                }
                else
                {
                    int existingClone = clones[cloneIndex - newCloneCount];
                    Main.npc[existingClone].Center = position;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, existingClone);
                }
                cloneIndex++;
            }

            NPC.ai[2] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileID.CultistRitual, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
            NPC.Center += RitualOffset(cultistSlot);
            NPC.netUpdate = true;
        }

        private void SpawnAncientLightSpread(Vector2 playerDirection, bool death)
        {
            Vector2 origin = NPC.Center;
            float speed = death ? 5f : 4f;
            float spread = MathHelper.ToRadians(CalamityWorld.death ? 90f : 80f);
            int count = death ? 9 : 7;
            float intervals = count - 1;
            for (int i = 0; i < count; i++)
            {
                float edgeDistance = Math.Abs(i - (intervals * 0.5f)) / (intervals * 0.5f);
                float angle = spread * (i / intervals) - spread * 0.5f;
                Vector2 velocity = playerDirection.RotatedBy(angle) * speed * (0.75f + 1.5f * edgeDistance);
                float angularVelocity = (Main.rand.NextFloat() - 0.5f) * 0.2f * MathHelper.TwoPi / 60f;
                int light = NPC.NewNPC(NPC.GetSource_FromAI(), (int)origin.X, (int)origin.Y + 7, NPCID.AncientLight, 0, 0f, angularVelocity, velocity.X, velocity.Y);
                Main.npc[light].velocity = velocity;
            }
        }

        /// <summary>Updates the Cultist or clone using the existing synchronized attack state.</summary>
        /// <param name="mod">The mod that owns this AI override.</param>
        /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Chant sound
            if (!IsAttack(Attack.Introduction) && Main.rand.NextBool(1000))
            {
                SoundStyle chantSound = Utils.SelectRandom(Main.rand, new SoundStyle[]
                {
                    SoundID.Zombie88,
                    SoundID.Zombie89,
                    SoundID.Zombie90,
                    SoundID.Zombie91
                });

                SoundEngine.PlaySound(chantSound, NPC.Center);
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool phase2 = lifeRatio < 0.85f || death;
            bool phase3 = lifeRatio < 0.7f || death;
            bool phase4 = lifeRatio < (death ? 0.8f : 0.55f);
            bool phase5 = lifeRatio < (death ? 0.6f : 0.4f);
            bool phase6 = lifeRatio < (death ? 0.4f : 0.25f);
            bool phase7 = death && lifeRatio < 0.2f;
            bool phase8 = death && lifeRatio < 0.1f;

            // Variables
            bool isCultist = NPC.type == NPCID.CultistBoss;
            bool dontTakeDamage = false;

            float moveSpeed = death ? 300f : 75f;

            int iceMistDamage = isCultist ? IceMistDamage : 0;
            int fireballDamage = isCultist ? FireballDamage : CloneFireballDamage;
            int lightningDamage = isCultist ? LightningDamage : 0;

            int iceMistFireInterval = phase2 ? 50 : 60;
            float iceMistSpeed = 12f + (death ? 4f : 2f) * (1f - lifeRatio);
            int iceMistVolleyCount = phase3 ? 2 : 1;
            int fireballFireInterval = (phase5 ? 20 : 24) - (death ? 5 : 0);
            float fireballSpeed = ((phase7 ? 8f : phase6 ? 7f : 6f) + (death ? (1f - lifeRatio) : 0f)) * (isCultist ? 1f : 0.5f);
            int fireballVolleyCount = death ? 8 : 4;
            int lightningOrbPhaseTime = phase2 ? 90 : 120;
            int ancientLightSpawnInterval = phase7 ? 20 : phase4 ? 25 : 30;
            int ancientLightVolleyCount = phase7 ? 4 : phase4 ? 3 : 2;
            int idleTime = phase8 ? 40 : phase7 ? 45 : phase3 ? 55 : 60;
            float timeToFinishRitual = phase8 ? 180f : phase7 ? 240f : phase5 ? 300f : 360f;

            if (Main.getGoodWorld)
            {
                iceMistFireInterval = 40;
                iceMistSpeed = 15f;
                fireballFireInterval = 8;
                fireballSpeed *= 1.25f;
                lightningOrbPhaseTime = 60;
                ancientLightSpawnInterval = 10;
                ancientLightVolleyCount = 5;
                idleTime = 20;
            }

            // Center and target
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active || Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
            {
                Retarget();

                player = Main.player[NPC.target];
                NPC.netUpdate = true;
            }

            // Enrage
            if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
            {
                calamityGlobalNPC.newAI[0] += 1f;
                if (calamityGlobalNPC.newAI[0] >= 120f)
                {
                    calamityGlobalNPC.newAI[0] = 120f;
                    iceMistSpeed = 16f;
                    iceMistFireInterval = 15;
                    lightningOrbPhaseTime = 30;
                    ancientLightSpawnInterval = 5;
                    idleTime = 10;
                    timeToFinishRitual = 120f;
                }
            }
            else if (calamityGlobalNPC.newAI[0] > 0f)
                calamityGlobalNPC.newAI[0] -= 1f;

            // Cultist clone AI
            if (!isCultist)
            {
                if (NPC.ai[3] < 0f || !Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != NPCID.CultistBoss)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                    return false;
                }

                NPC.ai[0] = Main.npc[(int)NPC.ai[3]].ai[0];
                NPC.ai[1] = Main.npc[(int)NPC.ai[3]].ai[1];
                dontTakeDamage = true;

                if (IsAttack(Attack.Ritual) && NPC.ai[1] >= 120f && NPC.ai[1] < timeToFinishRitual)
                {
                    dontTakeDamage = false;
                    if (NPC.justHit)
                        Main.npc[(int)NPC.ai[3]].ai[1] = timeToFinishRitual;
                }
            }

            // Stop spawning ritual if hit
            else if (IsAttack(Attack.Ritual) && NPC.ai[1] >= 120f && NPC.ai[1] < timeToFinishRitual && NPC.justHit)
                CancelRitual();

            // Despawn
            if (player.dead || !player.active || Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
                DespawnEncounter();

            // Clones set to Cultist phase
            float clonePhase = NPC.ai[3];

            // Spawn and play sound
            if (NPC.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie89, NPC.Center);
                NPC.localAI[0] = 1f;
                NPC.alpha = 255;
                NPC.rotation = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = (float)Attack.Introduction;
                    NPC.netUpdate = true;
                }
            }

            if (IsAttack(Attack.Introduction))
            {
                UpdateIntroduction();
                dontTakeDamage = true;
            }

            // Only the branch executed this tick supplies a duration. Selecting an attack does not advance it yet.
            float? attackDuration = null;
            bool retargetAfterAttack = false;

            if (IsAttack(Attack.Idle))
                UpdateIdle(player, isCultist, idleTime, moveSpeed);
            else if (IsAttack(Attack.Move))
                UpdateMovement();

            // Repeated aimed volleys: ice mist, fireballs, and Ancient Light.
            else if (IsAttack(Attack.IceMist) || IsAttack(Attack.Fireballs) || IsAttack(Attack.AncientLight))
            {
                Attack attack = (Attack)NPC.ai[0];
                int fireInterval = attack == Attack.IceMist ? iceMistFireInterval : attack == Attack.Fireballs ? fireballFireInterval : ancientLightSpawnInterval;
                NPC.localAI[2] = ForwardCastAnimation;

                if (NPC.ai[1] >= 4f && isCultist && (int)(NPC.ai[1] - 4f) % fireInterval == 0 &&
                    (attack != Attack.AncientLight || (int)(NPC.ai[1] - 4f) / fireInterval <= (death ? 4 : 3)))
                {
                    // Ice clones fire every volley, fireball clones fire the third, and light clones only face the target.
                    if (attack == Attack.IceMist ? Main.netMode != NetmodeID.MultiplayerClient : (int)(NPC.ai[1] - 4f) / fireInterval == 2)
                    {
                        List<int> clones = CollectClones();

                        if (attack == Attack.AncientLight)
                        {
                            foreach (int cloneIndex in clones)
                            {
                                NPC clone = Main.npc[cloneIndex];
                                Vector2 center = clone.Center;
                                FaceDirection(clone, player.Center.X - center.X);
                            }
                        }
                        else if (attack == Attack.IceMist || Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            foreach (int cloneIndex in clones)
                                FireCloneFireball(cloneIndex, player, fireballSpeed, attack == Attack.IceMist ? fireballDamage : CloneFireballDamage);
                        }
                    }

                    if (attack != Attack.IceMist)
                        FaceDirection(NPC, player.Center.X - NPC.Center.X);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 playerDirection = attack == Attack.Fireballs ? NPC.DirectionTo(player.Center) : Vector2.Normalize(player.Center - NPC.Center);
                        if (playerDirection.HasNaNs())
                            playerDirection = new Vector2(NPC.direction, 0f);

                        if (attack == Attack.IceMist)
                        {
                            Vector2 iceMistDirection = NPC.Center + new Vector2(NPC.direction * 30, 12f);
                            Vector2 iceMistVelocity = playerDirection * iceMistSpeed;
                            Projectile iceMist = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), iceMistDirection, iceMistVelocity, ProjectileID.CultistBossIceMist, iceMistDamage, 0f, Main.myPlayer, 0f, 1f)];
                            iceMist.timeLeft = 240;
                        }
                        else if (attack == Attack.Fireballs)
                        {
                            Vector2 fireballDirection = NPC.Center + new Vector2(NPC.direction * 30, 12f);
                            Vector2 fireballVelocity = playerDirection * (fireballSpeed + (float)Main.rand.NextDouble() * 2f);
                            fireballVelocity = fireballVelocity.RotatedByRandom(Math.PI / 6D);
                            for (var i = -1; i <= 1; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireballDirection, fireballVelocity.RotatedBy(1f * i), ProjectileID.CultistBossFireBall, fireballDamage, 0f, Main.myPlayer);
                        }
                        else
                        {
                            SpawnAncientLightSpread(playerDirection, death);
                        }
                    }
                }

                attackDuration = attack == Attack.IceMist ? 4 + iceMistFireInterval * iceMistVolleyCount :
                    attack == Attack.Fireballs ? 4 + fireballFireInterval * fireballVolleyCount : 30 + ancientLightSpawnInterval * ancientLightVolleyCount;
            }

            // Lightning Orb
            else if (IsAttack(Attack.Lightning))
            {
                NPC.localAI[2] = isCultist ? UpwardCastAnimation : ForwardCastAnimation;

                if (NPC.ai[1] == 20f && isCultist && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (int cloneIndex in CollectClones())
                        FireCloneFireball(cloneIndex, player, fireballSpeed, fireballDamage);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + (death ? 210f : -100f), 0f, 0f, ProjectileID.CultistBossLightningOrb, lightningDamage, 0f, Main.myPlayer);
                    if (death)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 210f, NPC.Center.Y - 210f, 0f, 0f, ProjectileID.CultistBossLightningOrb, lightningDamage, 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 210f, NPC.Center.Y - 210f, 0f, 0f, ProjectileID.CultistBossLightningOrb, lightningDamage, 0f, Main.myPlayer);
                    }
                }

                attackDuration = 20 + lightningOrbPhaseTime;
            }

            // Spawn Clones, and Dragon or Cthulhu head
            else if (IsAttack(Attack.Ritual))
            {
                NPC.localAI[2] = IdleAnimation;

                if (NPC.ai[1] >= 0f && NPC.ai[1] < 30f)
                {
                    dontTakeDamage = true;
                    float cultistAlphaControl = NPC.ai[1] / 30f;
                    NPC.alpha = (int)(cultistAlphaControl * 255f);
                }
                else if (NPC.ai[1] >= 30f && NPC.ai[1] < 90f)
                {
                    if (NPC.ai[1] == 30f && Main.netMode != NetmodeID.MultiplayerClient && isCultist)
                        BeginRitual(player, death, lifeRatio);

                    dontTakeDamage = true;
                    NPC.alpha = 255;

                    FaceRitualCenter(isCultist);
                }
                else if (NPC.ai[1] >= 90f && NPC.ai[1] < 120f)
                {
                    dontTakeDamage = true;
                    float ritualAlphaControl = (NPC.ai[1] - 90f) / 30f;
                    NPC.alpha = 255 - (int)(ritualAlphaControl * 255f);
                }
                else if (NPC.ai[1] >= 120f && NPC.ai[1] < timeToFinishRitual)
                {
                    NPC.alpha = 0;

                    FaceRitualCenter(isCultist);
                }

                attackDuration = timeToFinishRitual;
                retargetAfterAttack = true;
            }

            // Pause
            else if (IsAttack(Attack.Pause))
            {
                NPC.localAI[2] = SummoningAnimation;

                attackDuration = idleTime * 3;
            }

            // Ancient Doom
            else if (IsAttack(Attack.AncientDoom))
            {
                NPC.localAI[2] = SummoningAnimation;

                if (NPC.ai[1] >= 4f && isCultist && (int)(NPC.ai[1] - 4f) % 20f == 0f && NPC.ai[1] <= 64)
                {
                    int ancientDoomAmount = 2 + (lifeRatio < 0.75f && death ? 1 : 0) +
                        (lifeRatio < 0.5f ? 1 : 0) + (lifeRatio < 0.25f && death ? 1 : 0);

                    FaceDirection(NPC, player.Center.X - NPC.Center.X);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Spawn circle of Ancient Dooms around target
                        for (int i = 0; i < ancientDoomAmount; i++)
                        {
                            float ai2 = i * (360f / ancientDoomAmount);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.Center.X + (float)(Math.Sin(i * 120) * 550)), (int)(player.Center.Y + (float)(Math.Cos(i * 120) * 550)),
                                NPCID.AncientDoom, 0, NPC.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
                        }
                    }
                }

                attackDuration = 195f;
            }

            if (attackDuration.HasValue)
            {
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= attackDuration.Value)
                    FinishAttack(retargetAfterAttack);
            }

            // Set Clones to Cultist phase
            if (!isCultist)
                NPC.ai[3] = clonePhase;

            // Take damage or not
            NPC.dontTakeDamage = dontTakeDamage;
            NPC.chaseable = !IsAttack(Attack.Introduction) && !IsAttack(Attack.Ritual);

            return false;
        }

        /// <summary>Controls Ancient Light acceleration, curved flight, and expiration.</summary>
        public class AncientLightAI : VanillaAIOverride
        {
            /// <summary>Updates an Ancient Light and its visual effects.</summary>
            /// <param name="mod">The mod that owns this AI override.</param>
            /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
            public override bool AI(Mod mod)
            {
                NPC.dontTakeDamage = true;

                // Slow and kill code
                if (NPC.ai[0] == -1f)
                {
                    // Slow down over time
                    if (NPC.velocity.Length() >= 0.2f)
                        NPC.velocity *= 0.96f;
                    else
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.position = NPC.oldPosition;

                        // Kill after half a second
                        NPC.ai[1] += 1f;
                        if (NPC.ai[1] >= 30f)
                        {
                            NPC.HitEffect(0, 9999.0);
                            NPC.active = false;
                        }
                    }

                    return false;
                }

                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                // Set velocity and emit dust when spawned
                if (NPC.localAI[0] == 0f)
                {
                    NPC.localAI[0] = 1f;
                    NPC.velocity.X = NPC.ai[2];
                    NPC.velocity.Y = NPC.ai[3];
                    for (int i = 0; i < 13; i++)
                    {
                        int ancientLight = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 2.5f);
                        Main.dust[ancientLight].noGravity = true;
                        Main.dust[ancientLight].fadeIn = 1f;
                        Dust dust = Main.dust[ancientLight];
                        dust.velocity *= 4f;
                        Main.dust[ancientLight].noLight = true;
                    }
                }

                // Spawn dust
                for (int j = 0; j < 2; j++)
                {
                    if (Main.rand.Next(10 - (int)Math.Min(7f, NPC.velocity.Length())) < 1)
                    {
                        int ancientLight2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 2.5f);
                        Main.dust[ancientLight2].noGravity = true;
                        Dust dust = Main.dust[ancientLight2];
                        dust.velocity *= 0.2f;
                        Main.dust[ancientLight2].fadeIn = 0.4f;
                        if (Main.rand.NextBool(6))
                        {
                            dust = Main.dust[ancientLight2];
                            dust.velocity *= 5f;
                            Main.dust[ancientLight2].noLight = true;
                        }
                        else
                            Main.dust[ancientLight2].velocity = NPC.DirectionFrom(Main.dust[ancientLight2].position) * Main.dust[ancientLight2].velocity.Length();
                    }
                }

                if (NPC.ai[0] >= 0f)
                {
                    // Triple damage if the Primordial Wyrm is alive
                    if (NPC.ai[0] == 0f && CalamityGlobalNPC.adultEidolonWyrmHead != -1 &&
                        Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                        NPC.damage = (int)Math.Round(NPC.defDamage * PrimordialWyrmHead.LightDamageMult);

                    NPC.ai[0] += 1f;

                    float duration = 120f;

                    // Increase velocity for 1 second
                    if (NPC.ai[0] < duration - 60f && NPC.velocity.Length() < 20f)
                        NPC.velocity *= 1.03f;

                    // Intersect velocity paths with other Ancient Lights for 1 second
                    if (NPC.ai[0] >= duration - 60f)
                        NPC.velocity = NPC.velocity.RotatedBy(NPC.ai[1]);

                    // Engage slow code
                    if (NPC.ai[0] >= duration)
                        NPC.ai[0] = -1f;
                }

                return false;
            }
        }

        /// <summary>Controls Ancient Dooms owned by the Cultist or Primordial Wyrm.</summary>
        public class AncientDoomAI : VanillaAIOverride
        {
            private enum OrbitDustStyle
            {
                Shadowflame,
                Granite,
                StationaryGranite
            }

            private void SpawnOrbitDust(OrbitDustStyle style)
            {
                if (!Main.rand.NextBool(6))
                    return;

                bool shadowflame = style == OrbitDustStyle.Shadowflame;
                bool stationary = style == OrbitDustStyle.StationaryGranite;
                Vector2 offset = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Dust dust = Main.dust[Dust.NewDust(NPC.Center - offset * (shadowflame ? 20f : 30f), 0, 0,
                    shadowflame ? DustID.Shadowflame : DustID.Granite, 0f, 0f, 0, default, 1f)];
                if (!stationary)
                    dust.noGravity = true;
                dust.position = NPC.Center - offset * (shadowflame ? Main.rand.Next(10, 21) : 20f) * NPC.scale;
                dust.velocity = stationary ? Vector2.Zero : offset.RotatedBy(shadowflame ? MathHelper.PiOver2 : -MathHelper.PiOver2) * (shadowflame ? 4f : 2f);
                dust.scale = 0.5f + Main.rand.NextFloat();
                dust.fadeIn = 0.5f;
                if (stationary)
                    dust.noLight = true;
            }

            /// <summary>Updates an Ancient Doom's orbit, visual effects, and final projectile burst.</summary>
            /// <param name="mod">The mod that owns this AI override.</param>
            /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
                NPC.damage = NPC.defDamage = 0;
                float duration = 420f;
                float spawnAnimTime = 120f;
                float splitProjVelocity = death ? 4.5f : 3f;

                NPC owner = Main.npc[(int)NPC.ai[0]];
                bool isWyrm = owner.type == ModContent.NPCType<PrimordialWyrmHead>();
                bool validOwner = owner.type == NPCID.CultistBoss || isWyrm;
                bool kill = NPC.ai[1] < 0f || !owner.active || !validOwner;
                int target = validOwner ? owner.target : Main.maxPlayers;
                if (isWyrm)
                    NPC.dontTakeDamage = true;

                NPC.ai[1] += 6f;
                float growthRate = MathHelper.Clamp(NPC.ai[1] / spawnAnimTime, 0f, 1f);
                NPC.position = NPC.Center;
                NPC.scale = MathHelper.Lerp(0f, 1f, growthRate);
                NPC.Center = NPC.position;
                NPC.alpha = (int)(255f - growthRate * 255f);

                if (NPC.ai[3] == 0f)
                    NPC.ai[3] = NPC.ai[2];

                double rad = NPC.ai[3] * (Math.PI / 180);
                double dist = 550;
                if (isWyrm)
                {
                    float aiGateValue = owner.Calamity().newAI[2] - 30f;
                    int ancientDoomScale = (int)(aiGateValue / 120f);
                    dist += ancientDoomScale * 45;
                }
                NPC.position.X = Main.player[target].Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
                NPC.position.Y = Main.player[target].Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
                NPC.ai[3] += 8f * (1f - (NPC.ai[1] / duration));

                SpawnOrbitDust(OrbitDustStyle.Shadowflame);
                SpawnOrbitDust(OrbitDustStyle.Granite);
                SpawnOrbitDust(OrbitDustStyle.StationaryGranite);

                NPC.localAI[0] += (float)(Math.PI / 60D);

                NPC.localAI[1] = 0.25f + Vector2.UnitY.RotatedBy(NPC.ai[1] * MathHelper.TwoPi / 60f).Y * 0.25f;

                if (NPC.ai[1] >= duration)
                {
                    int damage = DoomDamage;

                    // Increase damage if the Primordial Wyrm is alive
                    if (Main.npc[(int)NPC.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
                        damage = (int)Math.Round(damage * PrimordialWyrmHead.DoomDamageMult);

                    kill = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int totalProjectiles = Main.getGoodWorld ? 5 : 3;
                        Vector2 spinningPoint = new Vector2(0f, -splitProjVelocity);
                        var rotOffset = NPC.DirectionTo(Main.player[target].Center).ToRotation();
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 doomProjRotate = spinningPoint.RotatedBy(rotOffset + MathHelper.Pi * ((k + 1) / (float)(totalProjectiles + 1)));
                            var proj = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, doomProjRotate, ProjectileID.AncientDoomProjectile, damage, 0f, Main.myPlayer)];
                            proj.tileCollide = false;
                        }
                    }
                }

                if (kill)
                {
                    NPC.HitEffect(0, 9999.0);
                    NPC.active = false;
                }

                return false;
            }
        }
    }
}
