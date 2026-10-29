using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
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
    public class StormWeaverTail : StormWeaverSegment
    {
        private int invinceTime = 180;

        public static Asset<Texture2D> Phase2Texture;
        public static Asset<Texture2D> GlowTexture;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.StormWeaverHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[Type] = 1;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "Naked", AssetRequestMode.AsyncLoad);
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int LightningOrbDamage = 64; // 256

        public override void SetDefaults()
        {
            NPC.damage = 80; // 160
            NPC.npcSlots = 5f;
            NPC.width = 48;
            NPC.height = 80;
            NPC.lifeMax = 825000;
            NPC.LifeMaxNERB(NPC.lifeMax, NPC.lifeMax, 500000);

            // Phase one settings
            NPC.takenDamageMultiplier = 2f;
            NPC.HitSound = SoundID.NPCHit53;
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
            writer.Write(invinceTime);
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (invinceTime > 0)
            {
                invinceTime--;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            // Shed armor
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            // Update armored settings to naked settings
            if (phase2 && (!Main.zenithWorld || !CalamityWorld.revenge))
            {
                // Spawn armor gore and set other crucial variables
                if (NPC.takenDamageMultiplier == 2f)
                {
                    NPC.Calamity().VulnerableToHeat = true;
                    NPC.Calamity().VulnerableToCold = true;
                    NPC.Calamity().VulnerableToSickness = true;

                    if (!Main.dedServ)
                    {
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorTail1").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorTail2").Type, NPC.scale);
                    }

                    CalamityGlobalNPC global = NPC.Calamity();
                    NPC.defense = 40;
                    global.DR = 0.4f;
                    NPC.takenDamageMultiplier = 1f;
                    NPC.HitSound = SoundID.NPCHit13;
                    NPC.frame = new Rectangle(0, 0, 42, 68);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Fire a lightning orb every 5 seconds
                    float spawnOrbGateValue = 300f;
                    if (Main.npc[(int)NPC.ai[2]].localAI[0] % spawnOrbGateValue == 0f)
                    {
                        int type = ProjectileID.CultistBossLightningOrb;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, LightningOrbDamage, 0f, Main.myPlayer);
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

            if (!phase2)
            {
                texture = GlowTexture.Value;
                Color rainbowBecauseWhyTheFuckNot = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                Color drawColorAlpha37 = Color.Lerp(Color.White, rainbowBecauseWhyTheFuckNot, 0.5f);
                spriteBatch.Draw(texture, drawLocation, NPC.frame, drawColorAlpha37, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeTail1").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeTail2").Type, NPC.scale);
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
