using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CalamityMod.Items.Accessories
{
    internal static class AccessoryInventoryDrawing
    {
        internal static void DrawCustomScale(
            int itemType,
            SpriteBatch spriteBatch,
            Vector2 position,
            Rectangle frame,
            Color drawColor,
            Color itemColor,
            Vector2 origin,
            float scale,
            float wantedScale,
            Vector2 drawOffset = default)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[itemType].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: wantedScale,
                drawOffset: drawOffset
            );
        }
    }
}
