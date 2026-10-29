using System;
using System.Collections.Generic;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class FearlessGoldfishWarrior : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = Main.hardMode ? 100 : 30;
            NPC.width = 36;
            NPC.height = 32;
            NPC.defense = Main.hardMode ? 10 : 2;
            NPC.lifeMax = Main.hardMode ? 150 : 50;
            NPC.knockBackResist = Main.hardMode ? 0.2f : 0.5f;
            NPC.value = Item.buyPrice(silver: 1);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FearlessGoldfishWarriorBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FearlessGoldfishWarrior")
            });
        }

        public override void AI()
        {
            bool noXMovement = false;
            if (NPC.velocity.X == 0f)
            {
                noXMovement = true;
            }
            if (NPC.justHit)
            {
                noXMovement = false;
            }
            bool isWalking = false;
            bool unusedFlag = false;
            int backUpTimer = 60;
            if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
            {
                isWalking = true;
            }
            if ((NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= (float)backUpTimer) | isWalking)
            {
                NPC.ai[3] += 1f;
            }
            else if ((double)Math.Abs(NPC.velocity.X) > 0.9 && NPC.ai[3] > 0f)
            {
                NPC.ai[3] -= 1f;
            }
            if (NPC.ai[3] > (float)(backUpTimer * 10))
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.justHit)
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.ai[3] == (float)backUpTimer)
            {
                NPC.netUpdate = true;
            }
            if (NPC.ai[3] < (float)backUpTimer)
            {
                NPC.TargetClosest(true);
            }
            else if (NPC.ai[2] <= 0f)
            {
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
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 18f)
            {
                NPC.ai[3] = 0f;
                NPC.velocity.X = NPC.velocity.X * 0.9f;
                if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
                return;
            }
            float maxVelocity = CalamityWorld.death ? 3f : CalamityWorld.revenge ? 2f : 1f;
            float acceleration = CalamityWorld.death ? 0.28f : CalamityWorld.revenge ? 0.18f : 0.08f;
            maxVelocity += (1f - (float)NPC.life / (float)NPC.lifeMax) * 2f;
            acceleration += (1f - (float)NPC.life / (float)NPC.lifeMax) * 0.2f;
            if (NPC.velocity.X < -maxVelocity || NPC.velocity.X > maxVelocity)
            {
                if (NPC.velocity.Y == 0f)
                    NPC.velocity *= 0.7f;
            }
            else if (NPC.velocity.X < maxVelocity && NPC.direction == 1)
            {
                NPC.velocity.X = NPC.velocity.X + acceleration;
                if (NPC.velocity.X > maxVelocity)
                    NPC.velocity.X = maxVelocity;
            }
            else if (NPC.velocity.X > -maxVelocity && NPC.direction == -1)
            {
                NPC.velocity.X = NPC.velocity.X - acceleration;
                if (NPC.velocity.X < -maxVelocity)
                    NPC.velocity.X = -maxVelocity;
            }
            CalamityRegularEnemyAI.WalkAndHandleObstacles(NPC, backUpTimer, noXMovement, unusedFlag);
        }

        public override void FindFrame(int frameHeight)
        {
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 18f || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y = frameHeight * 5;
                if (NPC.frame.Y > frameHeight * 9)
                    NPC.frame.Y = frameHeight * 5;
            }
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                    return;
                }
                if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 4)
                        NPC.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneRain || spawnInfo.Player.ZoneDesert || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OverworldDayRain.Chance * 0.05f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            bool instakill = false;
            List<string> metarexNames = new List<string> { "LordMetarex", "Metarex" };
            foreach (string s in metarexNames)
            {
                if (s.ToLower() == target.name.ToLower())
                {
                    instakill = true;
                    break;
                }
            }

            if (instakill)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Goldfish").ToNetworkText(target.name)), 1000.0, 0, false);
                modifiers.FinalDamage *= target.statLifeMax2 * Main.rand.NextFloat(2.0f, 3.5f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.TinHelmet);
            npcLoot.AddIf(() => Main.hardMode, ItemID.MagicDagger);
        }
    }
}
