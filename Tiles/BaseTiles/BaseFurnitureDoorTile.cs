using Terraria.GameContent.ObjectInteractions;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BaseFurnitureDoorTile : BaseRedTorchStoneDustTile
    {
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
    }
}
