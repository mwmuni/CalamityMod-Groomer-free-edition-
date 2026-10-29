using CalamityMod.NPCs.Providence;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    internal static class ProfanedGuardianDrawHelper
    {
        internal static void DrawGuardianInstance(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor,
            Vector2 drawOffset,
            Color? colorOverride,
            int afterimageAmount,
            Texture2D dayGlowTexture,
            Texture2D nightGlowTexture,
            Texture2D additionalGlowTexture = null)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[npc.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture.Width / 2, texture.Height / Main.npcFrameCount[npc.type] / 2);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmount; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = npc.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmount - i) / 15f;
                    if (colorOverride != null)
                        afterimageColor = colorOverride.Value;

                    Vector2 afterimagePos = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    afterimagePos += halfSizeTexture * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
                    spriteBatch.Draw(texture, afterimagePos, npc.frame, afterimageColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = npc.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            drawLocation += halfSizeTexture * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
            spriteBatch.Draw(texture, drawLocation, npc.frame, colorOverride ?? npc.GetAlpha(drawColor), npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);

            Texture2D glowTexture = Main.zenithWorld ? nightGlowTexture : dayGlowTexture;
            Color glowColor = Main.zenithWorld ? Main.DiscoColor : Color.Lerp(Color.White, Color.Yellow, 0.5f);
            if (colorOverride != null)
                glowColor = colorOverride.Value;

            Color additionalGlowColor = Color.Lerp(Color.White, Color.Violet, 0.5f);
            if (colorOverride != null)
                additionalGlowColor = colorOverride.Value;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmount; j++)
                {
                    Color afterimageGlowColor = glowColor;
                    afterimageGlowColor = Color.Lerp(afterimageGlowColor, Color.White, 0.5f);
                    afterimageGlowColor = npc.GetAlpha(afterimageGlowColor);
                    afterimageGlowColor *= (afterimageAmount - j) / 15f;
                    if (colorOverride != null)
                        afterimageGlowColor = colorOverride.Value;

                    Vector2 afterimageGlowPos = npc.oldPos[j] + new Vector2(npc.width, npc.height) / 2f - screenPos;
                    afterimageGlowPos -= new Vector2(glowTexture.Width, glowTexture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    afterimageGlowPos += halfSizeTexture * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
                    spriteBatch.Draw(glowTexture, afterimageGlowPos, npc.frame, afterimageGlowColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);

                    if (additionalGlowTexture != null)
                    {
                        Color additionalGlowAfterimageColor = additionalGlowColor;
                        additionalGlowAfterimageColor = Color.Lerp(additionalGlowAfterimageColor, Color.White, 0.5f);
                        additionalGlowAfterimageColor = npc.GetAlpha(additionalGlowAfterimageColor);
                        additionalGlowAfterimageColor *= (afterimageAmount - j) / 15f;
                        if (colorOverride != null)
                            additionalGlowAfterimageColor = colorOverride.Value;

                        spriteBatch.Draw(additionalGlowTexture, afterimageGlowPos, npc.frame, additionalGlowAfterimageColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
                    }
                }
            }

            npc.DrawBackglow(Main.zenithWorld ? Main.DiscoColor : new Color(255, 64, 0, 0), 4f, spriteEffects, npc.frame, Main.screenPosition, glowTexture);
            spriteBatch.Draw(glowTexture, drawLocation, npc.frame, ProvUtils.GetColorBasedOnEnrage(false, 0), npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);

            if (additionalGlowTexture != null)
                spriteBatch.Draw(additionalGlowTexture, drawLocation, npc.frame, additionalGlowColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
        }
    }
}
