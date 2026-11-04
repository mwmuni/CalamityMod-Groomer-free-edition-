using System;
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

        private static readonly SoundStyle[] ChantSounds =
        {
            SoundID.Zombie88, SoundID.Zombie89, SoundID.Zombie90, SoundID.Zombie91
        };

        private readonly record struct Encounter(float LifeRatio, bool Death, bool IsCultist);
        private readonly record struct Volley(int Interval, int Count, float Speed = 0f, int Damage = 0);
        private readonly record struct AttackTuning(
            Volley IceMist, Volley Fireballs, Volley AncientLight,
            int LightningTime, int LightningDamage, int IdleTime, float RitualTime);
        private readonly record struct AttackUpdate(float? Duration = null, bool Retarget = false, bool? Invulnerable = null);

        private static AttackTuning CreateTuning(in Encounter encounter)
        {
            float lifeRatio = encounter.LifeRatio;
            bool death = encounter.Death;
            bool phase2 = lifeRatio < 0.85f || death;
            bool phase3 = lifeRatio < 0.7f || death;
            bool phase4 = lifeRatio < (death ? 0.8f : 0.55f);
            bool phase5 = lifeRatio < (death ? 0.6f : 0.4f);
            bool phase6 = lifeRatio < (death ? 0.4f : 0.25f);
            bool phase7 = death && lifeRatio < 0.2f;
            bool phase8 = death && lifeRatio < 0.1f;

            AttackTuning tuning = new(
                new Volley(phase2 ? 50 : 60, phase3 ? 2 : 1,
                    12f + (death ? 4f : 2f) * (1f - lifeRatio), encounter.IsCultist ? IceMistDamage : 0),
                new Volley((phase5 ? 20 : 24) - (death ? 5 : 0), death ? 8 : 4,
                    ((phase7 ? 8f : phase6 ? 7f : 6f) + (death ? (1f - lifeRatio) : 0f)) * (encounter.IsCultist ? 1f : 0.5f),
                    encounter.IsCultist ? FireballDamage : CloneFireballDamage),
                new Volley(phase7 ? 20 : phase4 ? 25 : 30, phase7 ? 4 : phase4 ? 3 : 2),
                phase2 ? 90 : 120, encounter.IsCultist ? LightningDamage : 0,
                phase8 ? 40 : phase7 ? 45 : phase3 ? 55 : 60,
                phase8 ? 180f : phase7 ? 240f : phase5 ? 300f : 360f);

            return Main.getGoodWorld ? tuning with
            {
                IceMist = tuning.IceMist with { Interval = 40, Speed = 15f },
                Fireballs = tuning.Fireballs with { Interval = 8, Speed = tuning.Fireballs.Speed * 1.25f },
                LightningTime = 60,
                AncientLight = tuning.AncientLight with { Interval = 10, Count = 5 },
                IdleTime = 20
            } : tuning;
        }

        private AttackTuning UpdateEnrage(Player player, CalamityGlobalNPC globalNPC, in AttackTuning tuning)
        {
            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
            {
                if (globalNPC.newAI[0] > 0f)
                    globalNPC.newAI[0] -= 1f;
                return tuning;
            }

            globalNPC.newAI[0] += 1f;
            if (!(globalNPC.newAI[0] >= 120f))
                return tuning;

            globalNPC.newAI[0] = 120f;
            return tuning with
            {
                IceMist = tuning.IceMist with { Speed = 16f, Interval = 15 },
                LightningTime = 30,
                AncientLight = tuning.AncientLight with { Interval = 5 },
                IdleTime = 10,
                RitualTime = 120f
            };
        }

        private void PlayChant()
        {
            if (IsAttack(Attack.Introduction) || !Main.rand.NextBool(1000))
                return;

            SoundStyle chant = Utils.SelectRandom(Main.rand, ChantSounds);
            SoundEngine.PlaySound(chant, NPC.Center);
        }

        private Player GetTarget()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || ShouldDespawn(player))
            {
                Retarget();
                player = Main.player[NPC.target];
                NPC.netUpdate = true;
            }
            return player;
        }

        private bool ShouldDespawn(Player player) =>
            player.dead || !player.active || Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

        private bool IsRitualVulnerable(float duration) =>
            IsAttack(Attack.Ritual) && NPC.ai[1] >= 120f && NPC.ai[1] < duration;

        private bool TrySynchronizeClone(float ritualTime, out bool invulnerable)
        {
            invulnerable = true;
            if (NPC.ai[3] < 0f || !Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != NPCID.CultistBoss)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return false;
            }

            NPC.ai[0] = Main.npc[(int)NPC.ai[3]].ai[0];
            NPC.ai[1] = Main.npc[(int)NPC.ai[3]].ai[1];
            invulnerable = !IsRitualVulnerable(ritualTime);
            if (!invulnerable && NPC.justHit)
                Main.npc[(int)NPC.ai[3]].ai[1] = ritualTime;
            return true;
        }

        private void InitializeEncounter()
        {
            if (NPC.localAI[0] != 0f)
                return;

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

        // Snapshot indices into caller-owned stack storage; callbacks can change the live NPC collection.
        private ReadOnlySpan<int> CollectClones(Span<int> buffer, bool includeSelf = false)
        {
            int count = 0;
            if (includeSelf)
                buffer[count++] = NPC.whoAmI;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (candidate.active && candidate.type == NPCID.CultistBossClone && candidate.ai[3] == NPC.whoAmI)
                    buffer[count++] = i;
            }
            return buffer[..count];
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

            ReadOnlySpan<int> formation = CollectClones(stackalloc int[Main.maxNPCs + 1], includeSelf: true);
            bool evenFormation = formation.Length % 2 == 0;
            int formationIndex = 0;
            foreach (int cloneIndex in formation)
            {
                NPC clone = Main.npc[cloneIndex];
                Vector2 center = clone.Center;
                float angle = (formationIndex + evenFormation.ToInt() + 1) / 2 * MathHelper.TwoPi * 0.4f / formation.Length;
                if (formationIndex % 2 == 1)
                    angle *= -1f;
                if (formation.Length == 1)
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

            ReadOnlySpan<int> clones = CollectClones(stackalloc int[Main.maxNPCs]);
            int spawnLimit = 2 + (death ? 2 : 0) + (lifeRatio < 0.75f ? 1 : 0) +
                (lifeRatio < 0.5f ? 1 : 0) + (lifeRatio < 0.25f ? 1 : 0) + (lifeRatio < 0.1f ? 1 : 0);
            int newCloneCount = Math.Min(8 - clones.Length, spawnLimit);
            int formationSize = clones.Length + newCloneCount + 1;
            Span<Vector2> offsets = stackalloc Vector2[formationSize];
            for (int slot = 0; slot < offsets.Length; slot++)
                offsets[slot] = RitualOffset(slot, formationSize);
            int cultistSlot = FindCultistRitualSlot(player, offsets);
            ArrangeRitualClones(clones, newCloneCount, cultistSlot, offsets);

            NPC.ai[2] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileID.CultistRitual, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
            NPC.Center += offsets[cultistSlot];
            NPC.netUpdate = true;
        }

        private static Vector2 RitualOffset(int slot, int formationSize) =>
            new Vector2(180f, 0f).RotatedBy(slot * MathHelper.TwoPi / formationSize - MathHelper.PiOver2);

        private int FindCultistRitualSlot(Player player, ReadOnlySpan<Vector2> offsets)
        {
            int closestSlot = 0;
            float closestDistance = Vector2.Distance(NPC.Center + offsets[0], player.Center);
            for (int slot = 1; slot < offsets.Length; slot++)
            {
                float distance = Vector2.Distance(NPC.Center + offsets[slot], player.Center);
                if (closestDistance > distance)
                {
                    closestSlot = slot;
                    closestDistance = distance;
                }
            }
            return closestSlot < offsets.Length / 2 ? closestSlot + offsets.Length / 2 : closestSlot - offsets.Length / 2;
        }

        private void ArrangeRitualClones(ReadOnlySpan<int> clones, int newCloneCount, int cultistSlot, ReadOnlySpan<Vector2> offsets)
        {
            int cloneIndex = 0;
            for (int slot = 0; slot < offsets.Length; slot++)
            {
                if (slot == cultistSlot)
                    continue;

                Vector2 position = NPC.Center + offsets[slot];
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

        private AttackUpdate UpdateAttack(Player player, in Encounter encounter, in AttackTuning tuning)
        {
            // Switch on the float itself, not a truncating enum cast.
            switch (NPC.ai[0])
            {
                case (float)Attack.Idle:
                    UpdateIdle(player, encounter.IsCultist, tuning.IdleTime, encounter.Death ? 300f : 75f);
                    return default;
                case (float)Attack.Move:
                    UpdateMovement();
                    return default;
                case (float)Attack.IceMist:
                case (float)Attack.Fireballs:
                case (float)Attack.AncientLight:
                    return new(UpdateAimedAttack(player, (Attack)NPC.ai[0], encounter, tuning));
                case (float)Attack.Lightning:
                    return new(UpdateLightning(player, encounter, tuning));
                case (float)Attack.Ritual:
                    return UpdateRitual(player, encounter, tuning);
                case (float)Attack.Pause:
                    NPC.localAI[2] = SummoningAnimation;
                    return new(tuning.IdleTime * 3);
                case (float)Attack.AncientDoom:
                    UpdateAncientDoom(player, encounter);
                    return new(195f);
                default:
                    return default;
            }
        }

        private void AdvanceAttack(in AttackUpdate update)
        {
            // Only an executed timed attack advances; selecting the next attack does not advance it yet.
            if (!update.Duration.HasValue)
                return;

            NPC.ai[1] += 1f;
            if (NPC.ai[1] >= update.Duration.Value)
                FinishAttack(update.Retarget);
        }

        private float UpdateAimedAttack(Player player, Attack attack, in Encounter encounter, in AttackTuning tuning)
        {
            Volley volley = attack switch
            {
                Attack.IceMist => tuning.IceMist,
                Attack.Fireballs => tuning.Fireballs,
                _ => tuning.AncientLight
            };
            NPC.localAI[2] = ForwardCastAnimation;

            if (NPC.ai[1] >= 4f && encounter.IsCultist && (int)(NPC.ai[1] - 4f) % volley.Interval == 0 &&
                (attack != Attack.AncientLight || (int)(NPC.ai[1] - 4f) / volley.Interval <= (encounter.Death ? 4 : 3)))
            {
                if (attack == Attack.IceMist ? Main.netMode != NetmodeID.MultiplayerClient : (int)(NPC.ai[1] - 4f) / volley.Interval == 2)
                    UpdateVolleyClones(player, attack, tuning);

                if (attack != Attack.IceMist)
                    FaceDirection(NPC, player.Center.X - NPC.Center.X);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    FireAimedVolley(player, attack, encounter, tuning);
            }

            return (attack == Attack.AncientLight ? 30 : 4) + volley.Interval * volley.Count;
        }

        private void UpdateVolleyClones(Player player, Attack attack, in AttackTuning tuning)
        {
            ReadOnlySpan<int> clones = CollectClones(stackalloc int[Main.maxNPCs]);
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
                    FireCloneFireball(cloneIndex, player, tuning.Fireballs.Speed, attack == Attack.IceMist ? tuning.Fireballs.Damage : CloneFireballDamage);
            }
        }

        private void FireAimedVolley(Player player, Attack attack, in Encounter encounter, in AttackTuning tuning)
        {
            Vector2 direction = attack == Attack.Fireballs ? NPC.DirectionTo(player.Center) : Vector2.Normalize(player.Center - NPC.Center);
            if (direction.HasNaNs())
                direction = new Vector2(NPC.direction, 0f);

            switch (attack)
            {
                case Attack.IceMist:
                    FireIceMist(direction, tuning.IceMist);
                    break;
                case Attack.Fireballs:
                    FireFireballSpread(direction, tuning.Fireballs);
                    break;
                case Attack.AncientLight:
                    SpawnAncientLightSpread(direction, encounter.Death);
                    break;
            }
        }

        private void FireIceMist(Vector2 direction, in Volley volley)
        {
            Vector2 origin = NPC.Center + new Vector2(NPC.direction * 30, 12f);
            Vector2 velocity = direction * volley.Speed;
            Projectile mist = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), origin, velocity, ProjectileID.CultistBossIceMist, volley.Damage, 0f, Main.myPlayer, 0f, 1f)];
            mist.timeLeft = 240;
        }

        private void FireFireballSpread(Vector2 direction, in Volley volley)
        {
            Vector2 origin = NPC.Center + new Vector2(NPC.direction * 30, 12f);
            Vector2 velocity = direction * (volley.Speed + (float)Main.rand.NextDouble() * 2f);
            velocity = velocity.RotatedByRandom(Math.PI / 6D);
            for (int i = -1; i <= 1; i++)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), origin, velocity.RotatedBy(1f * i), ProjectileID.CultistBossFireBall, volley.Damage, 0f, Main.myPlayer);
        }

        private float UpdateLightning(Player player, in Encounter encounter, in AttackTuning tuning)
        {
            NPC.localAI[2] = encounter.IsCultist ? UpwardCastAnimation : ForwardCastAnimation;
            if (NPC.ai[1] == 20f && encounter.IsCultist && Main.netMode != NetmodeID.MultiplayerClient)
            {
                foreach (int cloneIndex in CollectClones(stackalloc int[Main.maxNPCs]))
                    FireCloneFireball(cloneIndex, player, tuning.Fireballs.Speed, tuning.Fireballs.Damage);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + (encounter.Death ? 210f : -100f), 0f, 0f, ProjectileID.CultistBossLightningOrb, tuning.LightningDamage, 0f, Main.myPlayer);
                if (encounter.Death)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 210f, NPC.Center.Y - 210f, 0f, 0f, ProjectileID.CultistBossLightningOrb, tuning.LightningDamage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 210f, NPC.Center.Y - 210f, 0f, 0f, ProjectileID.CultistBossLightningOrb, tuning.LightningDamage, 0f, Main.myPlayer);
                }
            }
            return 20 + tuning.LightningTime;
        }

        private AttackUpdate UpdateRitual(Player player, in Encounter encounter, in AttackTuning tuning)
        {
            NPC.localAI[2] = IdleAnimation;
            bool? invulnerable = null;
            if (NPC.ai[1] >= 0f && NPC.ai[1] < 30f)
            {
                invulnerable = true;
                NPC.alpha = (int)(NPC.ai[1] / 30f * 255f);
            }
            else if (NPC.ai[1] >= 30f && NPC.ai[1] < 90f)
            {
                if (NPC.ai[1] == 30f && Main.netMode != NetmodeID.MultiplayerClient && encounter.IsCultist)
                    BeginRitual(player, encounter.Death, encounter.LifeRatio);

                invulnerable = true;
                NPC.alpha = 255;
                FaceRitualCenter(encounter.IsCultist);
            }
            else if (NPC.ai[1] >= 90f && NPC.ai[1] < 120f)
            {
                invulnerable = true;
                NPC.alpha = 255 - (int)((NPC.ai[1] - 90f) / 30f * 255f);
            }
            else if (NPC.ai[1] >= 120f && NPC.ai[1] < tuning.RitualTime)
            {
                NPC.alpha = 0;
                FaceRitualCenter(encounter.IsCultist);
            }
            return new(tuning.RitualTime, Retarget: true, Invulnerable: invulnerable);
        }

        private void UpdateAncientDoom(Player player, in Encounter encounter)
        {
            NPC.localAI[2] = SummoningAnimation;
            if (!(NPC.ai[1] >= 4f && encounter.IsCultist && (int)(NPC.ai[1] - 4f) % 20f == 0f && NPC.ai[1] <= 64))
                return;

            int count = 2 + (encounter.LifeRatio < 0.75f && encounter.Death ? 1 : 0) +
                (encounter.LifeRatio < 0.5f ? 1 : 0) + (encounter.LifeRatio < 0.25f && encounter.Death ? 1 : 0);
            FaceDirection(NPC, player.Center.X - NPC.Center.X);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < count; i++)
                {
                    float angle = i * (360f / count);
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.Center.X + (float)(Math.Sin(i * 120) * 550)), (int)(player.Center.Y + (float)(Math.Cos(i * 120) * 550)),
                        NPCID.AncientDoom, 0, NPC.whoAmI, 0f, angle, 0f, Main.maxPlayers);
                }
            }
        }

        /// <summary>Updates the Cultist or clone using the existing synchronized attack state.</summary>
        /// <param name="mod">The mod that owns this AI override.</param>
        /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            PlayChant();
            Encounter encounter = new(NPC.life / (float)NPC.lifeMax,
                CalamityWorld.death || BossRushEvent.BossRushActive, NPC.type == NPCID.CultistBoss);
            AttackTuning tuning = CreateTuning(encounter);
            Player player = GetTarget();
            tuning = UpdateEnrage(player, calamityGlobalNPC, tuning);
            bool dontTakeDamage = false;

            if (!encounter.IsCultist)
            {
                if (!TrySynchronizeClone(tuning.RitualTime, out dontTakeDamage))
                    return false;
            }
            else if (IsRitualVulnerable(tuning.RitualTime) && NPC.justHit)
                CancelRitual();

            if (ShouldDespawn(player))
                DespawnEncounter();

            float clonePhase = NPC.ai[3];
            InitializeEncounter();
            if (IsAttack(Attack.Introduction))
            {
                UpdateIntroduction();
                dontTakeDamage = true;
            }

            AttackUpdate update = UpdateAttack(player, encounter, tuning);
            AdvanceAttack(update);

            if (!encounter.IsCultist)
                NPC.ai[3] = clonePhase;

            NPC.dontTakeDamage = update.Invulnerable ?? dontTakeDamage;
            NPC.chaseable = !IsAttack(Attack.Introduction) && !IsAttack(Attack.Ritual);

            return false;
        }

        /// <summary>Controls Ancient Light acceleration, curved flight, and expiration.</summary>
        public class AncientLightAI : VanillaAIOverride
        {
            private const float FlightDuration = 120f;

            /// <summary>Updates an Ancient Light and its visual effects.</summary>
            /// <param name="mod">The mod that owns this AI override.</param>
            /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
            public override bool AI(Mod mod)
            {
                NPC.dontTakeDamage = true;

                if (NPC.ai[0] == -1f)
                {
                    SlowAndExpire();
                    return false;
                }

                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                InitializeFlight();
                SpawnTrailDust();
                UpdateFlight();
                return false;
            }

            private void SlowAndExpire()
            {
                if (NPC.velocity.Length() >= 0.2f)
                    NPC.velocity *= 0.96f;
                else
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.position = NPC.oldPosition;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 30f)
                    {
                        NPC.HitEffect(0, 9999.0);
                        NPC.active = false;
                    }
                }
            }

            private Dust SpawnDust() => Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height,
                DustID.AncientLight, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 2.5f)];

            private void InitializeFlight()
            {
                if (NPC.localAI[0] != 0f)
                    return;

                NPC.localAI[0] = 1f;
                NPC.velocity.X = NPC.ai[2];
                NPC.velocity.Y = NPC.ai[3];
                for (int i = 0; i < 13; i++)
                {
                    Dust dust = SpawnDust();
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.velocity *= 4f;
                    dust.noLight = true;
                }
            }

            private void SpawnTrailDust()
            {
                for (int j = 0; j < 2; j++)
                {
                    if (Main.rand.Next(10 - (int)Math.Min(7f, NPC.velocity.Length())) < 1)
                    {
                        Dust dust = SpawnDust();
                        dust.noGravity = true;
                        dust.velocity *= 0.2f;
                        dust.fadeIn = 0.4f;
                        if (Main.rand.NextBool(6))
                        {
                            dust.velocity *= 5f;
                            dust.noLight = true;
                        }
                        else
                            dust.velocity = NPC.DirectionFrom(dust.position) * dust.velocity.Length();
                    }
                }
            }

            private void UpdateFlight()
            {
                if (!(NPC.ai[0] >= 0f))
                    return;

                if (NPC.ai[0] == 0f && CalamityGlobalNPC.adultEidolonWyrmHead != -1 &&
                    Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    NPC.damage = (int)Math.Round(NPC.defDamage * PrimordialWyrmHead.LightDamageMult);

                NPC.ai[0] += 1f;
                if (NPC.ai[0] < FlightDuration - 60f && NPC.velocity.Length() < 20f)
                    NPC.velocity *= 1.03f;
                if (NPC.ai[0] >= FlightDuration - 60f)
                    NPC.velocity = NPC.velocity.RotatedBy(NPC.ai[1]);
                if (NPC.ai[0] >= FlightDuration)
                    NPC.ai[0] = -1f;
            }
        }

        /// <summary>Controls Ancient Dooms owned by the Cultist or Primordial Wyrm.</summary>
        public class AncientDoomAI : VanillaAIOverride
        {
            private const float Lifetime = 420f;
            private const float GrowthDuration = 120f;

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

            private void UpdateGrowth()
            {
                NPC.ai[1] += 6f;
                float growthRate = MathHelper.Clamp(NPC.ai[1] / GrowthDuration, 0f, 1f);
                NPC.position = NPC.Center;
                NPC.scale = MathHelper.Lerp(0f, 1f, growthRate);
                NPC.Center = NPC.position;
                NPC.alpha = (int)(255f - growthRate * 255f);
            }

            private void UpdateOrbit(NPC owner, bool isWyrm, int target)
            {
                if (NPC.ai[3] == 0f)
                    NPC.ai[3] = NPC.ai[2];

                double radians = NPC.ai[3] * (Math.PI / 180);
                double distance = 550;
                if (isWyrm)
                {
                    float aiGateValue = owner.Calamity().newAI[2] - 30f;
                    int orbitExpansion = (int)(aiGateValue / 120f);
                    distance += orbitExpansion * 45;
                }
                NPC.position.X = Main.player[target].Center.X - (int)(Math.Cos(radians) * distance) - NPC.width / 2;
                NPC.position.Y = Main.player[target].Center.Y - (int)(Math.Sin(radians) * distance) - NPC.height / 2;
                NPC.ai[3] += 8f * (1f - (NPC.ai[1] / Lifetime));
            }

            private void Burst(int target, float speed)
            {
                int damage = DoomDamage;
                // Dust callbacks may have changed the owner since the start of this tick.
                if (Main.npc[(int)NPC.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
                    damage = (int)Math.Round(damage * PrimordialWyrmHead.DoomDamageMult);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;

                int count = Main.getGoodWorld ? 5 : 3;
                Vector2 velocity = new Vector2(0f, -speed);
                float rotation = NPC.DirectionTo(Main.player[target].Center).ToRotation();
                for (int i = 0; i < count; i++)
                {
                    Vector2 projectileVelocity = velocity.RotatedBy(rotation + MathHelper.Pi * ((i + 1) / (float)(count + 1)));
                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, ProjectileID.AncientDoomProjectile, damage, 0f, Main.myPlayer)];
                    projectile.tileCollide = false;
                }
            }

            /// <summary>Updates an Ancient Doom's orbit, visual effects, and final projectile burst.</summary>
            /// <param name="mod">The mod that owns this AI override.</param>
            /// <returns><see langword="false"/> to suppress vanilla AI.</returns>
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
                NPC.damage = NPC.defDamage = 0;
                float splitProjVelocity = death ? 4.5f : 3f;

                NPC owner = Main.npc[(int)NPC.ai[0]];
                bool isWyrm = owner.type == ModContent.NPCType<PrimordialWyrmHead>();
                bool validOwner = owner.type == NPCID.CultistBoss || isWyrm;
                bool kill = NPC.ai[1] < 0f || !owner.active || !validOwner;
                int target = validOwner ? owner.target : Main.maxPlayers;
                if (isWyrm)
                    NPC.dontTakeDamage = true;

                UpdateGrowth();
                UpdateOrbit(owner, isWyrm, target);

                SpawnOrbitDust(OrbitDustStyle.Shadowflame);
                SpawnOrbitDust(OrbitDustStyle.Granite);
                SpawnOrbitDust(OrbitDustStyle.StationaryGranite);

                NPC.localAI[0] += (float)(Math.PI / 60D);

                NPC.localAI[1] = 0.25f + Vector2.UnitY.RotatedBy(NPC.ai[1] * MathHelper.TwoPi / 60f).Y * 0.25f;

                if (NPC.ai[1] >= Lifetime)
                {
                    Burst(target, splitProjVelocity);
                    kill = true;
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
