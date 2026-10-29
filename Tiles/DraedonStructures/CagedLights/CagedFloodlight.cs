using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class CagedFloodlight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedFloodlightItem>();
        protected override int FixtureDustType => DustID.BlueFlare;
        protected override Vector3 LightColor => new Vector3(105f / 255f, 195f / 255f, 193f / 255f);
    }
}
