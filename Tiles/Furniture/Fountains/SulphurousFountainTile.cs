using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Tiles.BaseTiles;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class SulphurousFountainTile : BaseFountainTile<SulphurousFountainItem>
    {
        protected override Color MapColor => new Color(141, 121, 77);

        protected override void ApplyFountainEffect(int i, int j) =>
            Main.SceneMetrics.ActiveFountainColor = Main.zenithWorld ? PissWater.Instance.Slot : SulphuricWater.Instance.Slot;
    }
}
