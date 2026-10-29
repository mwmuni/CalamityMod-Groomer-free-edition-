using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class MagentaCoral : ColoredCoralTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;

        protected override int CoralDustType => DustID.Ice_Pink;
        protected override Color CoralMapColor => new Color(253, 112, 186);
        protected override Vector3 CoralLightColor => new Vector3(0.81f, 0.32f, 0.54f);
    }
}
