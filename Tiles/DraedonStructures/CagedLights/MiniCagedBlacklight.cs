using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniCagedBlacklight : MiniCagedLightTile<MiniCagedBlacklightItem>
    {
        protected override int FixtureDustType => DustID.ShadowbeamStaff;
        protected override Vector3 LightColor => new Vector3(182f / 255f, 29f / 255f, 232f / 255f);
    }
}
