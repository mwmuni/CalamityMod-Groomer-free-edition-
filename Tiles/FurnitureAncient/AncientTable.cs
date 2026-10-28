using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientTable : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpTable(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientTable>(), true);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
