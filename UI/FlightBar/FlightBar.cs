using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public sealed class FlightBar : ModSystem
    {
        // These values were handpicked on a 1080p screen by Amber. Please disregard the bizarre precision.
        internal const float DefaultFlightPosX = 40.9375f;
        internal const float DefaultFlightPosY = 7.2222223f; //yes really, that many twos were handpicked, how? your guess is as good as mine lmao
        private const int FlightAnimFrameDelay = 5;
        private const int FlightAnimFrames = 17;

        private static int FlightAnimFrame = -1;
        private static int FlightAnimTimer = 0;

        private static Vector2? dragOffset = null;
        private static Texture2D borderTexture, flightBarAnimTexture, barTexture, disabledBarTexture, infiniteBarTexture, limitedBarTexture;
        private static bool completedAnimation = false;

        private static Texture2D GetApplicableBorder(CalamityPlayer modPlayer)
        {
            if (modPlayer.Player.equippedWings != null && modPlayer.Player.wingTimeMax == 0 && modPlayer.Player.mount.Active && modPlayer.Player.mount._data.flightTimeMax == 0)
                return disabledBarTexture;
            if ((modPlayer.infiniteFlight || RidingInfiniteFlightMount(modPlayer.Player)) && completedAnimation)
                return infiniteBarTexture;
            if (modPlayer.weakPetrification || modPlayer.icarusFolly || modPlayer.DoGExtremeGravity)
                return limitedBarTexture;
            return borderTexture;
        }

        private static object GetFlightTime(CalamityPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            object result;
            if (player.equippedWings != null && player.wingTimeMax == 0 && !(player.mount.Active && player.mount._data.flightTimeMax > 0))
                result = 0;
            if ((modPlayer.infiniteFlight || RidingInfiniteFlightMount(modPlayer.Player)) && completedAnimation)
            {
                result = "∞"; //infinite flight
            }
            else
            {
                bool ridingLimitedFlightMount = player.mount.Active && player.mount._data.flightTimeMax > 0;
                bool ridingCarpet = player.carpet && !player.canCarpet;

                int currentFlight = ridingCarpet ? player.carpetTime : ridingLimitedFlightMount ? player.mount._flyTime + (int)(player.mount._data.fatigueMax - player.mount._fatigue) : (int)player.wingTime;
                int maxFlight = ridingCarpet ? 300 : ridingLimitedFlightMount ? player.mount._data.flightTimeMax + (int)player.mount._data.fatigueMax : player.wingTimeMax;
                return (Math.Min(100f * currentFlight / maxFlight, 100f)).ToString("0.00"); // why the FUCK can wingtime be higher than max wingtime?????????
            }

            return result;
        }

        public override void OnModLoad()
        {
            borderTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorder", AssetRequestMode.ImmediateLoad).Value;
            flightBarAnimTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarAnim", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBar", AssetRequestMode.ImmediateLoad).Value;
            disabledBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderDisabled", AssetRequestMode.ImmediateLoad).Value;
            infiniteBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderInfinite", AssetRequestMode.ImmediateLoad).Value;
            limitedBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderReduced", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        public override void Unload()
        {
            Reset();
            borderTexture = flightBarAnimTexture = barTexture = disabledBarTexture = infiniteBarTexture = limitedBarTexture = null;
            FlightAnimFrame = -1;
            FlightAnimTimer = 0;
            completedAnimation = false;
        }

        private static void Reset() => dragOffset = null;



        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = MeterUIHelper.GetScreenPosition(
                new Vector2(CalamityClientConfig.Instance.FlightBarPosX, CalamityClientConfig.Instance.FlightBarPosY),
                new Vector2(DefaultFlightPosX, DefaultFlightPosY),
                out Vector2 screenPos);

            float uiScale = Main.UIScale;

            CalamityPlayer modPlayer = player.Calamity();

            // If not drawing the flight bar, save its latest position to config and leave.
            if (CalamityClientConfig.Instance.FlightBar && ((player.wingsLogic > 0 && player.wingTimeMax > 0) || (player.mount.Active && player.mount._data.flightTimeMax > 0) || player.carpet && !player.canCarpet))
            {
                DrawFlightBar(spriteBatch, modPlayer, screenPos);
            }
            else
            {
                bool changed = false;
                if (CalamityClientConfig.Instance.FlightBarPosX != screenRatioPosition.X)
                {
                    CalamityClientConfig.Instance.FlightBarPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityClientConfig.Instance.FlightBarPosY != screenRatioPosition.Y)
                {
                    CalamityClientConfig.Instance.FlightBarPosY = screenRatioPosition.Y;
                    changed = true;
                }

                if (changed)
                    CalamityClientConfig.Instance.SaveChanges();
                return;
            }

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle flightBar = Utils.CenteredRectangle(screenPos, borderTexture.Size() * uiScale);

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Handle mouse dragging
            if (flightBar.Intersects(mouseHitbox))
            {
                if (!CalamityClientConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                if (modPlayer.Player.equippedWings != null && modPlayer.Player.wingTimeMax > 0 || (player.mount.Active && modPlayer.Player.mount._data.flightTimeMax > 0) || player.carpet && !player.canCarpet) //equipped wings or riding a flying mount and max wingtime/flighttime above 0 (so not disabled bar)
                {
                    string textToDisplay = CalamityUtils.GetText("UI.Flight").Format((GetFlightTime(modPlayer).ToString() + (modPlayer.infiniteFlight ? "" : "%"))); //the percent is here and not in localisation otherwise it looks like a dick when it's infinite flight
                    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
                }

                MeterUIHelper.HandleDragging(screenRatioPosition, screenPos, ms, mousePos, ref dragOffset, static newPosition =>
                {
                    CalamityClientConfig.Instance.FlightBarPosX = newPosition.X;
                    CalamityClientConfig.Instance.FlightBarPosY = newPosition.Y;
                });
            }
        }

        /**
        private const int FlightAnimFrameDelay = 5;
        private const int FlightAnimFrames = 17;
        
        private static int FlightAnimFrame = -1;
        private static int FlightAnimTimer = 0;
         */
        private static void DrawFlightBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            Player player = modPlayer.Player;
            float flightRatio = 1;
            if (!modPlayer.infiniteFlight && !RidingInfiniteFlightMount(player))
                flightRatio = player.carpet && !player.canCarpet ? Math.Min((float)player.carpetTime / 300f, 1f) : player.mount.Active && player.mount._data.flightTimeMax > 0 ? Math.Min((float)(player.mount._flyTime + (player.mount._data.fatigueMax - player.mount._fatigue)) / (player.mount._data.flightTimeMax + player.mount._data.fatigueMax), 1f) : Math.Min(player.wingTime / player.wingTimeMax, 1f); // why the FUCK can wingtime be higher than max wingtime?????????
            if (!completedAnimation && FlightAnimFrame == -1 && (modPlayer.infiniteFlight || RidingInfiniteFlightMount(modPlayer.Player)))
                FlightAnimFrame++;
            if (FlightAnimFrame > -1) //animation started, complete it.
            {
                FlightAnimTimer++;
                if (FlightAnimTimer >= FlightAnimFrameDelay)
                {
                    if (FlightAnimFrame >= FlightAnimFrames - 1) // Must be # of frames - 1 or else there is 1 frame of normal border before it cuts to infinite border
                    {
                        FlightAnimFrame = -1;
                        FlightAnimTimer = 0;
                        completedAnimation = modPlayer.infiniteFlight || RidingInfiniteFlightMount(modPlayer.Player); //completed animation sets to true if infinite flight still exists
                    }
                    else
                    {
                        FlightAnimTimer = 0;
                        FlightAnimFrame++;
                    }
                }
            }
            Texture2D correctBorder = GetApplicableBorder(modPlayer); //Fetch texture after animation calculations in case update to infinite flight

            if (completedAnimation && !modPlayer.infiniteFlight && correctBorder != infiniteBarTexture)
                completedAnimation = false; //reset flight anim once infinite flight expires.


            float offset = (correctBorder.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(correctBorder, screenPos, null, Color.White, 0f, correctBorder.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            if (correctBorder != disabledBarTexture && correctBorder != infiniteBarTexture) //neither requires an internal bar to be drawn
            {
                int correctHeight = (correctBorder == limitedBarTexture ? barTexture.Height / 2 : barTexture.Height);
                Rectangle barRectangle = barTexture.Bounds;
                barRectangle.Height = (int)(correctHeight * flightRatio);
                Vector2 origin = correctBorder.Size() * 0.5f;
                origin.Y += 0.1f;
                Vector2 drawPos = screenPos - new Vector2(offset * uiScale, 12 * uiScale);
                spriteBatch.Draw(barTexture, drawPos, barRectangle, Color.White, MathHelper.ToRadians(180), origin, uiScale, SpriteEffects.None, 0);
            }
            if (!completedAnimation && FlightAnimFrame >= 0)
            {
                Vector2 origin = new Vector2(correctBorder.Width * 0.5f, (correctBorder.Height / FlightAnimFrames) * 0.5f);
                float xOffset = (correctBorder.Width - flightBarAnimTexture.Width) / 2f;
                int frameHeight = (flightBarAnimTexture.Height / FlightAnimFrames) - 1;
                Vector2 sizeDiffOffset = new Vector2(xOffset, -37f) * uiScale;
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * FlightAnimFrame, flightBarAnimTexture.Width, frameHeight);
                spriteBatch.Draw(flightBarAnimTexture, screenPos + sizeDiffOffset, animCropRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);
            }
        }

        private static bool RidingInfiniteFlightMount(Player player)
        {
            if (player.mount.Active && (player.mount._data.fatigueMax >= int.MaxValue - 1 || infiniteFlightMounts.Contains(player.mount.Type)))
                return true;
            return false;
        }

        public static List<int> infiniteFlightMounts = new List<int>
        {
            MountID.UFO, MountID.Drill, MountID.PirateShip, MountID.WitchBroom, MountID.CuteFishron
        };
    }
}
