using CalamityMod.Tiles;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseChairTile<TItem> : ModTile where TItem : ModItem
    {
        protected virtual bool IsWide => false;
        protected virtual bool LavaImmune => true;

        public override void SetStaticDefaults() => this.SetUpChair(ModContent.ItemType<TItem>(), LavaImmune);

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) =>
            FurnitureCommon.ChairSitInfo(i, j, ref info, fat: IsWide, hasOffset: IsWide);

        public override bool RightClick(int i, int j) => FurnitureCommon.ChairRightClick(i, j);

        public override void MouseOver(int i, int j) =>
            FurnitureCommon.ChairMouseOver(i, j, ModContent.ItemType<TItem>(), IsWide);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) =>
            settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
    }
}
