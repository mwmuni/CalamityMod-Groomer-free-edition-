using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenBookcase : BaseRedTorchStoneDustTile
    {
        public override void SetStaticDefaults() => this.SetUpBookcase(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenBookcase>(), true);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 25;
                frameCounter = 0;
            }
        }
    }
}
