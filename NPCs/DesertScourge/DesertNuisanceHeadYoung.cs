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
            DesertNuisanceHead.UpdateNuisanceAI<DesertNuisanceHeadYoung, DesertNuisanceBodyYoung, DesertNuisanceTailYoung, DesertNuisanceHead>(NPC, ref biomeEnrageTimer, ref tailSpawned, true);
        }

        internal static void UpdateSpitAttack(NPC NPC, bool revenge, bool death, bool getMad)
        {
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
