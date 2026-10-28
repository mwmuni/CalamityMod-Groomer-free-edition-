using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenTable : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpTable(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenTable>(), true);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
