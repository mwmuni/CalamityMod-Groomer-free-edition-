using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralDesertSurfaceBGStyle : AstralSurfaceBackgroundStyleBase
    {
        private static readonly BackgroundLayer[] backgroundLayers =
        {
            new BackgroundLayer(
                static () => SkyTextureRefs.AstralDesertSurfaceMiddle.Value,
                yOffset: 475,
                scale: 1.25f,
                parallax: 0.4,
                topPositionMultiplier: 1800.0,
                topPositionOffset: 1500.0,
                drawDepth: 1.2f,
                menuTopY: 320),
            new BackgroundLayer(
                static () => SkyTextureRefs.AstralDesertSurfaceClose.Value,
                yOffset: 475,
                scale: 1.31f,
                parallax: 0.43,
                topPositionMultiplier: 1950.0,
                topPositionOffset: 1750.0,
                drawDepth: 1f,
                menuTopY: 400,
                menuOffsetX: 80)
        };

        private protected override BackgroundLayer[] BackgroundLayers => backgroundLayers;

        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceHorizon");

        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralDesertSurfaceFar");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Skies/AstralDesertSurfaceMiddle");
    }
}
