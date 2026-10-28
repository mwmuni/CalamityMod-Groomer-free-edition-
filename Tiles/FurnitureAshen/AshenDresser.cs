using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenDresser : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenDresser>());
            AddMapEntry(new Color(191, 142, 111), CalamityUtils.GetItemName<Items.Placeables.FurnitureAshen.AshenDresser>(), FurnitureCommon.GetMapChestName);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<Items.Placeables.FurnitureAshen.AshenDresser>();
        public override void MouseOver(int i, int j) => FurnitureCommon.DresserMouseOver<Items.Placeables.FurnitureAshen.AshenDresser>();
        public override void MouseOverFar(int i, int j) => FurnitureCommon.DresserMouseFar<Items.Placeables.FurnitureAshen.AshenDresser>();
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
        public override bool RightClick(int i, int j) => FurnitureCommon.DresserRightClick();
    }
}
