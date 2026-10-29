using CalamityMod.NPCs.ProfanedGuardians;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Providence
{
    [AutoloadBossHead]
    public class ProvSpawnHealer : ProvidenceSpawnBase
    {
        public override string Texture => "CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianHealer";

        protected override int Defense => 30;
        protected override float DamageReduction => 0.2f;
        protected override int NormalLifeMax => 15000;
        protected override int BossRushLifeMax => 20000;
        protected override int Height => 164;
        protected override string GoreNamePrefix => "H";
        protected override int DeathDustCount => 50;

        protected override void RegisterWithGlobalNPC() => CalamityGlobalNPC.holyBossHealer = NPC.whoAmI;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Texture2D texture2D16 = ProfanedGuardianHealer.Texture_Glow2.Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2));
            int afterimageAmt = 5;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ProfanedGuardianHealer.Texture_Glow.Value;
            Color yellowLerp = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            Color violetLerp = Color.Lerp(Color.White, Color.Violet, 0.5f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color yellowAfterimageColor = yellowLerp;
                    yellowAfterimageColor = Color.Lerp(yellowAfterimageColor, Color.White, 0.5f);
                    yellowAfterimageColor = NPC.GetAlpha(yellowAfterimageColor);
                    yellowAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 yellowAfterimagePos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    yellowAfterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    yellowAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, yellowAfterimagePos, NPC.frame, yellowAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

                    Color violetAfterimageColor = violetLerp;
                    violetAfterimageColor = Color.Lerp(violetAfterimageColor, Color.White, 0.5f);
                    violetAfterimageColor = NPC.GetAlpha(violetAfterimageColor);
                    violetAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    spriteBatch.Draw(texture2D16, yellowAfterimagePos, NPC.frame, violetAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            NPC.DrawBackglow(new Color(255, 64, 0, 0), 4f, spriteEffects, NPC.frame, Main.screenPosition, texture2D15);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, ProvUtils.GetColorBasedOnEnrage(false, 0), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D16, drawLocation, NPC.frame, violetLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

    }
}
