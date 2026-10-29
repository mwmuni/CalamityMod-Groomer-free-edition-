using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.PlayerTurrets
{
    public abstract class PlayerTargetingTurretTile<TEntity> : ModTile where TEntity : TENPCTargetingTurret
    {
        public const int Width = 3;
        public const int Height = 2;
        public const int OriginOffsetX = 1;
        public const int OriginOffsetY = 1;
        public const int SheetSquare = 18;

        public Asset<Texture2D> HeadTexture;

        protected abstract string TurretHeadTexture { get; }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;

            // Style3x2 provides the correct dimensions and origin.
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = false;

            TEntity te = ModContent.GetInstance<TEntity>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(67, 72, 81), Language.GetText("MapObject.Turret"));
            HitSound = SoundID.Item14;
            MineResist = 5f;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Electric);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % (Width * SheetSquare) / SheetSquare;
            int top = j - tile.TileFrameY % (Height * SheetSquare) / SheetSquare;

            TEntity te = CalamityUtils.FindTileEntity<TEntity>(i, j, Width, Height, SheetSquare);
            te?.Kill(left, top);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 36 || tile.TileFrameY != 0 || tile.IsTileActuallyInvisible())
                return;

            TEntity te = CalamityUtils.FindTileEntity<TEntity>(i, j, Width, Height, SheetSquare);
            if (te is null)
                return;

            int drawDirection = te.Direction;
            Color drawColor = Lighting.GetColor(i, j);

            HeadTexture ??= ModContent.Request<Texture2D>(TurretHeadTexture);
            Texture2D texture = HeadTexture.Value;
            Vector2 screenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + screenOffset;
            drawOffset.Y -= 2f;
            drawOffset.X += (drawDirection == -1 ? -10f : 2f) - 2f;

            SpriteEffects effects = drawDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            spriteBatch.Draw(texture, drawOffset, null, drawColor, te.Angle, texture.Size() * 0.5f, 1f, effects, 0f);
        }
    }
}
