using System;
using System.IO;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PrimordialWyrm
{
    public abstract class PrimordialWyrmBodyBase : ModNPC
    {
        protected abstract int BodyWidth { get; }
        protected abstract Asset<Texture2D> Lightmask { get; }
        protected abstract string DeathGoreName { get; }

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PrimordialWyrmHead.DisplayName");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = BodyWidth;
            NPC.height = 88;
            NPC.defense = 0;
            NPC.LifeMaxNERB(2500000, 3000000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            // Difficulty modes
            bool death = CalamityWorld.death;
            bool revenge = CalamityWorld.revenge;
            bool expertMode = Main.expertMode;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive. If not, die.
            int wyrmHeadID = ModContent.NPCType<PrimordialWyrmHead>();
            bool shouldDespawn = !NPC.AnyNPCs(wyrmHeadID);
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)NPC.ai[2]].Calamity();

            float chargePhaseGateValue = death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
            float lightningChargePhaseGateValue = death ? 120f : revenge ? 135f : expertMode ? 150f : 180f;

            bool invisiblePartOfChargePhase = calamityGlobalNPC_Head.newAI[2] >= chargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= chargePhaseGateValue + 1f && (calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.ChargeOne || calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.ChargeTwo || calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.FastCharge);
            bool invisiblePartOfLightningChargePhase = calamityGlobalNPC_Head.newAI[2] >= lightningChargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= lightningChargePhaseGateValue + 1f && calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.LightningCharge;
            bool invisiblePhase = calamityGlobalNPC_Head.newAI[0] == 1f || calamityGlobalNPC_Head.newAI[0] == 5f || calamityGlobalNPC_Head.newAI[0] == 7f;
            if (!invisiblePartOfChargePhase && !invisiblePartOfLightningChargePhase && !invisiblePhase)
            {
                if (Main.npc[(int)NPC.ai[1]].Opacity > 0.5f)
                {
                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity > 1f)
                        NPC.Opacity = 1f;
                }
            }
            else
            {
                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            DoVariantAttack(calamityGlobalNPC_Head, death, revenge, expertMode);

            // Decide segment offset stuff.
            NPC aheadSegment = Main.npc[(int)NPC.ai[1]];
            Vector2 directionToNextSegment = aheadSegment.Center - NPC.Center;
            if (aheadSegment.rotation != NPC.rotation)
            {
                directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(aheadSegment.rotation - NPC.rotation) * 0.08f);
                directionToNextSegment = directionToNextSegment.MoveTowards((aheadSegment.rotation - NPC.rotation).ToRotationVector2(), 1f);
            }

            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * NPC.width;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();
        }

        protected abstract void DoVariantAttack(CalamityGlobalNPC calamityGlobalNPC_Head, bool death, bool revenge, bool expertMode);

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2);

            Vector2 center = NPC.Center - screenPos;
            center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            center += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, center, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            // this math is so incredibly scuffed. please someone fix it i dont even know what any of this actually does
            float brightness = 1f;
            float nameWasTooLong = Main.GameUpdateCount * 0.01f;
            float saneVelocity = MathHelper.Clamp((int)PrimordialWyrmHead.PWHeadVelocity, 6f, 8f);
            brightness = MathF.Sin(nameWasTooLong * (6f + saneVelocity) - NPC.whoAmI);
            brightness = MathHelper.Clamp(brightness, 0.25f, 1f);
            texture = Lightmask.Value;
            spriteBatch.Draw(texture, center, NPC.frame, Color.White * (NPC.Opacity * brightness), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, hit.HitDirection, -1f, 0, default, 1f);

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>(DeathGoreName).Type, 1f);
                }
            }
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.Jared");
            }
        }
    }
}
