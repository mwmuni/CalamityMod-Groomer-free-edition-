using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI.SulphurousWaterMeter
{
    public class SulphurousWaterMeterUI : ModSystem
    {
        // These values were decided relative to the stealth meter positions, and are by default slightly below them.
        internal const float DefaultPosX = StealthUI.DefaultStealthPosX;
        internal const float DefaultPosY = StealthUI.DefaultStealthPosY + 2.286282f;
        private static Vector2? dragOffset = null;
        private static Texture2D edgeTexture, barTexture;

        public override void OnModLoad()
        {
            edgeTexture = ModContent.Request<Texture2D>("CalamityMod/UI/SulphurousWaterMeter/SulphWaterMeter", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/SulphurousWaterMeter/SulphWaterBar", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        public override void Unload()
        {
            Reset();
            edgeTexture = barTexture = null;
        }

        private static void Reset() => dragOffset = null;

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing. This is done relative.
            Vector2 screenRatioPosition = MeterUIHelper.GetScreenPosition(
                new Vector2(CalamityClientConfig.Instance.SulphuricWaterMeterPosX, CalamityClientConfig.Instance.SulphuricWaterMeterPosY),
                new Vector2(DefaultPosX, DefaultPosY),
                out Vector2 screenPos);

            float uiScale = Main.UIScale;

            CalamityPlayer modPlayer = player.Calamity();

            // If not drawing the water meter, save its latest position to config and leave.
            if (modPlayer.SulphWaterUIOpacity > 0f && (modPlayer.ZoneSulphur || player.Calamity().ZoneAbyssLayer1))
                DrawWaterBar(spriteBatch, modPlayer, screenPos);
            else
            {
                bool changed = false;
                if (CalamityClientConfig.Instance.SulphuricWaterMeterPosX != screenRatioPosition.X)
                {
                    CalamityClientConfig.Instance.SulphuricWaterMeterPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityClientConfig.Instance.SulphuricWaterMeterPosY != screenRatioPosition.Y)
                {
                    CalamityClientConfig.Instance.SulphuricWaterMeterPosY = screenRatioPosition.Y;
                    changed = true;
                }

                if (changed)
                    CalamityClientConfig.Instance.SaveChanges();
            }

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle waterBarArea = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Handle mouse dragging
            if (waterBarArea.Intersects(mouseHitbox))
            {
                if (!CalamityClientConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // If the mouse is on top of the meter, show the player's accumulated sulphuric poisoning.
                if (modPlayer.SulphWaterPoisoningLevel > 0f && modPlayer.SulphWaterUIOpacity >= 0f)
                {
                    string poisonText = (modPlayer.SulphWaterPoisoningLevel * 100f).ToString("n2");
                    string textToDisplay = $"{CalamityUtils.GetTextValue("UI.SulphurousWater")}: {poisonText}/100";
                    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
                    modPlayer.SulphWaterUIOpacity = MathHelper.Lerp(modPlayer.SulphWaterUIOpacity, 0.25f, 0.035f);
                }

                MeterUIHelper.HandleDragging(screenRatioPosition, screenPos, ms, mousePos, ref dragOffset, static newPosition =>
                {
                    CalamityClientConfig.Instance.SulphuricWaterMeterPosX = newPosition.X;
                    CalamityClientConfig.Instance.SulphuricWaterMeterPosY = newPosition.Y;
                });
            }
            else
                modPlayer.SulphWaterUIOpacity = MathHelper.Clamp(modPlayer.SulphWaterUIOpacity + (modPlayer.SulphWaterPoisoningLevel > 0f).ToDirectionInt() * 0.06f, 0f, 1f);
        }

        private static void DrawWaterBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(edgeTexture, screenPos, null, Color.White * modPlayer.SulphWaterUIOpacity, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            float completionRatio = MathHelper.Clamp(modPlayer.SulphWaterPoisoningLevel, 0f, 1f);
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            spriteBatch.Draw(barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White * modPlayer.SulphWaterUIOpacity, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
        }
    }
}
