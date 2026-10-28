using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems.Graphic.LiquidSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SunkenSeaShoresWaterflow : ModWaterfallStyle { }

    public class SunkenSeaShoresWater : SunkenSeaWaterStyleBase<SunkenSeaShoresWater>
    {
        protected override Color GlowColor => new Color(82, 223, 255);

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/SunkenSeaShoresWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<SunkenSeaShoresSplash>();
        public override int GetDropletGore() => ModContent.GoreType<SunkenSeaShoresWaterDroplet>();
        public override Color BiomeHairColor() => Color.OrangeRed;
    }
}
