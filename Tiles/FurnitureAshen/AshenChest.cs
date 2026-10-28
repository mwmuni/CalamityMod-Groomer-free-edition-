using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenChest : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenChest>(), true);
            AddMapEntry(new Color(191, 142, 111), this.GetLocalization("MapEntry0"), MapChestName);
            AddMapEntry(new Color(174, 129, 92), this.GetLocalization("MapEntry1"), MapChestName);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

        public string MapChestName(string name, int i, int j) => FurnitureCommon.GetMapChestName(name, i, j);
        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);
        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            int option = frameX / 36;
            return this.GetLocalization("MapEntry" + option);
        }
        public override void MouseOver(int i, int j) => FurnitureCommon.ChestMouseOver<Items.Placeables.FurnitureAshen.AshenChest>(i, j);
        public override void MouseOverFar(int i, int j) => FurnitureCommon.ChestMouseFar<Items.Placeables.FurnitureAshen.AshenChest>(i, j);
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

        // Locked Chest stuff
        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;
        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            dustType = DustType;
            return true;
        }
        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i;
            int top = j;

            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            return FurnitureCommon.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
        }
    }
}
