using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniCagedFlamelight : MiniCagedLightTile<MiniCagedFlamelightItem>
    {
        protected override int FixtureDustType => DustID.RedTorch;
        protected override Vector3 LightColor => new Vector3(255f / 255f, 104f / 255f, 104f / 255f);
    }
}
