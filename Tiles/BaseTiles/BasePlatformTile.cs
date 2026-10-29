using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public abstract class BasePlatformTile : ModTile
    {
        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
