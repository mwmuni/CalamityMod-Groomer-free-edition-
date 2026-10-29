using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Tiles.BaseTiles;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class AstralFountainTile : BaseFountainTile<AstralFountainItem>
    {
        protected override Color MapColor => new Color(59, 50, 77);
        protected override int FirstDustType => ModContent.DustType<AstralBlue>();
        protected override int SecondDustType => ModContent.DustType<AstralOrange>();
        protected override Color? FirstDustColor => null;
        protected override Color? SecondDustColor => null;

        protected override void ApplyFountainEffect(int i, int j) =>
            Main.SceneMetrics.ActiveFountainColor = AstralWater.Instance.Slot;
    }
}
