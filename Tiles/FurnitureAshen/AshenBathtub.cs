using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenBathtub : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpBathtub(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenBathtub>());

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
