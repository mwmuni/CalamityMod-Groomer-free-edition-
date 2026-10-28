using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientBookcase : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpBookcase(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientBookcase>(), true);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
