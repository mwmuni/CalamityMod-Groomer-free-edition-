using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniCagedCinderlight : MiniCagedLightTile<MiniCagedCinderlightItem>
    {
        protected override int FixtureDustType => DustID.Torch;
        protected override Vector3 LightColor => new Vector3(252f / 255f, 184f / 255f, 29f / 255f);
    }
}
