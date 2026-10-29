using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class CyanCoral : ColoredCoralTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;

        protected override int CoralDustType => DustID.HallowSpray;
        protected override Color CoralMapColor => new Color(109, 190, 221);
        protected override Vector3 CoralLightColor => new Vector3(0.43f, 0.71f, 0.82f);
    }
}
