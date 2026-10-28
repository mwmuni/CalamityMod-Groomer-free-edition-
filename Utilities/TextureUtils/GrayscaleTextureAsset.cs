using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Threading;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    internal sealed class GrayscaleTextureAsset
    {
        private Asset<Texture2D> _asset;
        private bool _prepared;

        internal Texture2D Texture { get; private set; }

        internal bool IsAssetLoaded => _asset?.IsLoaded ?? false;

        internal bool IsPrepared => _prepared;

        internal Texture2D Request(string assetName, IDeferredLoadTexture owner)
        {
            _asset = ModContent.Request<Texture2D>(assetName);
            if (_asset is null)
                return null;

            Texture = _asset.Value;
            DeferredTextureLoadingManager.Enqueue(owner);
            return Texture;
        }

        internal void OnTextureLoaded(Action<Texture2D> prepare)
        {
            if (_prepared)
                return;

            Texture = _asset.Value;
            if (Texture is null)
                return;

            prepare(Texture);
            _prepared = true;
        }

        internal void Unload()
        {
            _asset = null;
            Texture = null;
            _prepared = false;
        }

        internal static void CopyRedChannel(Texture2D texture, float[] scales)
        {
            Color[] colorScheme = new Color[scales.Length];
            texture.GetData(colorScheme);
            FastParallel.For(0, scales.Length, (startInclusive, endExclusive, context) =>
            {
                for (int i = startInclusive; i < endExclusive; i++)
                    scales[i] = colorScheme[i].R / 255.0f;
            });
        }
    }
}
