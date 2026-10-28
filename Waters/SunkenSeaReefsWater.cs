using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems.Graphic.LiquidSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SunkenSeaReefsWaterflow : ModWaterfallStyle { }

    public class SunkenSeaReefsWater : SunkenSeaWaterStyleBase<SunkenSeaReefsWater>
    {
        protected override Color GlowColor => new Color(140, 222, 240);

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/SunkenSeaReefsWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<SunkenSeaReefsSplash>();
        public override int GetDropletGore() => ModContent.GoreType<SunkenSeaReefsWaterDroplet>();
        public override Color BiomeHairColor() => Color.SkyBlue;
    }
}
