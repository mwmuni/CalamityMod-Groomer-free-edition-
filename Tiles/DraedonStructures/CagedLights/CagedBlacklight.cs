using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class CagedBlacklight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedBlacklightItem>();
        protected override int FixtureDustType => DustID.ShadowbeamStaff;
        protected override Vector3 LightColor => new Vector3(182f / 255f, 28f / 255f, 232f / 255f);
    }
}
