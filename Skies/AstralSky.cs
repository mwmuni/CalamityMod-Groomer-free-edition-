using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityMod.Skies
{
    public class AstralSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args)
        {
            skyActive = (Main.LocalPlayer.Calamity().ZoneAstral && !Main.LocalPlayer.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950) || Main.LocalPlayer.Calamity().monolithAstralShader > 0;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, new Color(63, 51, 90, inColor.A), opacity);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            // Small worlds, default draw height.
            int AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 2;

            // Medium worlds.
            if (Main.maxTilesX >= 6400 && Main.maxTilesX < 8400)
                AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 4;

            // Large worlds (and anything bigger).
            if (Main.maxTilesX >= 8400)
                AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 140;

            float whateverTheFuckThisVariableIsSupposedToBe = 3.40282347E+38f;
            if (maxDepth >= whateverTheFuckThisVariableIsSupposedToBe && minDepth < whateverTheFuckThisVariableIsSupposedToBe)
            {
                spriteBatch.Draw(SkyTextureRefs.AstralSky.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * opacity);

                // Terraria's conditions.
                if (!Main.dedServ)
                    AstralSkyStarRenderer.Draw(spriteBatch, opacity);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if ((!Main.LocalPlayer.Calamity().ZoneAstral && Main.LocalPlayer.Calamity().monolithAstralShader <= 0) || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
