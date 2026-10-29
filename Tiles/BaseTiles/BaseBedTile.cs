using CalamityMod.Tiles;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseBedTile : ModTile
    {
        protected abstract int AssociatedItemType { get; }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j) => FurnitureCommon.BedRightClick(i, j);

        public override void MouseOver(int i, int j) => FurnitureCommon.MouseOver(i, j, AssociatedItemType);
    }
}
