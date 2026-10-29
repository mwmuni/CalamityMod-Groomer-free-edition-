using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace CalamityMod.UI
{
    internal static class MeterUIHelper
    {
        private const float MouseDragEpsilon = 0.05f; // 0.05%

        internal static Vector2 GetScreenPosition(Vector2 screenRatioPosition, Vector2 defaultPosition, out Vector2 screenPosition)
        {
            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = defaultPosition.X;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = defaultPosition.Y;

            // Cast to integer to prevent blurriness from decimal pixel positions.
            screenPosition = screenRatioPosition;
            screenPosition.X = (int)(screenPosition.X * 0.01f * Main.screenWidth);
            screenPosition.Y = (int)(screenPosition.Y * 0.01f * Main.screenHeight);
            return screenRatioPosition;
        }

        internal static void HandleDragging(Vector2 screenRatioPosition, Vector2 screenPosition, MouseState mouseState, Vector2 mousePosition,
            ref Vector2? dragOffset, Action<Vector2> updatePosition)
        {
            Vector2 newScreenRatioPosition = screenRatioPosition;
            if (!CalamityClientConfig.Instance.MeterPosLock && mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!dragOffset.HasValue)
                    dragOffset = mousePosition - screenPosition;

                Vector2 newCorner = mousePosition - dragOffset.GetValueOrDefault(Vector2.Zero);
                newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
            }

            Vector2 delta = newScreenRatioPosition - screenRatioPosition;
            if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                updatePosition(newScreenRatioPosition);

            if (dragOffset.HasValue && mouseState.LeftButton == ButtonState.Released)
            {
                dragOffset = null;
                CalamityClientConfig.Instance.SaveChanges();
            }
        }
    }
}
