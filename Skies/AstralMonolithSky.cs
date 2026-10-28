using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityMod.Skies
{
    public class AstralMonolithSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args) => skyActive = false;

        public override void Reset() => skyActive = false;

        public override bool IsActive() => skyActive || opacity > 0f;

        public override void Activate(Vector2 position, params object[] args) => skyActive = true;

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Calamity().monolithAstralShader < 1)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.LocalPlayer.Calamity().monolithAstralShader <= 0)
                return;
            float whateverTheFuckThisVariableIsSupposedToBe = 3.40282347E+38f;
            if (maxDepth >= whateverTheFuckThisVariableIsSupposedToBe && minDepth < whateverTheFuckThisVariableIsSupposedToBe)
            {
                spriteBatch.Draw(SkyTextureRefs.AstralSky.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * opacity);

                // Terraria's conditions.
                if (!Main.dedServ)
                    AstralSkyStarRenderer.Draw(spriteBatch, opacity);
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, new Color(63, 51, 90, inColor.A), opacity);
        }

        public override float GetCloudAlpha() => (1f - opacity) * 0.3f + 0.7f;
    }
}
