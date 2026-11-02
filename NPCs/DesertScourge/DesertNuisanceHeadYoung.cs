using System;
using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class DesertNuisanceHeadYoung : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public bool flies = false;
        private bool tailSpawned = false;

        public const float SegmentVelocity_Expert = 10f;
        public const float SegmentVelocity_Death = 12f;
        public const float SegmentVelocity_GoodWorld = 16f;
        public const float SegmentVelocity_ZenithSeed = 18f;

        public const float SpitGateValue = 180f;

        public const float OpenMouthForBiteDistance = 220f;


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 25; // 40 (1.6x expert scaling)

            NPC.defense = 2;
            if (Main.getGoodWorld)
                NPC.defense += 18;

            NPC.width = 78;
            NPC.height = 78;

            NPC.LifeMaxNERB(1300, 1560, 35000);
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;

            if (Main.zenithWorld)
                NPC.scale *= 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<DesertScourgeHead>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DesertNuisance")
            });
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Become angry when the other Nuisance dies.
            bool getMad = (!NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) && revenge) || death;

            // Enrage
            if (!Main.player[NPC.target].ZoneDesert && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0;

            float enrageScale = getMad ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 2f;
            }

            // Percent life remaining.
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = death ? 0.09f : 0.07f;
            float turnSpeed = death ? 0.18f : 0.14f;
            speed += speed * 0.4f * (1f - lifeRatio);
            turnSpeed += turnSpeed * 0.4f * (1f - lifeRatio);
            speed += 0.07f * enrageScale;
            turnSpeed += 0.14f * enrageScale;

            if (Main.getGoodWorld)
            {
                speed *= 1.1f;
                turnSpeed *= 1.2f;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (NPC.Distance(Main.player[NPC.target].Center) > 360f)
            {
                if ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                    NPC.Calamity().newAI[0] += 1f;

                if (NPC.Calamity().newAI[0] >= SpitGateValue)
                {
                    NPC.Calamity().newAI[0] = 0f;
                    SoundEngine.PlaySound(SoundID.NPCDeath11, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * (revenge ? 10f : 8f);
                        int numProj = death ? 6 : getMad ? 4 : 3;
                        int spread = death ? 28 : getMad ? 22 : 20;

                        float rotation = MathHelper.ToRadians(spread);
                        int type = ModContent.ProjectileType<DesertScourgeSpit>();
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));

                            for (int k = 0; k < 10; k++)
                            {
                                int dust = Dust.NewDust(NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, 10, 10, (int)CalamityDusts.SulphurousSeaAcid);
                                Main.dust[dust].velocity = perturbedSpeed;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 5f, perturbedSpeed, type, DesertScourgeHead.SpitDamage, 0f, Main.myPlayer);
                                Main.projectile[proj].aiStyle = -1;
                                Main.projectile[proj].netUpdate = true;
                            }
                        }
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && NPC.ai[0] == 0f)
                {
                    int previous = NPC.whoAmI;
                    int minLength = 8;
                    if (Main.getGoodWorld)
                        minLength *= 2;

                    int bodyTypeAIVariable = 0;
                    for (int i = 0; i < minLength + 1; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength)
                        {
                            if (i == 0)
                                bodyTypeAIVariable = 0;
                            else if (i == minLength - 1)
                                bodyTypeAIVariable = 30;
                            else if (i % 2 == 0)
                                bodyTypeAIVariable = 20;
                            else
                                bodyTypeAIVariable = 10;

                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertNuisanceBodyYoung>(), NPC.whoAmI);
                            Main.npc[lol].ai[3] = bodyTypeAIVariable;
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertNuisanceTailYoung>(), NPC.whoAmI);

                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[1] = previous;
                        Main.npc[previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        previous = lol;
                    }

                    tailSpawned = true;
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            bool shouldFly = false;
            if (!shouldFly)
            {
                for (int k = tilePositionX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePositionY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int directChaseDistance = death ? 400 : revenge ? 500 : 1000;
                bool shouldDirectlyChase = true;
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    for (int m = 0; m < Main.maxPlayers; m++)
                    {
                        if (Main.player[m].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - directChaseDistance, (int)Main.player[m].position.Y - directChaseDistance, directChaseDistance * 2, directChaseDistance * 2);
                            if (rectangle.Intersects(rectangle2))
                            {
                                shouldDirectlyChase = false;
                                break;
                            }
                        }
                    }

                    if (shouldDirectlyChase)
                        shouldFly = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = 1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = -1;

            float maxChaseSpeed = Main.zenithWorld ? SegmentVelocity_ZenithSeed :
                Main.getGoodWorld ? SegmentVelocity_GoodWorld :
                death ? SegmentVelocity_Death :
                SegmentVelocity_Expert;
            maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);
            if (death)
                maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);

            if (Main.player[NPC.target].dead)
            {
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16D)
                {
                    NPC.velocity.Y += 1f;
                    maxChaseSpeed *= 2f;
                }

                if ((double)NPC.position.Y > Main.rockLayer * 16D)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertNuisanceHeadYoung>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceBodyYoung>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceTailYoung>())
                            Main.npc[a].active = false;
                    }
                }
            }

            Vector2 npcCenter = NPC.Center;
            float playerX = Main.player[NPC.target].Center.X;
            float targettingPosition = Main.player[NPC.target].Center.Y;
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));

            if (!shouldFly)
            {
                NPC.velocity.Y += 0.1f;
                DesertScourgeHead.UpdateFallingVelocity(NPC, maxChaseSpeed, speed, playerX);
            }
            else
            {
                DesertScourgeHead.UpdateDiggingVelocity(NPC, targetDistance, playerX, targettingPosition, maxChaseSpeed, speed, turnSpeed);
            }

            Vector2 destination = Main.player[NPC.target].Center;
            if (NPC.Distance(destination) > (getMad ? 750f : 1000f))
                NPC.velocity += (destination - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + MathHelper.PiOver2;

            if (shouldFly)
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
                NPC.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 45f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisance2Head").Type, NPC.scale);

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // Open mouth to prepare for a nibble ;3
            // Also open mouth for projectile spreads
            bool aboutToSpitSpread = NPC.Calamity().newAI[0] > SpitGateValue - 30f;
            bool openMouth = NPC.Distance(Main.player[NPC.target].Center) < OpenMouthForBiteDistance &&
                (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation() &&
                NPC.ai[3] == 0f;

            DesertScourgeHead.UpdateMouthFrame(NPC, Type, frameHeight, openMouth || aboutToSpitSpread);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                NPC.ai[3] = 1f;
                NPC.ForceNetUpdate();
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
