using CalamityMod.Tiles;
using Terraria;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseToiletTile : ModTile
    {
        protected abstract int AssociatedItemType { get; }

        protected virtual bool IsWide => false;

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) =>
            FurnitureCommon.ChairSitInfo(i, j, ref info, fat: IsWide, shitter: true);

        public override bool RightClick(int i, int j) => FurnitureCommon.ChairRightClick(i, j);

        public override void MouseOver(int i, int j) =>
            FurnitureCommon.ChairMouseOver(i, j, AssociatedItemType, IsWide);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) =>
            settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int spawnX = i;
            int frameY = IsWide ? tile.TileFrameY % 40 : tile.TileFrameY;
            int spawnY = j - frameY / 18;

            Wiring.SkipWire(spawnX, spawnY);
            Wiring.SkipWire(spawnX, spawnY + 1);

            if (Wiring.CheckMech(spawnX, spawnY, 60))
            {
                Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
            }
        }
    }
}
