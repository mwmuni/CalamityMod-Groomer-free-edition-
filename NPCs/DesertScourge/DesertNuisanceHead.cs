using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class DesertNuisanceHead : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public bool flies = false;
        private bool tailSpawned = false;

        public const float SegmentVelocity_Expert = 11f;
        public const float SegmentVelocity_Death = 13.5f;
        public const float SegmentVelocity_GoodWorld = 19f;
        public const float SegmentVelocity_ZenithSeed = 22f;

        public const float OpenMouthForBiteDistance = 220f;


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.7f,
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            value.Position.X += 0;
            value.Position.Y -= 20;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 25; // 40 (1.6x expert scaling)

            NPC.defense = 3;
            if (Main.getGoodWorld)
                NPC.defense += 19;

            NPC.width = 88;
            NPC.height = 88;

            NPC.LifeMaxNERB(1500, 1800, 40000);
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
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
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
            UpdateNuisanceAI<DesertNuisanceHead, DesertNuisanceBody, DesertNuisanceTail, DesertNuisanceHeadYoung>(NPC, ref biomeEnrageTimer, ref tailSpawned, false);
        }

        internal static void UpdateNuisanceAI<THead, TBody, TTail, TOtherHead>(NPC NPC, ref int biomeEnrageTimer, ref bool tailSpawned, bool young)
            where THead : ModNPC
            where TBody : ModNPC
            where TTail : ModNPC
            where TOtherHead : ModNPC
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Become angry when the other Nuisance dies.
            bool getMad = (!NPC.AnyNPCs(ModContent.NPCType<TOtherHead>()) && revenge) || death;

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

            float speed = young ? (death ? 0.09f : 0.07f) : (death ? 0.105f : 0.085f);
            float turnSpeed = young ? (death ? 0.18f : 0.14f) : (death ? 0.21f : 0.17f);
            speed += speed * 0.4f * (1f - lifeRatio);
            turnSpeed += turnSpeed * 0.4f * (1f - lifeRatio);
            speed += (young ? 0.07f : 0.085f) * enrageScale;
            turnSpeed += (young ? 0.14f : 0.17f) * enrageScale;

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

            if (young)
                DesertNuisanceHeadYoung.UpdateSpitAttack(NPC, revenge, death, getMad);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && NPC.ai[0] == 0f)
                {
                    int minLength = 8;
                    if (Main.getGoodWorld)
                        minLength *= 2;

                    DesertScourgeHead.SpawnSegments<TBody, TTail>(NPC, minLength);
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
                    // Retain each head's player traversal rather than changing enumerators.
                    if (young)
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
                    }
                    else
                    {
                        foreach (Player plr in Main.ActivePlayers)
                        {
                            Rectangle rectangle2 = new Rectangle((int)plr.position.X - directChaseDistance, (int)plr.position.Y - directChaseDistance, directChaseDistance * 2, directChaseDistance * 2);
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

            float maxChaseSpeed = young
                ? (Main.zenithWorld ? DesertNuisanceHeadYoung.SegmentVelocity_ZenithSeed :
                    Main.getGoodWorld ? DesertNuisanceHeadYoung.SegmentVelocity_GoodWorld :
                    death ? DesertNuisanceHeadYoung.SegmentVelocity_Death :
                    DesertNuisanceHeadYoung.SegmentVelocity_Expert)
                : (Main.zenithWorld ? SegmentVelocity_ZenithSeed :
                    Main.getGoodWorld ? SegmentVelocity_GoodWorld :
                    death ? SegmentVelocity_Death :
                    SegmentVelocity_Expert);
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
                        if (Main.npc[a].type == ModContent.NPCType<THead>() || Main.npc[a].type == ModContent.NPCType<TBody>() || Main.npc[a].type == ModContent.NPCType<TTail>())
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
                if (young)
                    NPC.velocity.Y += 0.1f;
                else
                {
                    NPC.velocity.Y += (death ? 0.125f : 0.1f);
                    if (NPC.velocity.Y > 0f && Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > 180f)
                        NPC.velocity.Y += 0.05f;
                }

                BaseWormNPC.UpdateFallingVelocity(NPC, maxChaseSpeed, speed, playerX);
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

            return minDist <= 50f * NPC.scale;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceHead").Type, NPC.scale);

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // Open mouth to prepare for a nibble ;3
            bool openMouth = NPC.Distance(Main.player[NPC.target].Center) < OpenMouthForBiteDistance &&
                (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation() &&
                NPC.ai[3] == 0f;

            DesertScourgeHead.UpdateMouthFrame(NPC, Type, frameHeight, openMouth);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                NPC.Opacity = 1f;
                // Reimplementation of CalamityUtils.DrawAnimatedBestiaryWorm but tweaked due to this entity' animations
                NPC.frame = texture.Frame();
                // Buffers the segment position and rotations
                float offset = -0.2f;
                float startX = 60;
                float startY = 70;
                int segmentSpacing = 50;
                int animationSpeed = 8;
                float wormTimer = NPC.Calamity().bestiaryWormTimer;
                int segCount = 3;
                // Draw the body segments
                for (int i = segCount; i > 0; i--)
                {
                    // The first segment is slightly closer to keep up with the head
                    float bodyOffset = i * segmentSpacing - segmentSpacing * 0.5f;

                    Texture2D toUse = i == 1 ? TextureAssets.Npc[ModContent.NPCType<DesertNuisanceBody>()].Value : DesertNuisanceBody.BodyTexture2.Value;
                    int frameCount = i == 1 ? 7 : 1;
                    spriteBatch.Draw(toUse, NPC.position + new Vector2(startX + bodyOffset, MathF.Sin((wormTimer + offset * i) * animationSpeed) * 2 + startY), toUse.Frame(1, frameCount, 0, 0), NPC.GetAlpha(drawColor), NPC.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer + offset * i) * animationSpeed) * MathHelper.PiOver4 * 0.075f, new Vector2(toUse.Width / 2, toUse.Height / (frameCount * 2)), NPC.scale, SpriteEffects.None, 0f);
                }
                // Draw the head
                spriteBatch.Draw(texture, NPC.position + new Vector2(startX + 18, MathF.Sin(wormTimer * animationSpeed) * 2 + startY), texture.Frame(1, 7, 0, 0), NPC.GetAlpha(drawColor), NPC.rotation - MathHelper.PiOver2 - MathF.Cos(wormTimer * animationSpeed) * MathHelper.PiOver4 * 0.075f, new Vector2(texture.Width * 0.5f, texture.Height / 7), NPC.scale, SpriteEffects.None, 0f);

                return false;
            }
            return true;
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
