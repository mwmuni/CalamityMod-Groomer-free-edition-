using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    [LegacyName("AncientWorkbench")]
    public class AncientWorkBench : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpWorkBench(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientWorkBench>(), true);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
