using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public abstract class BaseBlueWhiteFrameLightTile : ModTile
    {
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 0.8f;
                g = 0.9f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }
    }
}
