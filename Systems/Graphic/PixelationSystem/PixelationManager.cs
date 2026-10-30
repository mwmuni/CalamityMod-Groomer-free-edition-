using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Enums;
using CalamityMod.Graphics;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems.Graphic.PixelationSystem
{
    public class PixelationManager : ModSystem
    {
        private static readonly GeneralDrawLayer[] DrawLayerOrder =
        [
            GeneralDrawLayer.BeforeAllTiles,
            GeneralDrawLayer.BeforeSolidTiles,
            GeneralDrawLayer.BeforeNPCs,
            GeneralDrawLayer.AfterNPCs,
            GeneralDrawLayer.BeforeProjectiles,
            GeneralDrawLayer.AfterProjectiles,
            GeneralDrawLayer.AfterPlayers,
            GeneralDrawLayer.AfterDusts,
            GeneralDrawLayer.AfterEverything,
        ];

        private static PixelationLayerState[] PixelationLayers;

        private sealed class PixelationLayerState
        {
            public readonly Dictionary<BlendState, RenderTargetLease> Targets = [];
            public readonly List<PixelatedDrawer> Drawers = [];
        }

        /// <summary>
        /// The resolution ratio at which pixelated drawers should draw.<br/>
        /// <c>0.5f</c> will draw at half resolution, <c>0.25f</c> will draw at quarter resolution, etc.
        /// </summary>
        internal const float PixelationResolution = 0.5f;

        /// <summary>
        /// The transformation matrix used by <see cref="PixelationManager"/> to draw pixelated assets.
        /// </summary>
        internal static Matrix PixelationMatrix
        {
            get
            {
                return Main.GameViewMatrix.TransformationMatrix
                   * Matrix.CreateScale(PixelationResolution / Main.GameViewMatrix.Zoom.X, PixelationResolution / Main.GameViewMatrix.Zoom.Y, 1f)
                   * Matrix.CreateTranslation(Main.GameViewMatrix.Translation.X * PixelationResolution, Main.GameViewMatrix.Translation.Y * PixelationResolution, 0f);
            }
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            PixelationLayers = DrawLayerOrder.Select(_ => new PixelationLayerState()).ToArray();

            GeneralDrawLayerSystem.OnDrawLayerLate += DrawPixelatedTargets;
            GeneralDrawLayerSystem.OnPrepareDraw += PrepareTargets;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            PixelationLayers = null;
        }

        public override void OnWorldUnload()
        {
            if (Main.dedServ)
                return;

            foreach (PixelationLayerState layer in PixelationLayers)
                layer.Targets.Clear();

            foreach (PixelationLayerState layer in PixelationLayers)
                layer.Drawers.Clear();
        }


        /// <summary>
        /// Queues a <see cref="PixelatedDrawer"/> instance to draw pixelated assets.
        /// </summary>
        /// <param name="drawAction">All relevant drawing code should be written here.</param>
        /// <param name="drawLayer">The layer at which you'd like these assets to be drawn on. <br><b>e.g.</b> <see cref="GeneralDrawLayer.BeforeProjectiles"/> will make all drawn assets render behind all projectiles.</br></param>
        /// <param name="defaultBlendState">The default <see cref="BlendState"/> that your pixelated assets will be rendered in.
        /// <br>Leave as null to render them using <see cref="BlendState.AlphaBlend"/>.</br></param>
        public static void AddPixelatedDrawer(Action<Matrix> drawAction, GeneralDrawLayer drawLayer, BlendState defaultBlendState = null)
        {
            if (Main.dedServ || Main.gameMenu)
                return;

            BlendState blendState = defaultBlendState ?? BlendState.AlphaBlend;

            // Check if the associated render target for this drawer exists and create it if necessary.
            VerifyTargetExistence(drawLayer, blendState);

            PixelatedDrawer drawer = new(drawAction, drawLayer, blendState);
            ReturnAssociatedLayerState(drawLayer).Drawers.Add(drawer);
        }

        private static void PrepareTargets()
        {
            if (!Main.gameMenu && !Main.dedServ)
            {
                foreach (PixelationLayerState layer in PixelationLayers)
                    DrawCollectionsToTarget(layer.Targets, PixelationMatrix, layer.Drawers);
            }
        }

        private static void DrawCollectionsToTarget(Dictionary<BlendState, RenderTargetLease> targetCollection, Matrix pixelationMatrix, List<PixelatedDrawer> drawerCollection)
        {
            foreach (var blendStateTargetPair in targetCollection)
            {
                using (blendStateTargetPair.Value.Scope(clearColor: Color.Transparent))
                {
                    Main.spriteBatch.Begin(default, blendStateTargetPair.Key, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, pixelationMatrix);

                    // Select only drawers who match this target's BlendState.
                    var drawersByBlendState = drawerCollection.Where(d => d.DefaultBlendState == blendStateTargetPair.Key).ToList();
                    if (drawersByBlendState.Count > 0)
                    {
                        foreach (PixelatedDrawer drawer in drawersByBlendState)
                            drawer.DrawAction.Invoke(pixelationMatrix);
                    }

                    Main.spriteBatch.End();
                }
            }

            drawerCollection.Clear();
        }

        private static void DrawPixelatedTargets(GeneralDrawLayer drawLayer)
        {
            var targetCollection = ReturnAssociatedLayerState(drawLayer).Targets;
            foreach (var keyValuePair in targetCollection)
            {
                Main.spriteBatch.Begin(default, keyValuePair.Key, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float targetScale = 1f / PixelationResolution;
                Main.spriteBatch.Draw(keyValuePair.Value.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, targetScale, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
            }
        }

        private static int FindLayerIndex(GeneralDrawLayer drawLayer)
        {
            return Array.IndexOf(DrawLayerOrder, drawLayer);
        }

        private static PixelationLayerState ReturnAssociatedLayerState(GeneralDrawLayer drawLayer)
        {
            int layerIndex = FindLayerIndex(drawLayer);
            return PixelationLayers[layerIndex < 0 ? DrawLayerOrder.Length - 1 : layerIndex];
        }

        private static void VerifyTargetExistence(GeneralDrawLayer drawLayer, BlendState blendState)
        {
            int layerIndex = FindLayerIndex(drawLayer);
            if (layerIndex < 0)
                return;

            Dictionary<BlendState, RenderTargetLease> targets = PixelationLayers[layerIndex].Targets;
            if (!targets.ContainsKey(blendState))
                Main.QueueMainThreadAction(() => PixelationLayers[layerIndex].Targets[blendState] = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice, (w, h) => ((int)(w * PixelationResolution), (int)(h * PixelationResolution))));
        }
    }
}
