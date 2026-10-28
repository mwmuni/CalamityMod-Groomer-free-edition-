using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralSnowSurfaceBGStyle : AstralSurfaceBackgroundStyleBase
    {
        private static readonly BackgroundLayer[] backgroundLayers =
        {
            new BackgroundLayer(
                static () => SkyTextureRefs.AstralSnowSurfaceMiddle.Value,
                yOffset: 475,
                scale: 1.25f,
                parallax: 0.4,
                topPositionMultiplier: 1800.0,
                topPositionOffset: 1500.0,
                drawDepth: 1.2f,
                menuTopY: 320),
            new BackgroundLayer(
                static () => SkyTextureRefs.AstralSurfaceClose.Value,
                static () => SkyTextureRefs.AstralSurfaceCloseGlow.Value,
                0.7f,
                yOffset: 175,
                scale: 1.31f,
                parallax: 0.43,
                topPositionMultiplier: 1950.0,
                topPositionOffset: 1750.0,
                drawDepth: 1f,
                menuTopY: 400,
                menuOffsetX: 80),
            new BackgroundLayer(
                static () => SkyTextureRefs.AstralSurfaceFront.Value,
                static () => SkyTextureRefs.AstralSurfaceFrontGlow.Value,
                0.9f,
                yOffset: 275,
                scale: 1.34f,
                parallax: 0.49,
                topPositionMultiplier: 2100.0,
                topPositionOffset: 2000.0,
                drawDepth: 1f,
                menuTopY: 480,
                menuOffsetX: 120)
        };

        private protected override BackgroundLayer[] BackgroundLayers => backgroundLayers;

        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceHorizon");

        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceFar");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Skies/AstralSnowSurfaceMiddle");
    }
}
