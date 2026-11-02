using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(AstrumDeusHead))]
    public class AstrumDeusTail : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.AstrumDeusHead.DisplayName");

        public static Asset<Texture2D> GlowTexture;
        public static Asset<Texture2D> GlowTexture2;
        public static Asset<Texture2D> TextureFlash;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[Type] = 1;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                GlowTexture2 = ModContent.Request<Texture2D>(Texture + "Glow2", AssetRequestMode.AsyncLoad);
                TextureFlash = ModContent.Request<Texture2D>(Texture + "GlowFlash", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.damage = 60; // 120
            NPC.npcSlots = 5f;
            NPC.width = 52;
            NPC.height = 68;
            NPC.defense = 50;
            NPC.DR_NERD(0.4f);
            NPC.LifeMaxNERB(200000, 240000, 650000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.4f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.35f;
            else if (Main.expertMode)
                NPC.scale *= 1.2f;

            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = AstrumDeusHead.HitSound;
            NPC.DeathSound = AstrumDeusHead.DeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.dontCountMe = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            AstrumDeusBody.UpdateSegmentAI(NPC, false);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool deathModeEnragePhase = Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] == 3f;
            bool doubleWormPhase = NPC.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            float cyanThreshold = Main.getGoodWorld ? 300f : 600f;
            // Tail is always the first segment to visually transition
            float transitionStart = cyanThreshold * 0.75f;
            float transitionEnd = cyanThreshold * 0.8f;
            bool drawCyan = NPC.Calamity().newAI[3] >= transitionEnd && NPC.Calamity().newAI[3] <= cyanThreshold + transitionEnd;
            bool inColorTrans = doubleWormPhase && NPC.Calamity().newAI[3] % cyanThreshold >= transitionStart && NPC.Calamity().newAI[3] % cyanThreshold <= transitionEnd;

            Texture2D wormTexture = TextureAssets.Npc[Type].Value;
            Texture2D otherTexture;
            Vector2 halfSizeTex = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(wormTexture.Width, wormTexture.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTex * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(wormTexture, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);

            wormTexture = GlowTexture.Value;
            Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
            Color otherPhaseColor = drawCyan ? Color.Orange : Color.Cyan;
            if (doubleWormPhase) // otherTexture contains the opposite texture, and is faded in during the transition
            {
                wormTexture = drawCyan ? GlowTexture2.Value : wormTexture;
                otherTexture = drawCyan ? wormTexture : GlowTexture2.Value;
            }
            else
                otherTexture = wormTexture;

            Color wormColorLerp = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);

            int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
            for (int i = 0; i < timesToDraw; i++)
            {
                float opacity = Utils.GetLerpValue(transitionStart, transitionEnd, NPC.Calamity().newAI[3] % cyanThreshold, true);
                spriteBatch.Draw(wormTexture, drawLocation, NPC.frame, wormColorLerp * (inColorTrans ? 1f - opacity : 1f), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Controls drawing the new fading in glowmask for the upcoming behavior
                if (inColorTrans)
                    spriteBatch.Draw(otherTexture, drawLocation, NPC.frame, Color.Lerp(Color.White, otherPhaseColor, 0.5f) * opacity, NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Controls drawing the white flash immediately after swapping behaviors
                if (doubleWormPhase && NPC.Calamity().newAI[3] % cyanThreshold < 25f)
                    spriteBatch.Draw(TextureFlash.Value, drawLocation, NPC.frame, Color.White * MathHelper.Lerp(1f, 0f, NPC.Calamity().newAI[3] % cyanThreshold / 25f), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
            }
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !NPC.dontTakeDamage;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && !Main.zenithWorld) // I value people's computers
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 5; i++)
                {
                    int purpleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[purpleDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[purpleDust].scale = 0.5f;
                        Main.dust[purpleDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[astralDust].noGravity = true;
                    Main.dust[astralDust].velocity *= 5f;
                    astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[astralDust].velocity *= 2f;
                }
                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("AstrumDeusTail1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("AstrumDeusTail2").Type, 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
