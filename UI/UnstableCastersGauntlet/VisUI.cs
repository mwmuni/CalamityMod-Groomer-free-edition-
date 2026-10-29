using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonsArsenal
{
    [Autoload(Side = ModSide.Client)]
    public sealed class VisUI : ModSystem
    {
        // Base pulled from Arsenal charge UI
        // Defaults to same pos as arsenal charge meter since there isn't a state where both would coexist
        internal const float DefaultChargePosX = 50.104603f;
        internal const float DefaultChargePosY = 58.05169f;
        private static Vector2? dragOffset = null;
        private static Texture2D edgeTexture, barTexture;

        public override void OnModLoad()
        {
            edgeTexture = ModContent.Request<Texture2D>("CalamityMod/UI/UnstableCastersGauntlet/VisMeterBorder", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/UnstableCastersGauntlet/VisMeter", AssetRequestMode.ImmediateLoad).Value;
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
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = MeterUIHelper.GetScreenPosition(
                new Vector2(CalamityClientConfig.Instance.ChargeMeterPosX, CalamityClientConfig.Instance.ChargeMeterPosY),
                new Vector2(DefaultChargePosX, DefaultChargePosY),
                out Vector2 screenPos);

            float uiScale = Main.UIScale;

            void SavePosition()
            {
                bool changed = false;
                if (CalamityClientConfig.Instance.ChargeMeterPosX != screenRatioPosition.X)
                {
                    CalamityClientConfig.Instance.ChargeMeterPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityClientConfig.Instance.ChargeMeterPosY != screenRatioPosition.Y)
                {
                    CalamityClientConfig.Instance.ChargeMeterPosY = screenRatioPosition.Y;
                    changed = true;
                }
                if (changed)
                    CalamityClientConfig.Instance.SaveChanges();
            }

            // If the Charge Meter is turned off or the player is not holding an item, stop.
            Item heldItem = player.HeldItem;
            if (!CalamityClientConfig.Instance.ChargeMeter || heldItem is null || heldItem.IsAir)
            {
                Reset();
                SavePosition();
                return;
            }

            // If the player is not holding the correct item, stop.
            if (heldItem.type != ModContent.ItemType<UnstableCastersGauntlet>())
            {
                Reset();
                SavePosition();
                return;
            }

            CalamityPlayer modPlayer = player.Calamity();
            DrawChargeBar(spriteBatch, modPlayer.unstableCastersGauntletVis / 100f, screenPos);


            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle chargeBar = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale * 0.8f);
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            if (chargeBar.Intersects(mouseHitbox))
            {
                if (!CalamityClientConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // If the mouse is on top of the meter, show the exact numeric charge.
                string percentString = (player.Calamity().unstableCastersGauntletVis).ToString("n2");
                Main.instance.MouseText($"{CalamityUtils.GetTextValue("UI.Vis")}: {percentString}%", 0, 0, -1, -1, -1, -1);

                MeterUIHelper.HandleDragging(screenRatioPosition, screenPos, ms, mousePos, ref dragOffset, static newPosition =>
                {
                    CalamityClientConfig.Instance.ChargeMeterPosX = newPosition.X;
                    CalamityClientConfig.Instance.ChargeMeterPosY = newPosition.Y;
                });
            }
        }

        private static void DrawChargeBar(SpriteBatch spriteBatch, float chargeRatio, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(edgeTexture, screenPos, null, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale * 0.8f, SpriteEffects.None, 0);

            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * chargeRatio), barTexture.Width);
            spriteBatch.Draw(barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale * 0.85f, SpriteEffects.None, 0);
        }
    }
}
