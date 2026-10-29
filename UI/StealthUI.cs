using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public sealed class StealthUI : ModSystem
    {
        // These values were handpicked on a 1080p screen by Ozzatron. Please disregard the bizarre precision.
        internal const float DefaultStealthPosX = 50.104603f;
        internal const float DefaultStealthPosY = 55.765408f;
        private static Vector2? dragOffset = null;
        private static Texture2D edgeTexture, indicatorTexture, barTexture, fullBarTexture;

        public override void OnModLoad()
        {
            edgeTexture = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/StealthMeter", AssetRequestMode.ImmediateLoad).Value;
            indicatorTexture = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/StealthMeterStrikeIndicator", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/StealthMeterBar", AssetRequestMode.ImmediateLoad).Value;
            fullBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/StealthMeterBarFull", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        public override void Unload()
        {
            Reset();
            edgeTexture = indicatorTexture = barTexture = fullBarTexture = null;
        }

        private static void Reset() => dragOffset = null;

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = MeterUIHelper.GetScreenPosition(
                new Vector2(CalamityClientConfig.Instance.StealthMeterPosX, CalamityClientConfig.Instance.StealthMeterPosY),
                new Vector2(DefaultStealthPosX, DefaultStealthPosY),
                out Vector2 screenPos);

            float uiScale = Main.UIScale;

            CalamityPlayer modPlayer = player.Calamity();

            // If not drawing the stealth meter, save its latest position to config and leave.
            if (modPlayer.stealthUIAlpha > 0f && CalamityClientConfig.Instance.StealthMeter && modPlayer.rogueStealthMax > 0f && modPlayer.wearingRogueArmor)
            {
                DrawStealthBar(spriteBatch, modPlayer, screenPos);
            }
            else
            {
                bool changed = false;
                if (CalamityClientConfig.Instance.StealthMeterPosX != screenRatioPosition.X)
                {
                    CalamityClientConfig.Instance.StealthMeterPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityClientConfig.Instance.StealthMeterPosY != screenRatioPosition.Y)
                {
                    CalamityClientConfig.Instance.StealthMeterPosY = screenRatioPosition.Y;
                    changed = true;
                }

                if (changed)
                    CalamityClientConfig.Instance.SaveChanges();
            }

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle stealthBar = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Handle mouse dragging
            if (stealthBar.Intersects(mouseHitbox))
            {
                if (!CalamityClientConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // If the mouse is on top of the meter, show the player's exact numeric stealth.
                if (modPlayer.rogueStealthMax > 0f && modPlayer.stealthUIAlpha >= 0.5f)
                {
                    string stealthStr = (100f * modPlayer.rogueStealth).ToString("n2");
                    string maxStealthStr = (100f * modPlayer.rogueStealthMax).ToString("n2");
                    string textToDisplay = $"{CalamityUtils.GetTextValue("UI.Stealth")}: {stealthStr}/{maxStealthStr}\n";

                    if (!Main.keyState.PressingShift())
                    {
                        textToDisplay += CalamityUtils.GetTextValue("UI.StealthShiftText");
                    }
                    else
                    {
                        textToDisplay += CalamityUtils.GetTextValue("UI.StealthInfoText");
                    }

                    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
                    modPlayer.stealthUIAlpha = MathHelper.Lerp(modPlayer.stealthUIAlpha, 0.25f, 0.035f);
                }

                MeterUIHelper.HandleDragging(screenRatioPosition, screenPos, ms, mousePos, ref dragOffset, static newPosition =>
                {
                    CalamityClientConfig.Instance.StealthMeterPosX = newPosition.X;
                    CalamityClientConfig.Instance.StealthMeterPosY = newPosition.Y;
                });
            }
        }

        private static void DrawStealthBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(edgeTexture, screenPos, null, Color.White * modPlayer.stealthUIAlpha, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // If SS is available, display the explicit indication thereof
            if (modPlayer.StealthStrikeAvailable())
                spriteBatch.Draw(indicatorTexture, screenPos, null, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            float completionRatio = modPlayer.rogueStealthMax <= 0f ? 0f : modPlayer.rogueStealth / modPlayer.rogueStealthMax;
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            bool full = (modPlayer.rogueStealthMax > 0f) && (modPlayer.rogueStealth >= modPlayer.rogueStealthMax);
            spriteBatch.Draw(full ? fullBarTexture : barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
        }
    }
}
