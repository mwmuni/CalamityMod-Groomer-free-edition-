using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseFountainTile<TItem> : ModTile where TItem : ModItem
    {
        protected abstract Color MapColor { get; }
        protected virtual bool IsLava => false;
        protected virtual int FirstDustType => DustID.Stone;
        protected virtual int SecondDustType => DustID.Water;
        protected virtual Color? FirstDustColor => new Color(119, 102, 255);
        protected virtual Color? SecondDustColor => new Color(255, 255, 255);
        protected virtual int AnimationFrameCount => 4;

        public sealed override void SetStaticDefaults() =>
            this.SetUpFountain(ModContent.ItemType<TItem>(), MapColor, IsLava);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Vector2 position = new Vector2(i, j) * 16f;
            CreateFountainDust(position, FirstDustType, FirstDustColor);
            CreateFountainDust(position, SecondDustType, SecondDustColor);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % AnimationFrameCount;
                frameCounter = 0;
            }
        }

        public override void HitWire(int i, int j) => FurnitureCommon.LightHitWire(Type, i, j, 2, 4);

        public override bool RightClick(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 2, 4);
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.dedServ && Main.tile[i, j].TileFrameX >= 36)
                ApplyFountainEffect(i, j);
        }

        protected virtual void ApplyFountainEffect(int i, int j)
        {
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        private static void CreateFountainDust(Vector2 position, int dustType, Color? color)
        {
            if (color.HasValue)
                Dust.NewDust(position, 16, 16, dustType, 0f, 0f, 1, color.Value, 1f);
            else
                Dust.NewDust(position, 16, 16, dustType);
        }
    }
}
