using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class LimeCoral : ColoredCoralTile
    {
        protected override int CoralDustType => DustID.Terra;
        protected override Color CoralMapColor => new Color(176, 242, 96);
        protected override Vector3 CoralLightColor => new Vector3(139f / 255f, 206f / 255f, 56f / 255f);
    }
}
