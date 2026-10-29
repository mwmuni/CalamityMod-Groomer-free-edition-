using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniCagedBiolight : MiniCagedLightTile<MiniCagedBiolightItem>
    {
        protected override int FixtureDustType => DustID.GemEmerald;
        protected override Vector3 LightColor => new Vector3(9f / 255f, 195f / 255f, 0f);
    }
}
