using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientBathtub : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpBathtub(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientBathtub>());

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
