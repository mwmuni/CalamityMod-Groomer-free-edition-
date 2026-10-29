using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class YellowCoral : ColoredCoralTile
    {
        protected override int CoralDustType => DustID.Firework_Yellow;
        protected override Color CoralMapColor => new Color(238, 207, 37);
        protected override Vector3 CoralLightColor => new Vector3(0.73f, 0.68f, 0.22f);
    }
}
