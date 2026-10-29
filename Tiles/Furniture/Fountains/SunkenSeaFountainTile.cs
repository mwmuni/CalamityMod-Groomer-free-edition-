using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Tiles.BaseTiles;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class SunkenSeaFountainTile : BaseFountainTile<SunkenSeaFountain>
    {
        protected override Color MapColor => new Color(104, 195, 255);

        protected override void ApplyFountainEffect(int i, int j) =>
            Main.SceneMetrics.ActiveFountainColor = BasaltGullyWater.Type;
    }
}
