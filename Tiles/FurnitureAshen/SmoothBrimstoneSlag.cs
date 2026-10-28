using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class SmoothBrimstoneSlag : BaseGlowMaskRedTorchStoneDustTile
    {
        public override void SetupStatic()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeSmoothTiles(Type);
            CalamityUtils.MergeDecorativeTiles(Type);
            CalamityUtils.MergeWithHell(Type);

            HitSound = SoundID.Tink;
            MineResist = 1f;
            AddMapEntry(new Color(61, 40, 61));
        }

        public override Color GetGlowMaskColor(int i, int j, TileDrawInfo drawData)
        {
            return new Color(25, 25, 25);
        }
    }
}
