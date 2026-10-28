using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenBed : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpBed(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenBed>(), true);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool RightClick(int i, int j) => FurnitureCommon.BedRightClick(i, j);

        public override void MouseOver(int i, int j) => FurnitureCommon.MouseOver(i, j, ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenBed>());
    }
}
