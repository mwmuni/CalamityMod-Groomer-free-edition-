using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    /// <summary>
    /// Utility Class for using 1D Grayscale Texture as gradient
    /// </summary>
    public sealed class GrayscaleTexture1D : IDeferredLoadTexture
    {
        public Texture2D Texture { get; private set; }

        private int _Width = 0;
        private float[] _Scales;

        private readonly GrayscaleTextureAsset _asset = new();

        public bool IsAssetLoaded => _asset.IsAssetLoaded;

        public GrayscaleTexture1D(string assetName)
        {
            if (Main.dedServ)
                return;

            Texture = _asset.Request(assetName, this);
            if (Texture is null)
                return;
        }

        public void Unload()
        {
            _Width = 0;
            _Scales = null;
            _asset.Unload();
            Texture = null;
        }

        public void OnTextureLoaded()
        {
            _asset.OnTextureLoaded(texture =>
            {
                Texture = texture;
                _Width = Texture.Width;
                _Scales = new float[Texture.Width];
                GrayscaleTextureAsset.CopyRedChannel(Texture, _Scales);
            });
        }

        public float GetClamp(int x)
        {
            if (!_asset.IsPrepared)
                return default;

            if (_Width == 0)
                return default;

            x = Math.Clamp(x, 0, _Width - 1);
            return _Scales[x];
        }

        public float GetRepeat(int x)
        {
            if (!_asset.IsPrepared)
                return default;

            if (_Width == 0)
                return default;

            x %= _Width;
            return _Scales[x];
        }
    }
}
