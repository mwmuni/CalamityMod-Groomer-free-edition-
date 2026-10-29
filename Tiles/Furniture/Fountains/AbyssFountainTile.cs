using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Tiles.BaseTiles;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class AbyssFountainTile : BaseFountainTile<AbyssFountainItem>
    {
        protected override Color MapColor => new Color(71, 93, 107);

        protected override void ApplyFountainEffect(int i, int j) =>
            Main.SceneMetrics.ActiveFountainColor = VoidWater.Instance.Slot;
    }
}
