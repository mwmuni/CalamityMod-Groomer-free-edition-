using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems.Graphic.LiquidSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SunkenSeaPolypWaterflow : ModWaterfallStyle { }

    public class SunkenSeaPolypWater : SunkenSeaWaterStyleBase<SunkenSeaPolypWater>
    {
        protected override Color GlowColor => new Color(213, 185, 178);

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/SunkenSeaPolypWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<SunkenSeaPolypSplash>();
        public override int GetDropletGore() => ModContent.GoreType<SunkenSeaPolypWaterDroplet>();
        public override Color BiomeHairColor() => Color.Coral;
    }
}
