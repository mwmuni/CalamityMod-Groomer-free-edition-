using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Graphics.Metaballs
{
    internal static class AdditiveMetaballShaderHelper
    {
        internal static void Prepare()
        {
            var metaballShader = CalamityShaders.AdditiveMetaballEdgeShader;
            Vector2 screenSize = new(Main.screenWidth, Main.screenHeight);

            metaballShader.Value.Parameters["screenArea"]?.SetValue(screenSize);
            metaballShader.Value.Parameters["layerOffset"]?.SetValue(Vector2.Zero);
            metaballShader.Value.Parameters["singleFrameScreenOffset"]?.SetValue(Vector2.Zero);

            metaballShader.Value.CurrentTechnique.Passes[0].Apply();
        }
    }
}
