using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseCustomTorchTile<TItem> : ModTile where TItem : ModItem
    {
        public Asset<Texture2D> FlameTexture;

        protected abstract Color TorchDustColor { get; }
        protected abstract Vector3 TorchLightColor { get; }
        protected abstract string FlameTexturePath { get; }
        protected abstract int FlameSparkDustType { get; }
        protected abstract bool IsInTorchLuckBiome(Player player);
        protected virtual int AlternateFlameSparkDustType => -1;
        protected virtual bool WaterImmune => false;
        protected virtual bool LavaImmune => false;

        public override void SetStaticDefaults() =>
            this.SetUpTorch(ModContent.ItemType<TItem>(), WaterImmune, LavaImmune);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, TorchDustColor, 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 66)
            {
                r = TorchLightColor.X;
                g = TorchLightColor.Y;
                b = TorchLightColor.Z;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                    offsetY = 4;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FlameTexture ??= ModContent.Request<Texture2D>(FlameTexturePath);
            CalamityUtils.DrawFlameEffect(FlameTexture.Value, i, j, 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 66)
            {
                int dustType = AlternateFlameSparkDustType < 0
                    ? FlameSparkDustType
                    : Main.rand.NextBool() ? AlternateFlameSparkDustType : FlameSparkDustType;
                CalamityUtils.DrawFlameSparks(dustType, 5, i, j);
            }
        }

        public override bool RightClick(int i, int j)
        {
            FurnitureCommon.RightClickBreak(i, j);
            return true;
        }

        public override float GetTorchLuck(Player player)
        {
            // Note: Total Torch luck never goes below zero
            return IsInTorchLuckBiome(player) ? 1f : -1f;
        }
    }
}
