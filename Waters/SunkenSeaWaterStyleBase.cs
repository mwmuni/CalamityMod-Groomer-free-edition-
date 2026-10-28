using CalamityMod.Systems.Graphic.LiquidSystem;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    // TSelf is the concrete style, so Type and Instance are static per style rather than shared across the family.
    public abstract class SunkenSeaWaterStyleBase<TSelf> : ModWaterStyle, IWaterStyleModifyLight where TSelf : SunkenSeaWaterStyleBase<TSelf>
    {
        public static int Type { get; private set; }
        public static TSelf Instance { get; private set; }

        protected abstract Color GlowColor { get; }

        public override void SetStaticDefaults()
        {
            Type = Slot;
            Instance = (TSelf)this;
        }

        public override void Unload()
        {
            Type = -1;
            Instance = null;
        }

        public void ModifyLight(in Tile tile, int i, int j, ref float r, ref float g, ref float b)
        {
            Vector3 outputColor = new Vector3(r, g, b);

            if (tile.TileType != RustyChestTile.TileType)
            {
                WaterStyleCommon.ModifySunkenSeaWaterLight(i, j, GlowColor.ToVector3(), ref outputColor.X, ref outputColor.Y, ref outputColor.Z);
            }

            r = outputColor.X;
            g = outputColor.Y;
            b = outputColor.Z;
        }
    }
}
