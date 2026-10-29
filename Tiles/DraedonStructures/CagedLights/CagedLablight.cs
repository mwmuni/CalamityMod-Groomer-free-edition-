using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class CagedLablight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedLablightItem>();
        protected override int FixtureDustType => DustID.KryptonMoss;
        protected override Vector3 LightColor => new Vector3(31f / 255f, 251f / 255f, 255f / 255f);
    }
}
