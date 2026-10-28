using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems.Graphic.LiquidSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SunkenSeaBurrowsWaterflow : ModWaterfallStyle { }

    public class SunkenSeaBurrowsWater : SunkenSeaWaterStyleBase<SunkenSeaBurrowsWater>
    {
        protected override Color GlowColor => new Color(76, 211, 231);

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1.02f;
            g = 1.03f;
            b = 1.075f;
        }

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/SunkenSeaBurrowsWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<SunkenSeaBurrowsSplash>();
        public override int GetDropletGore() => ModContent.GoreType<SunkenSeaBurrowsWaterDroplet>();
        public override Color BiomeHairColor() => Color.Blue;
    }
}
