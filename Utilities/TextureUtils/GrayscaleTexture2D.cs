using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    /// <summary>
    /// Utility Class for using 2D Grayscale Texture as gradient
    /// </summary>
    public sealed class GrayscaleTexture2D : IDeferredLoadTexture
    {
        public Texture2D Texture { get; private set; }

        private int _Width = 0;
        private int _Height = 0;
        private float[] _Scales; // 2D array is not thread-safe

        private readonly GrayscaleTextureAsset _asset = new();

        public bool IsAssetLoaded => _asset.IsAssetLoaded;

        public GrayscaleTexture2D(string assetName)
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
            _Height = 0;
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
                _Height = Texture.Height;
                _Scales = new float[_Width * _Height];
                GrayscaleTextureAsset.CopyRedChannel(Texture, _Scales);
            });
        }

        public float GetClamp(int x, int y)
        {
            if (!_asset.IsPrepared)
                return default;

            if (_Width == 0 || _Height == 0)
                return default;

            x = Math.Clamp(x, 0, _Width - 1);
            y = Math.Clamp(x, 0, _Height - 1);
            return _Scales[x + (y * _Height)];
        }

        public float GetRepeat(int x, int y)
        {
            if (!_asset.IsPrepared)
                return default;

            if (_Width == 0 || _Height == 0)
                return default;

            x %= _Width;
            y %= _Height;
            return _Scales[x + (y * _Height)];
        }
    }
}
