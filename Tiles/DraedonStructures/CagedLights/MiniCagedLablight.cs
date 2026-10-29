using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniCagedLablight : MiniCagedLightTile<MiniCagedLablightItem>
    {
        protected override int FixtureDustType => DustID.KryptonMoss;
        protected override Vector3 LightColor => new Vector3(31f / 255f, 251f / 255f, 255f / 255f);
    }
}
