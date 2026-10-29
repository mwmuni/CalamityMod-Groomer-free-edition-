using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class OrangeCoral : ColoredCoralTile
    {
        protected override int CoralDustType => DustID.Pixie;
        protected override Color CoralMapColor => new Color(255, 144, 63);
        protected override Vector3 CoralLightColor => new Vector3(0.92f, 0.62f, 0.42f);
    }
}
