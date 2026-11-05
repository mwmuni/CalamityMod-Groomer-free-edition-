using System.Collections.Generic;
using CalamityMod.Enums;
using CalamityMod.Systems.Graphic;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Renderers
{
    public class RendererManager : ModSystem
    {
        #region Fields/Properties
        public static List<BaseRenderer> Renderers
        {
            get;
            private set;
        } = new();
        #endregion

        #region Loading
        public override void Load()
        {
            if (Main.dedServ)
                return;

            // This hooks here, because doing it any sooner causes the screen position to be a frame behind.
            GeneralDrawLayerSystem.OnDrawLayer += DrawRendererAtLayer;
            GeneralDrawLayerSystem.OnPrepareDraw += DrawToTargets;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            // Clear the cached renderers, so they can be readded on mod loading when initialized.
            Renderers.Clear();
        }
        #endregion

        #region Updating
        public override void PreUpdateEntities()
        {
            if (Main.dedServ)
                return;

            foreach (var renderer in Renderers)
                renderer.PreUpdate();
        }

        public override void PostUpdateEverything()
        {
            if (Main.dedServ)
                return;

            foreach (var renderer in Renderers)
                renderer.PostUpdate();
        }
        #endregion

        #region Drawing
        private static void DrawRendererAtLayer(GeneralDrawLayer drawLayer)
        {
            bool begunDrawing = false;
            foreach (var renderer in Renderers)
            {
                if (!renderer.ShouldDraw || renderer.Layer != drawLayer)
                    continue;

                if (!begunDrawing)
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                    begunDrawing = true;
                }

                renderer.DrawTarget(Main.spriteBatch);
            }

            if (begunDrawing)
                Main.spriteBatch.End();
        }

        private void DrawToTargets()
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            // Reset drawn frame check
            foreach (var player in Main.ActivePlayers)
            {
                player.Calamity().drawnAnyShieldThisFrame = false;
            }

            foreach (var renderer in Renderers)
            {
                if (!renderer.ShouldDraw)
                    continue;

                using (Main.spriteBatch.Scope())
                using (renderer.MainTarget.Scope(clearColor: Color.Transparent))
                {
                    Main.spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        Main.Rasterizer,
                        null,
                        Main.GameViewMatrix.TransformationMatrix
                    );
                    renderer.DrawToTarget(Main.spriteBatch);
                    Main.spriteBatch.End();
                }
            }
        }
        #endregion
    }
}
