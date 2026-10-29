using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.StormWeaver
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(StormWeaverHead))]
    public class StormWeaverBody : StormWeaverSegment
    {
        public static Asset<Texture2D> Phase2Texture;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.StormWeaverHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[Type] = 1;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "Naked", AssetRequestMode.AsyncLoad);
            }
        }

        public static int LaserDamage = 54; // 216

        public override void SetDefaults()
        {
            NPC.damage = 90; // 180
            NPC.npcSlots = 5f;
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 825000;
            NPC.LifeMaxNERB(NPC.lifeMax, NPC.lifeMax, 500000);

            // Phase one settings
            CalamityGlobalNPC global = NPC.Calamity();
            NPC.defense = 150;
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = StormWeaverHead.DeathSound;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.15f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.1f;
            else if (Main.expertMode)
                NPC.scale *= 1.05f;

            if (Main.getGoodWorld)
                NPC.scale *= 0.7f;

            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Shed armor
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            // Target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Update armored settings to naked settings
            if (phase2 && (!Main.zenithWorld || !revenge))
            {
                // Spawn armor gore and set other crucial variables
                if (!NPC.chaseable)
                {
                    NPC.Calamity().VulnerableToHeat = true;
                    NPC.Calamity().VulnerableToCold = true;
                    NPC.Calamity().VulnerableToSickness = true;

                    if (!Main.dedServ)
                    {
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorBody1").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorBody2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorBody3").Type, NPC.scale);
                    }

                    CalamityGlobalNPC global = NPC.Calamity();
                    NPC.defense = 30;
                    global.DR = 0.3f;
                    global.unbreakableDR = false;
                    NPC.chaseable = true;
                    NPC.HitSound = SoundID.NPCHit13;
                    NPC.frame = new Rectangle(0, 0, 54, 52);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Fire a barrage of lasers every 5 seconds
                    float laserBarrageGateValue = 300f;
                    if (Main.npc[(int)NPC.ai[2]].localAI[0] % laserBarrageGateValue == 0f)
                    {
                        float bodySegmentDivisor = death ? 6 : revenge ? 5 : expertMode ? 4 : 3;
                        if (NPC.ai[0] % bodySegmentDivisor == 0f)
                        {
                            NPC.TargetClosest();
                            float projectileVelocity = death ? 6.25f : revenge ? 5.75f : expertMode ? 5.5f : 5f;
                            Vector2 velocityVector = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                            int type = ModContent.ProjectileType<DestroyerElectricLaser>();
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocityVector, type, LaserDamage, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 900;
                        }
                    }
                }
            }

            UpdateSegmentFollowing();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool phase2 = lifeRatio < 0.8f && (!Main.zenithWorld || !revenge);
            bool phase3 = lifeRatio < 0.55f;

            // Gate value that decides when Storm Weaver will charge
            float chargePhaseGateValue = death ? 320f : revenge ? 340f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            Texture2D texture = phase2 ? Phase2Texture.Value : TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2(texture.Width / 2, texture.Height / 2);
            float chargeTelegraphTime = 120f;
            float chargeTelegraphGateValue = chargePhaseGateValue - chargeTelegraphTime;

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color drawColorAlpha = NPC.GetAlpha(drawColor);

            if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] > chargeTelegraphGateValue)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp((Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] - chargeTelegraphGateValue) / chargeTelegraphTime, 0f, 1f));
            else if (Main.npc[(int)NPC.ai[2]].localAI[3] > 0f)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp(Main.npc[(int)NPC.ai[2]].localAI[3] / 60f, 0f, 1f));

            spriteBatch.Draw(texture, drawLocation, NPC.frame, drawColorAlpha, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeBody1").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeBody2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeBody3").Type, NPC.scale);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(50 * NPC.scale);
                NPC.height = (int)(50 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 20; i++)
                {
                    int cosmiliteDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[cosmiliteDust].scale = 0.5f;
                        Main.dust[cosmiliteDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 40; j++)
                {
                    int cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmiliteDust2].noGravity = true;
                    Main.dust[cosmiliteDust2].velocity *= 5f;
                    cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust2].velocity *= 2f;
                }
            }
        }

    }
}
